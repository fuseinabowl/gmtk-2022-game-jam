using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;
    [SerializeField]
    private float fieldOfView;

    private void Update()
    {
        targetCamera.fieldOfView = fieldOfView;
    }
}
