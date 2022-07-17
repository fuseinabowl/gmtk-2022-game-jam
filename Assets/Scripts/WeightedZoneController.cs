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
    [SerializeField]
    private LivesCounter my_life_ctr;

    private void FixedUpdate() {
        if(isInZone){
            curRotateTimer -= Time.deltaTime;
            if (curRotateTimer <= 0){
                my_life_ctr.canSafelyReroll();
                my_half_dice_con.spin(id);
            }
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            if(!isInZone){
                curRotateTimer = rotateTimerMax;
            }
            isInZone = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player"){
            isInZone = false;
            curRotateTimer = rotateTimerMax;
        }
    }
}
