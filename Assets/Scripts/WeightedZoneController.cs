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
    private float rotateTimerMax;
    private float curRotateTimer;
    private bool isInZone;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            my_half_dice_con.spin(id);
        }
    }

    
}
