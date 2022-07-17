using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractJack : MonoBehaviour
{
    [SerializeField]
    private Rigidbody jack = null;
    [SerializeField]
    private bool isAttracting = false;
    [SerializeField]
    private AnimationCurve attractOverTime = AnimationCurve.EaseInOut(0f,0f, 0.2f,1f);

    private bool wasAttracting = false;
    private float attractStartTime = 0f;
    private Vector3 attractStartPosition = Vector3.zero;
    private Quaternion attractStartRotation = Quaternion.identity;

    private void Update()
    {
        if (isAttracting)
        {
            if (!wasAttracting)
            {
                StartAttracting();
                wasAttracting = true;
            }

            AnimateAttraction();
        }
        else
        {
            if (wasAttracting)
            {
                StopAttracting();
                wasAttracting = false;
            }
        }
    }

    private void StartAttracting()
    {
        jack.isKinematic = true;
        attractStartTime = Time.time;
        attractStartPosition = jack.transform.position;
        attractStartRotation = jack.transform.rotation;
    }

    private void AnimateAttraction()
    {
        var timeSinceAttractStart = Time.time - attractStartTime;
        var attractionProportion = attractOverTime.Evaluate(timeSinceAttractStart);
        jack.transform.position = Vector3.Lerp(attractStartPosition, transform.position, attractionProportion);
        jack.transform.rotation = Quaternion.Slerp(attractStartRotation, transform.rotation, attractionProportion);
    }

    private void StopAttracting()
    {
        jack.isKinematic = false;
    }
}
