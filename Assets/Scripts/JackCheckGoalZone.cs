using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackCheckGoalZone : MonoBehaviour
{
    private bool inGoalZone = false;
    [SerializeField]

        //Debug.DrawRay(topChainPt.transform.position, topChainPt.transform.TransformDirection(Vector3.up) * 1000, Color.black);
        //Debug.DrawRay(botChainPt.transform.position, botChainPt.transform.TransformDirection(Vector3.down) * 1000, Color.black);
        //Debug.DrawRay(leftChainPt.transform.position, leftChainPt.transform.TransformDirection(Vector3.back) * 1000, Color.black);
        //Debug.DrawRay(rightChainPt.transform.position, rightChainPt.transform.TransformDirection(Vector3.forward) * 1000, Color.black);


    private void OnTriggerEnter(Collider other) {
        if(other.tag == "GoalZone"){
            inGoalZone = true;
            
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "GoalZone"){
            inGoalZone = false;
            
        }
    }

    public bool getIfInGoalZone(){
        bool status = inGoalZone;
        inGoalZone = false;
        return status;
    }
}
