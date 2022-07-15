using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class DiceTurnController : MonoBehaviour
{
    [SerializeField]
    private GameObject diePrefab = null;
    [SerializeField]
    private int numberOfDice = 6;

    [SerializeField]
    private Camera playerCamera = null;

    [SerializeField]
    private float collectDiceDuration = 0.5f;
    [SerializeField]
    [Tooltip("When picking up the dice to reroll, how high should the dice go when animating back to the throwable bundle?")]
    private Vector3 collectDiceRiseOffset = Vector3.up;
    [SerializeField]
    private Vector3 collectDiceLocation = Vector3.zero;
    [SerializeField]
    private float collectDiceSpreadRadius = 0.5f;

    [SerializeField]
    private float throwableBundleClickableRadius = 1f;

    [SerializeField]
    private float jackedRethrowDiceDuration = 0.5f;

    private class Die
    {
        public GameObject gameObject = null;
        public Rigidbody body = null;
        public DieFaceDetector faceDetector = null;
        public DieStoppedDetector stoppedDetector = null;
    }
    private List<Die> dice = null;

    private void Start()
    {
        dice = CreateDice();
        MakeDiceKinematic();
        StartCoroutine(CollectAllDiceAndPrepareForRoll());
    }

    private List<Die> CreateDice()
    {
        return Enumerable.Range(0, numberOfDice)
            .Select(dieIndex => Instantiate(diePrefab))
            .Select(die =>
                new Die{
                    gameObject = die,
                    body = die.GetComponent<Rigidbody>(),
                    faceDetector = die.GetComponent<DieFaceDetector>(),
                    stoppedDetector = die.GetComponent<DieStoppedDetector>(),
                })
            .ToList();
    }

    private void MakeDiceKinematic()
    {
        foreach (var die in dice)
        {
            die.body.isKinematic = true;
        }
    }

    private class DieBezierPath
    {
        public int dieIndex;
        public Vector3 startPosition;
        public Vector3 control0;
        public Vector3 control1;
        public Vector3 endPosition;
    }
    private IEnumerator RunBezierPaths(List<DieBezierPath> bezierPaths, float duration)
    {
        var startTime = Time.time;
        var endTime = startTime + duration;

        while (Time.time < endTime)
        {
            var timeProportion = Mathf.InverseLerp(startTime, endTime, Time.time);

            foreach (var diePath in bezierPaths)
            {
                var dieTransform = dice[diePath.dieIndex].gameObject.transform;
                dieTransform.position = BezierLerp(diePath.startPosition, diePath.control0, diePath.control1, diePath.endPosition, timeProportion);
            }

            yield return 0;
        }
    }

    private Vector3 BezierLerp(Vector3 start, Vector3 control0, Vector3 control1, Vector3 end, float time)
    {
        var mid = Vector3.Lerp(control0, control1, time);

        var startLerp = Vector3.Lerp(start, mid, time);
        var endLerp = Vector3.Lerp(mid, end, time);

        return Vector3.Lerp(startLerp, endLerp, time);
    }

    private IEnumerator CollectAllDiceAndPrepareForRoll()
    {
        foreach (var die in dice)
        {
            Assert.IsTrue(die.body.isKinematic);
        }

        // create bezier path control points for each die
        var bezierPaths = Enumerable.Range(0, dice.Count)
            .Select(dieIndex => {
                var die = dice[dieIndex];
                var scrunchPosition = collectDiceLocation + Random.insideUnitSphere * collectDiceSpreadRadius;
                return new DieBezierPath{
                    dieIndex = dieIndex,
                    startPosition = die.gameObject.transform.position,
                    control0 = die.gameObject.transform.position + collectDiceRiseOffset,
                    control1 = scrunchPosition,
                    endPosition = scrunchPosition,
                };
            })
            .ToList();

        yield return RunBezierPaths(bezierPaths, collectDiceDuration);

        StartCoroutine(UserClicksAndThrowsDiceBundle());
    }

    private IEnumerator UserClicksAndThrowsDiceBundle()
    {
        while (!HasUserClickedOnTheDiceBundleThisFrame())
        {
            yield return 0;
        }

        Debug.Log("Picking up dice");

        while (IsUserCurrentlyHoldingMouseDown())
        {
            MoveDiceBundleToCursor();
            yield return 0;
        }

        Debug.Log("Throwing dice");

        StartCoroutine(ThrowDiceAndWaitForResult());
    }

    private bool HasUserClickedOnTheDiceBundleThisFrame()
    {
        if (Input.GetMouseButtonDown(0) && !IsClickingOnUi())
        {
            var clickWorldRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            var rayOriginToBundleOffset = collectDiceLocation - clickWorldRay.origin;
            // project onto ray to get closest point
            var distanceAlongRay = Vector3.Dot(rayOriginToBundleOffset, clickWorldRay.direction);
            var rayClosestPoint = clickWorldRay.GetPoint(distanceAlongRay);

            var distanceFromRaySquared = (rayClosestPoint - collectDiceLocation).sqrMagnitude;

            return distanceFromRaySquared < (throwableBundleClickableRadius * throwableBundleClickableRadius);
        }
        return false;
    }

    static private bool IsClickingOnUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsUserCurrentlyHoldingMouseDown()
    {
        return Input.GetMouseButton(0);
    }

    private void MoveDiceBundleToCursor()
    {
        // TODO
        // move dice bundle around (inside box)
    }

    private IEnumerator ThrowDiceAndWaitForResult()
    {
        var jackedDieIndices = new List<int>();
        do
        {
            while (!AreAllDiceStable())
            {
                yield return new WaitForFixedUpdate();
            }

            jackedDieIndices.Clear();
            jackedDieIndices.AddRange(GetJackedDieIndices());
            
            yield return RethrowJackedDice(jackedDieIndices);
        } while (jackedDieIndices.Count > 0);

        // TODO
        // StartCoroutine(ReportToWeightGameAndArrangeDiceResults());
    }

    private bool AreAllDiceStable()
    {
        return dice.All(die => die.stoppedDetector.IsStable());
    }

    private IEnumerable<int> GetJackedDieIndices()
    {
        return Enumerable.Range(0, dice.Count).Where(dieIndex => dice[dieIndex].faceDetector.IsJacked());
    }

    private IEnumerator RethrowJackedDice(List<int> jackedDieIndices)
    {
        // TODO
        // create bezier control points

        var endTime = Time.time + jackedRethrowDiceDuration;
        while (Time.time < endTime)
        {
            // TODO
            // move target dice to rethrow location
            yield return 0;
        }
    }
}