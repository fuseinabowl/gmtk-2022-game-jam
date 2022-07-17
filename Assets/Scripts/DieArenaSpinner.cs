using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DieArenaSpinner : MonoBehaviour
{
    private List<ConstantForce> containedArtificialGravities = new List<ConstantForce>();

    [SerializeField]
    private Rigidbody dieArenaBody = null;

    [SerializeField]
    private AnimationCurve angularVelocityOverThrowMultiplier = AnimationCurve.EaseInOut(0f,0f,1f,1f);
    [SerializeField]
    private float throwAngularVelocityMin = -5f;
    [SerializeField]
    private float throwAngularVelocityMax = 25f;
    [SerializeField]
    private float throwDuration = 1f;

    [SerializeField]
    private float airborneDuration = 1f;

    [SerializeField]
    private int freeBouncesMin = 3;
    [SerializeField]
    private int freeBouncesMax = 7;

    private void Start()
    {
        StartCoroutine(WaitThenSpin());
    }

    private IEnumerator WaitThenSpin()
    {
        yield return new WaitForSeconds(3f);
        StartSpinning(0);
    }

    public void OnDieArenaArtificialGravityComponentsAdded(IEnumerable<ConstantForce> newArtificialGravityComponents)
    {
        containedArtificialGravities.AddRange(newArtificialGravityComponents);
    }

    public void StartSpinning(int finalSideIndex)
    {
        StartCoroutine(ThrowDiceToSide(finalSideIndex));
    }

    private IEnumerator ThrowDiceToSide(int finalSideIndex)
    {
        Assert.IsTrue(dieArenaBody.isKinematic);

        yield return new WaitForFixedUpdate();

        // pick up dice
        // some small smoothly changing rotations
        var thisThrowAngularVelocity = Mathf.Lerp(throwAngularVelocityMin, throwAngularVelocityMax, Random.value);

        var numberOfFramesInThrow = throwDuration / Time.fixedDeltaTime;
        for (var frameIndex = 0; frameIndex < numberOfFramesInThrow; ++frameIndex)
        {
            var thisFrameThrowAngularVelocity = thisThrowAngularVelocity * angularVelocityOverThrowMultiplier.Evaluate((float)frameIndex / numberOfFramesInThrow);
            dieArenaBody.MoveRotation(Quaternion.AngleAxis(thisFrameThrowAngularVelocity, Vector3.up) * dieArenaBody.rotation);
            yield return new WaitForFixedUpdate();
        }
        // gravity still high
        // maybe some lateral acceleration?

        // disable artificial gravity
        foreach (var containedAG in containedArtificialGravities)
        {
            containedAG.enabled = false;
        }

        // throw dice
        // slow, unchanging angular velocity
        // zero gravity
        var numberOfFramesAirborne = airborneDuration / Time.fixedDeltaTime;
        for (var frameIndex = 0; frameIndex < numberOfFramesAirborne; ++frameIndex)
        {
            dieArenaBody.MoveRotation(Quaternion.AngleAxis(thisThrowAngularVelocity, Vector3.up) * dieArenaBody.rotation);
            yield return new WaitForFixedUpdate();
        }

        // free bouncing
        // frequent large changes to angular velocity
        // coincide with large downward delta linear velocities
        // otherwise zero gravity
        var numberOfFreeBounces = Random.Range(freeBouncesMin, freeBouncesMax);
        for (var bounceIndex = 0; bounceIndex < numberOfFreeBounces; ++bounceIndex)
        {
            // how long until the next bounce?
        }

        // end bouncing
        // find closest angle to land on the desired side
        // frequent large changes to angular velocity, going towards specific angles that are closer and closer to flat on the desired side
        // coincide with smaller and smaller downward delta linear velocities
        // otherwise zero gravity

        // finish
        // reenable artificial gravity
        foreach (var containedAG in containedArtificialGravities)
        {
            containedAG.enabled = true;
        }
    }
}
