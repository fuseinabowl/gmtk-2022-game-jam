using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesCounter : MonoBehaviour
{
    
    private int lifeCounterMax = 3;
    private bool safeReroll = false;
    private int curLives;
    [SerializeField]
    private GameObject[] lifeObjects;
    private void Start() {
        curLives = lifeCounterMax;
    }

    public void canSafelyReroll(){
        safeReroll = true;
    }

    public bool decrementLife(){
        if (!safeReroll){
            curLives -= 1;
            lifeObjects[curLives].SetActive(false);
        }else{
            safeReroll = false;
        }
        

        //if it returns false, that means GAME OVER
        return (curLives > 0);
        }

    
    


}
