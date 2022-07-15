using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DieFaceDetector : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PeriodicallyReportFace());
    }

    private IEnumerator PeriodicallyReportFace()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"Current face is {GetUpFace()}");
        }
    }

    private static readonly Vector3[] faces = new Vector3[]{Vector3.up, Vector3.right, Vector3.forward, Vector3.back, Vector3.left, Vector3.down};
    public int GetUpFace()
    {
        // which of the die's faces are pointing most toward the player?
        var bestFaceIndex = 0;
        var bestFacePointingness = GetFacePointingness(faces[0]);
        for (var faceIndex = 1; faceIndex < faces.Length; ++faceIndex)
        {
            var thisFacePointingness = GetFacePointingness(faces[faceIndex]);
            if (thisFacePointingness > bestFacePointingness)
            {
                bestFaceIndex = faceIndex;
                bestFacePointingness = thisFacePointingness;
            }
        }

        return bestFaceIndex;
    }

    private float GetFacePointingness(Vector3 faceDirection)
    {
        return Vector3.Dot(Vector3.up, transform.rotation * faceDirection);
    }

    public bool IsJacked()
    {
        return false;
    }
}
