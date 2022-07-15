using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DieStoppedDetector : MonoBehaviour
{
    [SerializeField]
    private float maximumLinearSpeedForStopped = 1e-4f;
    [SerializeField]
    private float maximumAngularSpeedForStopped = 1e-4f;
    [SerializeField]
    private int numberOfStableFramesForStopped = 10;

    delegate void OnNewlyStable();
    private OnNewlyStable onNewlyStable = null;

    private int numberOfFramesSinceUnstable = 0;

    private Rigidbody siblingBody = null;

    private void Awake()
    {
        siblingBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (IsStableThisFrame())
        {
            numberOfFramesSinceUnstable = 0;
        }
        else
        {
            if (!IsStable())
            {
                numberOfFramesSinceUnstable++;

                if (IsStable())
                {
                    ReportNewlyStable();
                }
            }
        }
    }

    private bool IsStableThisFrame()
    {
        return HasHighLinearMovement() || HasHighAngularMovement();
    }

    private bool HasHighLinearMovement()
    {
        return siblingBody.velocity.sqrMagnitude > Square(maximumLinearSpeedForStopped);
    }
    
    private float Square(float inValue)
    {
        return inValue * inValue;
    }

    private bool HasHighAngularMovement()
    {
        return siblingBody.angularVelocity.sqrMagnitude > Square(maximumAngularSpeedForStopped);
    }

    public bool IsStable()
    {
        return numberOfFramesSinceUnstable >= numberOfStableFramesForStopped;
    }

    private void ReportNewlyStable()
    {
        Debug.Log("Now stopped!");
        onNewlyStable?.Invoke();
    }
}
