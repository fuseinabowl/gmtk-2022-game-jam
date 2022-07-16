using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

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
    private Vector3 invalidDiceRethrowLeftPosition = Vector3.left;
    [SerializeField]
    private Vector3 invalidDiceRethrowRightPosition = Vector3.right;
    [SerializeField]
    private Vector3 invalidDiceRethrowBaseOffset = Vector3.back * 2f;
    [SerializeField]
    private float invalidDiceRethrowRandomOffset = 1f;
    [SerializeField]
    private float invalidDiceRethrowDuration = 0.5f;

    [SerializeField]
    private Vector3 arrangingTopLeftPosition = Vector3.zero;
    [SerializeField]
    private float arrangingHeight = 12f;
    [SerializeField]
    private float arrangingHorizontalSpacing = 12f;
    [SerializeField]
    private float arrangingDuration = 0.25f;

    [Serializable]
    private class FaceLookAt
    {
        [SerializeField]
        public Vector3 forward;
        [SerializeField]
        public Vector3 up;
    }
    [SerializeField]
    private List<FaceLookAt> arrangingDiceRotations = null;

    [SerializeField]
    private ConsumableMovements outputConsumableMovements = null;

    [SerializeField]
    private UIDocument gameUi = null;

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
        Assert.AreEqual(arrangingDiceRotations.Count, 6);
        Assert.IsNotNull(outputConsumableMovements);
        Assert.IsNotNull(gameUi);
    }

    private void Start()
    {
        dice = CreateDice();
        MakeDiceKinematic();
        MakeNextTurnButtonCollectDice();
        StartCoroutine(CollectAllDiceAndPrepareForRoll());
    }

    private void MakeNextTurnButtonCollectDice()
    {
        GetNextTurnButton().clicked += () =>
        {
            outputConsumableMovements.StopSharingMovements();
            StartCoroutine(CollectAllDiceAndPrepareForRoll());
        };
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

        public Quaternion startRotation;
        public Quaternion controlRotation0;
        public Quaternion controlRotation1;
        public Quaternion endRotation;
    }
    private IEnumerator RunBezierPaths(List<DieBezierPath> bezierPaths, float duration)
    {
        var startTime = Time.time;
        var endTime = startTime + duration;

        do
        {
            yield return 0;

            var timeProportion = Mathf.InverseLerp(startTime, endTime, Time.time);

            foreach (var diePath in bezierPaths)
            {
                var dieTransform = dice[diePath.dieIndex].gameObject.transform;
                dieTransform.position = BezierLerp(diePath.startPosition, diePath.control0, diePath.control1, diePath.endPosition, timeProportion);
                dieTransform.rotation = BezierQuaternionLerp(diePath.startRotation, diePath.controlRotation0, diePath.controlRotation1, diePath.endRotation, timeProportion);
            }
        } while (Time.time < endTime);
    }

    private Vector3 BezierLerp(Vector3 start, Vector3 control0, Vector3 control1, Vector3 end, float timeUnclamped)
    {
        var time = Mathf.Clamp(timeUnclamped, 0f, 1f);

        var mid = Vector3.Lerp(control0, control1, time);

        var startLerp = Vector3.Lerp(start, mid, time);
        var endLerp = Vector3.Lerp(mid, end, time);

        return Vector3.Lerp(startLerp, endLerp, time);
    }

    private Quaternion BezierQuaternionLerp(Quaternion start, Quaternion control0, Quaternion control1, Quaternion end, float timeUnclamped)
    {
        var time = Mathf.Clamp(timeUnclamped, 0f, 1f);

        var mid = Quaternion.Lerp(control0, control1, time);

        var startLerp = Quaternion.Lerp(start, mid, time);
        var endLerp = Quaternion.Lerp(mid, end, time);

        return Quaternion.Lerp(startLerp, endLerp, time);
    }

    private IEnumerator CollectAllDiceAndPrepareForRoll()
    {
        GetNextTurnButton().SetEnabled(false);

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

                    startRotation = die.gameObject.transform.rotation,
                    controlRotation0 = die.gameObject.transform.rotation,
                    controlRotation1 = die.gameObject.transform.rotation,
                    endRotation = die.gameObject.transform.rotation,
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
            die.body.velocity = lastFrameVelocity + UnityEngine.Random.insideUnitSphere * throwRandomVelocity;
            die.body.angularVelocity = UnityEngine.Random.insideUnitSphere * throwRandomAngularVelocity;
            die.body.isKinematic = false;
        }

        yield return new WaitForFixedUpdate();

        StartCoroutine(WaitForDiceThrowResult());
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

    private IEnumerator WaitForDiceThrowResult()
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
            
            if (jackedDieIndices.Count > 0)
            {
                yield return RethrowInvalidDice(jackedDieIndices);
            }
        } while (jackedDieIndices.Count > 0);

        foreach (var die in dice)
        {
            die.body.isKinematic = true;
        }

        StartCoroutine(ArrangeDiceResults());
    }

    private bool AreAllDiceStable()
    {
        return dice.All(die => die.stoppedDetector.IsStable());
    }

    private IEnumerable<int> GetJackedDieIndices()
    {
        return Enumerable.Range(0, dice.Count).Where(dieIndex => dice[dieIndex].faceDetector.IsJacked());
    }

    private IEnumerator RethrowInvalidDice(List<int> jackedDieIndices)
    {
        foreach (var dieIndex in jackedDieIndices)
        {
            var die = dice[dieIndex];
            die.body.isKinematic = true;
        }

        var bezierPaths = Enumerable.Range(0, jackedDieIndices.Count)
            .Select(jackedIndex => {
                var jackedProportionOnLine = (float)(jackedIndex + 1) / (jackedDieIndices.Count + 1);
                var dieIndex = jackedDieIndices[jackedIndex];
                var die = dice[dieIndex];
                var restartPosition = Vector3.Lerp(invalidDiceRethrowLeftPosition, invalidDiceRethrowRightPosition, jackedProportionOnLine);
                var restartThrowOffset = invalidDiceRethrowBaseOffset + UnityEngine.Random.insideUnitSphere * invalidDiceRethrowRandomOffset;
                return new DieBezierPath{
                    dieIndex = dieIndex,

                    startPosition = die.gameObject.transform.position,
                    control0 = die.gameObject.transform.position + collectDiceRiseOffset,
                    control1 = restartPosition + restartThrowOffset,
                    endPosition = restartPosition,

                    startRotation = die.gameObject.transform.rotation,
                    controlRotation0 = die.gameObject.transform.rotation,
                    controlRotation1 = die.gameObject.transform.rotation,
                    endRotation = die.gameObject.transform.rotation,
                };
            })
            .ToList();

        yield return RunBezierPaths(bezierPaths, invalidDiceRethrowDuration);

        for (var jackedIndex = 0; jackedIndex < jackedDieIndices.Count; ++jackedIndex)
        {
            var bezier = bezierPaths[jackedIndex];
            var dieIndex = jackedDieIndices[jackedIndex];
            var body = dice[dieIndex].body;
            body.velocity = (bezier.endPosition - bezier.control1) / invalidDiceRethrowDuration;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = false;
        }

        yield return new WaitForFixedUpdate();
    }

    private IEnumerator ArrangeDiceResults()
    {
        var seenFaceInstances = Enumerable.Range(0,6).Select(index => 0).ToList();

        var topPosition = arrangingTopLeftPosition;
        var bottomPosition = topPosition + Vector3.back * arrangingHeight;

        var beziers = dice
            .Select((die, dieIndex) => {
                var face = die.faceDetector.GetUpFace();
                var faceVerticalPosition = Vector3.Lerp(topPosition, bottomPosition, (float)face / 5f);
                var faceHorizontalOffset = Vector3.right * seenFaceInstances[face] * arrangingHorizontalSpacing;
                var targetPosition = faceVerticalPosition + faceHorizontalOffset;

                var targetLookAt = arrangingDiceRotations[face];
                var targetRotation = Quaternion.LookRotation(targetLookAt.forward, targetLookAt.up);

                seenFaceInstances[face] = seenFaceInstances[face] + 1;

                return new DieBezierPath{
                    dieIndex = dieIndex,

                    startPosition = die.gameObject.transform.position,
                    control0 = die.gameObject.transform.position,
                    control1 = targetPosition,
                    endPosition = targetPosition,

                    startRotation = die.gameObject.transform.rotation,
                    controlRotation0 = die.gameObject.transform.rotation,
                    controlRotation1 = targetRotation,
                    endRotation = targetRotation,
                };
            })
            .ToList();

        yield return RunBezierPaths(beziers, arrangingDuration);

        ReportResultsToWeightGame(seenFaceInstances);
        EnableNextTurnButton();
    }

    private void ReportResultsToWeightGame(List<int> seenFaceInstances)
    {
        // TODO
        Debug.Log("Seen faces:\n"+
            $"up: {seenFaceInstances[0]}\n"+
            $"left: {seenFaceInstances[1]}\n"+
            $"left-up: {seenFaceInstances[2]}\n"+
            $"right-up: {seenFaceInstances[3]}\n"+
            $"right: {seenFaceInstances[4]}\n"+
            $"stop: {seenFaceInstances[5]}\n"
        );

        outputConsumableMovements.SetAvailableMovements(
            seenFaceInstances[0],
            seenFaceInstances[1],
            seenFaceInstances[2],
            seenFaceInstances[3],
            seenFaceInstances[4],
            seenFaceInstances[5]
        );
    }

    private void EnableNextTurnButton()
    {
        GetNextTurnButton().SetEnabled(true);
    }

    private Button GetNextTurnButton()
    {
        return gameUi.rootVisualElement.Q<VisualElement>("MainPanel").Q<Button>("NextTurn");
    }
}
