using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieController : MonoBehaviour
{
    [SerializeField]
    private float impulse = 10f;

    void Start()
    {
        var body = GetComponent<Rigidbody>();
        body.AddForce(Vector3.forward * impulse, ForceMode.Impulse);
    }
}
