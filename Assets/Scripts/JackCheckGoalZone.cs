using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackCheckGoalZone : MonoBehaviour
{
    private bool inGoalZone = false;

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
        return inGoalZone;
    }
}
