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
    private List<Vector3> collectDiceOffsets = new List<Vector3>();

    [SerializeField]
    private AnimationCurve grabScrunchOverTime = AnimationCurve.EaseInOut(0f, 1f, 0.1f, 0.8f);
    [SerializeField]
    private AnimationCurve grabResponseOverTime = AnimationCurve.EaseInOut(0f, 0f, 0.1f, 1f);
    [SerializeField]
    private Vector2 grabMinBounds = Vector2.zero;
    [SerializeField]
    private Vector2 grabMaxBounds = Vector2.zero;
    [SerializeField]
    private float throwableBundleClickableRadius = 1f;

    [SerializeField]
    private float throwRandomVelocity = 1f;
    [SerializeField]
    private float throwRandomAngularVelocity = 1f;

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

    private void Awake()
    {
        Assert.AreEqual(collectDiceOffsets.Count, numberOfDice);
    }

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
                var scrunchPosition = collectDiceLocation + collectDiceOffsets[dieIndex];
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

        var grabTime = Time.time;
        var lastFramePosition = MoveDiceBundleToCursor(0f);
        var lastFrameVelocity = Vector3.zero;
        while (IsUserCurrentlyHoldingMouseDown())
        {
            yield return 0;
            var grabDuration = Time.time - grabTime;
            var thisFramePosition = MoveDiceBundleToCursor(grabDuration);
            lastFrameVelocity = (thisFramePosition - lastFramePosition) / Time.deltaTime;
            lastFramePosition = thisFramePosition;
        }

        foreach (var die in dice)
        {
            die.body.velocity = lastFrameVelocity + Random.insideUnitSphere * throwRandomVelocity;
            die.body.angularVelocity = Random.insideUnitSphere * throwRandomAngularVelocity;
            die.body.isKinematic = false;
        }

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

    private Vector3 MoveDiceBundleToCursor(float grabDuration)
    {
        var rawGrabPoint = GetMouseLocationOnGrabPlane();
        var grabPointInsideBounds = ClampGrabPointInsideBounds(rawGrabPoint);
        var grabResponseMultiplier = grabResponseOverTime.Evaluate(grabDuration);
        var diceBundleCenter = Vector3.Lerp(collectDiceLocation, grabPointInsideBounds, grabResponseMultiplier);

        var scrunchMultiplier = grabScrunchOverTime.Evaluate(grabDuration);

        foreach (var dieIndex in Enumerable.Range(0, dice.Count))
        {
            var die = dice[dieIndex];
            var scrunchOffset = collectDiceOffsets[dieIndex];

            die.gameObject.transform.position = diceBundleCenter + scrunchOffset * scrunchMultiplier;
        }

        return diceBundleCenter;
    }

    private Vector3 GetMouseLocationOnGrabPlane()
    {
        var mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        var grabHeight = collectDiceLocation.y;
        var planeDistanceAlongRay = -(Vector3.Dot(mouseRay.origin, Vector3.up) - grabHeight) / Vector3.Dot(mouseRay.direction, Vector3.up);
        return mouseRay.origin + planeDistanceAlongRay * mouseRay.direction;
    }

    private Vector3 ClampGrabPointInsideBounds(Vector3 rawGrabPoint)
    {
        var clampedX = Mathf.Clamp(rawGrabPoint.x, grabMinBounds.x, grabMaxBounds.x);
        var clampedZ = Mathf.Clamp(rawGrabPoint.z, grabMinBounds.y, grabMaxBounds.y);

        return new Vector3(clampedX, rawGrabPoint.y, clampedZ);
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
