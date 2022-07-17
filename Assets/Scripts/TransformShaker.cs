using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformShaker : MonoBehaviour
{
    [SerializeField]
    private Vector3 shakeShape = Vector3.one;
    [SerializeField]
    private float shakeAmplitude = 1f;
    [SerializeField]
    private float shakeFrequency = 1f;
    [SerializeField]
    private AnimationCurve shakeBehaviour = AnimationCurve.EaseInOut(0f,0f,1f,1f);

    private Vector3 lastShakePosition = Vector3.zero;
    private float timeAtLastShakePosition = 0f;
    private Vector3 nextShakePosition = Vector3.zero;

    private void Update()
    {
        var shakeDuration = 1f / shakeFrequency;

        CheckForEndOfCurrentShake(shakeDuration);
        MoveToCurrentShake(shakeDuration);
    }

    private void CheckForEndOfCurrentShake(float shakeDuration)
    {
        var nextShakeTime = timeAtLastShakePosition + shakeDuration;
        var timeSinceLastShakePosition = Time.time - timeAtLastShakePosition;
        if (timeSinceLastShakePosition > shakeDuration)
        {
            StartNextShake(nextShakeTime);
        }
    }

    private void StartNextShake(float startTime)
    {
        timeAtLastShakePosition = startTime;
        lastShakePosition = nextShakePosition;
        nextShakePosition = Vector3.Scale(Random.insideUnitSphere, shakeShape);
    }


    private void MoveToCurrentShake(float shakeDuration)
    {
        var timeSinceLastShakePosition = Time.time - timeAtLastShakePosition;
        var lerpTime = shakeBehaviour.Evaluate(timeSinceLastShakePosition / shakeDuration);
        var rawShakePosition = Vector3.Lerp(lastShakePosition, nextShakePosition, lerpTime);
        var modulatedShakePosition = rawShakePosition * shakeAmplitude;
        transform.localPosition = modulatedShakePosition;
    }
}
