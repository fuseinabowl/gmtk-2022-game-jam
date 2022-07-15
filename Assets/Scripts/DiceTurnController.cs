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
        StartCoroutine(GrabAllDiceAndPrepareForRoll());
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

    private IEnumerator GrabAllDiceAndPrepareForRoll()
    {
        foreach (var die in dice)
        {
            Assert.IsTrue(die.body.isKinematic);
        }

        // TODO
        // create bezier path control points for each die

        var endTime = Time.time + collectDiceDuration;
        while (Time.time < endTime)
        {
            // TODO
            // each frame, move the die along their path according to the time
            yield return 0; // wait one frame
        }

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
