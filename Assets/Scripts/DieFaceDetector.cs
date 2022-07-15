using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DieFaceDetector : MonoBehaviour
{
    [SerializeField]
    [Range(0f,1f)]
    private float maximumFaceTiltBeforeJacked = 1e-3f;

    private static readonly Vector3[] faces = new Vector3[]{Vector3.up, Vector3.right, Vector3.forward, Vector3.back, Vector3.left, Vector3.down};
    private struct FaceAndUpness
    {
        public int faceIndex;
        public float upness;
    }
    private FaceAndUpness GetBestUpFace()
    {
        // which of the die's faces are pointing most toward the player?
        var bestFaceIndex = 0;
        var bestFaceUpness = GetFacePointingness(faces[0]);
        for (var faceIndex = 1; faceIndex < faces.Length; ++faceIndex)
        {
            var thisFaceUpness = GetFacePointingness(faces[faceIndex]);
            if (thisFaceUpness > bestFaceUpness)
            {
                bestFaceIndex = faceIndex;
                bestFaceUpness = thisFaceUpness;
            }
        }

        return new FaceAndUpness{
            faceIndex = bestFaceIndex,
            upness = bestFaceUpness,
        };
    }

    public int GetUpFace()
    {
        return GetBestUpFace().faceIndex;
    }

    private float GetFacePointingness(Vector3 faceDirection)
    {
        return Vector3.Dot(Vector3.up, transform.rotation * faceDirection);
    }

    public bool IsJacked()
    {
        return GetBestUpFace().upness < (1f - maximumFaceTiltBeforeJacked);
    }
}
