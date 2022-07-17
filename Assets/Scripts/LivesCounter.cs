using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesCounter : MonoBehaviour
{
    
    [SerializeField]
    private int lifeCounterMax = 3;

    private int curLives;
    [SerializeField]
    private GameObject[] lifeObjects;
    private void Start() {
        curLives = lifeCounterMax;
    }

    public bool decrementLife(){
        curLives -= 1;
        TryHideLifeObject();

        // if it returns false, that means GAME OVER
        return (curLives >= 0);
    }

    private void TryHideLifeObject()
    {
        if (curLives >= 0 && curLives < lifeObjects.Length)
        {
            var lifeObject = lifeObjects[curLives];
            if (lifeObject != null)
            {
                lifeObject.SetActive(false);
            }
        }
    }
}
