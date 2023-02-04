using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public InputHandler input;

    [System.Serializable]
    public class AnimatedPart
    {
        public Transform transform;

        public Vector3 eulerRotationsYawDelta;
        public Vector3 eulerRotationsVelocity;

        public float rotationDamp;

        [HideInInspector] 
        public Quaternion rotation;
        [HideInInspector]
        public Quaternion startRotation;
    }

    public Transform visuals;

    [Header("Movement")]
    public float acceleration;
    public float maxVelocity;

    [Header("Animation")]
    public float visualsRotationDamp;
    public float yawDeltaMax;
    public float yawDeltaDamp;
    public AnimatedPart[] animatedParts;

    //Parts
    public Rigidbody rb;

    float yawDelta;
    Vector3 lastForward;

    public Vector2 RawInput { get; private set; }


    private void Start()
    {
        for (int i = 0; i < animatedParts.Length; i++)
        {
            animatedParts[i].startRotation = animatedParts[i].transform.localRotation;
        }
    }
    private void OnEnable()
    {
        input.moveEvent += OnMove;
        input.jumpEvent += OnJump;
    }

    private void OnDisable()
    {
        input.moveEvent -= OnMove;
        input.jumpEvent -= OnJump;
    }

    private void OnJump()
    {
        rb.AddForce(Vector3.up * PhysicsHelper.CalculateJumpVelocity(5000f, Physics.gravity.y));
    }

    private void Update()
    {
        Move();
    }

    protected void Move()
    {
        Vector3 input = GetInput();

        //Accelerate
        if (input.magnitude > float.Epsilon)
            Accelerate(input, maxVelocity);
        else
            Decelerate();
    }

    public void Accelerate(Vector3 input, float speed)
    {
        rb.velocity = new Vector3(
            Mathf.MoveTowards(rb.velocity.x, input.x * speed, acceleration * Time.deltaTime),
            rb.velocity.y,
            Mathf.MoveTowards(rb.velocity.z, input.z * speed, acceleration * Time.deltaTime));
    }

    public void Decelerate()
    {
        rb.velocity = new Vector3(
           Mathf.MoveTowards(rb.velocity.x, 0, acceleration * Time.deltaTime),
           rb.velocity.y,
           Mathf.MoveTowards(rb.velocity.z, 0, acceleration * Time.deltaTime));
    }


    private void OnMove(Vector2 input) => RawInput = Vector2.ClampMagnitude(input, 1f);

    public Vector3 GetInput()
    {
        Vector3 correctedHorizontal = Camera.main.transform.right;
        correctedHorizontal.y = 0f;
        correctedHorizontal.Normalize();
        Vector3 correctedVertical = Camera.main.transform.forward;
        correctedVertical.y = 0f;
        correctedVertical.Normalize();
        return RawInput.x * correctedHorizontal + RawInput.y * correctedVertical;
    }

    private void LateUpdate()
    {
        Animate();
    }

    private void Animate()
    {
        if (rb.velocity.sqrMagnitude > .1f)
        {
            Quaternion lookRot = Quaternion.LookRotation(rb.velocity, Vector3.up);
            visuals.rotation = Quaternion.Slerp(visuals.rotation, lookRot, Time.deltaTime * visualsRotationDamp);
        }

        float newYawDelta = Vector3.SignedAngle(lastForward, visuals.forward, Vector3.up) / Time.deltaTime;
        lastForward = visuals.forward;

        if (Mathf.Abs(newYawDelta) > yawDeltaMax)
        {
            newYawDelta = (newYawDelta >= 0) ? yawDeltaMax : -yawDeltaMax;
        }

        yawDelta = Mathf.Lerp(yawDelta, newYawDelta * 0.01f, Time.deltaTime * yawDeltaDamp);

        Vector3 localVelocity = visuals.InverseTransformDirection(rb.velocity);

        localVelocity = new Vector3(localVelocity.z, localVelocity.x, localVelocity.x);

        for (int i = 0; i < animatedParts.Length; i++)
        {
            Vector3 newYawDeltaRot = animatedParts[i].eulerRotationsYawDelta * yawDelta;
            Vector3 newVelocityRot = Vector3.Scale(animatedParts[i].eulerRotationsVelocity, localVelocity);

            Quaternion newRot = Quaternion.Euler(newVelocityRot + newYawDeltaRot);

            animatedParts[i].rotation = Quaternion.Slerp(animatedParts[i].rotation, newRot, Time.deltaTime * animatedParts[i].rotationDamp);
            animatedParts[i].transform.localRotation = animatedParts[i].startRotation * animatedParts[i].rotation;
        }
    }
}
