using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedZoneController : MonoBehaviour
{
    [SerializeField]
    private HalfDiceController my_half_dice_con;
    [SerializeField]
    private int id;
    [SerializeField]
    private GameObject wallAnchorPt;
    [SerializeField]
    private float rotateTimerMax;
    private float curRotateTimer;
    private bool isInZone;
    [SerializeField][Range(0.0f, 5.0f)]
    private float speed;
    private float LerpTime;

    private void Update() {
        LerpTime = Time.time - Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            other.transform.position = Vector3.Lerp(other.transform.position, wallAnchorPt.transform.position, (LerpTime / speed));
            my_half_dice_con.spin(id);
        }
    }

    
}
