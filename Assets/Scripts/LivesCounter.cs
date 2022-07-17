using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesCounter : MonoBehaviour
{

    private GameObject[] livesAr;
    
    private int lifeCounterMax = 3;
    private int curLives;
    [SerializeField]
    private GameObject[] lifeObjects;
    private void Start() {
        curLives = lifeCounterMax;
    }

    public bool decrementLife(){
        curLives -= 1;
        lifeObjects[curLives].SetActive(false);

        //if it returns false, that means GAME OVER
        return (curLives > 0);
        }

    
    


}
