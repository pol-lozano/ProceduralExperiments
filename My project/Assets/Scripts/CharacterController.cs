using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] LegStepper leftLegStepper;
    [SerializeField] LegStepper rightLegStepper;
    [SerializeField] LegStepper leftBackLegStepper;
    [SerializeField] LegStepper rightBackLegStepper;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    // Only allow diagonal leg pairs to step together
    IEnumerator LegUpdateCoroutine()
    {
        // Run continuously
        while (true)
        {
            // Try moving one diagonal pair of legs
            do
            {
                rightBackLegStepper.TryMove();
                leftLegStepper.TryMove();
                // Wait a frame
                yield return null;

                // Stay in this loop while either leg is moving.
                // If only one leg in the pair is moving, the calls to TryMove() will let
                // the other leg move if it wants to.
            } while (leftLegStepper.IsMoving);

            // Do the same thing for the other diagonal pair
            do
            {
                leftBackLegStepper.TryMove();
                rightLegStepper.TryMove();
                yield return null;
            } while (rightLegStepper.IsMoving);
        }
    }
}
