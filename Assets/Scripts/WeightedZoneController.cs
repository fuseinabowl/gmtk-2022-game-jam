using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedZoneController : MonoBehaviour
{
    [SerializeField]
    private HalfDiceController my_half_dice_con;
    [SerializeField]
    private int id;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            my_half_dice_con.spin(id);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player"){
            Debug.Log("Player exited Zone");
        }
    }
}
