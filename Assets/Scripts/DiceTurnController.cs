using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class DiceTurnController : MonoBehaviour
{
    [SerializeField]
    private GameObject diePrefab = null;
    [SerializeField]
    private int numberOfDice = 6;

    [SerializeField]
    private float collectDiceDuration = 0.5f;

    [SerializeField]
    private float rethrowDiceDuration = 0.5f;

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

        while (!IsUserCurrentlyHoldingMouseDown())
        {
            MoveDiceBundleToCursor();
            yield return 0;
        }

        StartCoroutine(ThrowDiceAndWaitForResult());
    }

    private bool HasUserClickedOnTheDiceBundleThisFrame()
    {
        // TODO
        return false;
    }

    private bool IsUserCurrentlyHoldingMouseDown()
    {
        // TODO
        return false;
    }

    private void MoveDiceBundleToCursor()
    {
        // TODO
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

        var endTime = Time.time + rethrowDiceDuration;
        while (Time.time < endTime)
        {
            // TODO
            // move target dice to rethrow location
            yield return 0;
        }
    }
}
