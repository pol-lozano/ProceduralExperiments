using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    //Default foot position
    [SerializeField] private Transform homeTransform;
    [SerializeField] private float wantStepAtDistance;
    [SerializeField] private float moveDuration;

    [SerializeField] private float stepHeightFraction = 2;
    // Fraction of the max distance from home we want to overshoot by
    [SerializeField] private float stepOvershootFraction;

    private bool isMoving;

    public bool IsMoving => isMoving;

    public void TryMove()
    {
        if (isMoving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we are too far off in position or rotation
        if (distFromHome > wantStepAtDistance)
        {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        isMoving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion endRot = homeTransform.rotation;

        // Directional vector from the foot to the home position
        Vector3 towardHome = (homeTransform.position - transform.position);
        // Total distnace to overshoot by   
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;


        //Ground the point
        if (Physics.Raycast(rayOrigin.position + (transform.up * .1f), Vector3.down, out RaycastHit hit, 10.0f))
        {
            //Set foot position
            homeTransform.position = hit.point;
            //Set foot rotation
            Quaternion rot = Quaternion.LookRotation(rayOrigin.forward);
            homeTransform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * rot;
        }

        // Apply the overshoot
        Vector3 endPoint = homeTransform.position + overshootVector;

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance (arbitrarily)
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / stepHeightFraction;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);

            // Quadratic bezier curve
            transform.position =
              Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
              );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        isMoving = false;
    }
}
