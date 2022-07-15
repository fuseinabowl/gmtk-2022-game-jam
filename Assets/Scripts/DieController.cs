using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieController : MonoBehaviour
{
    [SerializeField]
    private float minImpulse = 5f;
    [SerializeField]
    private float maxImpulse = 10f;

    void Start()
    {
        var body = GetComponent<Rigidbody>();
        var randomImpulse = Mathf.Lerp(minImpulse, maxImpulse, Random.value);
        body.AddForce(Vector3.forward * randomImpulse, ForceMode.Impulse);
    }
}
