using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HalfDiceController : MonoBehaviour
{

    private Transform my_trans;
    public GameObject Player;
    private float target = 270;
    [SerializeField]
    private float smoothing;
    private bool spinning = false;
    [SerializeField]
    private GameObject[] weightedZones;
    [SerializeField]
    private Animator[] goalIndicators;
    [SerializeField]
    private ConsumableMovements my_con_mov;
    [SerializeField]
    private DiceTurnController diceTurnController = null;
    [SerializeField]
    private TMP_Text scoreText = null;
    [SerializeField]
    private int scorePerDie;
    private int totalScore = 0;

    // Start is called before the first frame update
    void Start()
    {

        weightedZones[3].SetActive(true);
        goalIndicators[3].SetBool("ShouldBeOn", true);
        my_trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spinning){
            Quaternion targetRotation = Quaternion.Euler(0, target, 270);
            my_trans.rotation = Quaternion.RotateTowards(my_trans.rotation, targetRotation, Time.deltaTime * smoothing);
        }
    }

    //rotation references per zone
    //y = 0 -> weightedZone_3
    //y = 90 -> weightedZone_4
    //y = 180 -> weightedZone_0
    //y = 270 -> weightedZone_1/2

    public void spin(int id){
        //Player.transform.SetParent(my_trans);
        Player.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        spinning = true;
        totalScore += my_con_mov.getNumDiceRemaining() * scorePerDie;
        scoreText.text = "Score: " + totalScore.ToString();
        diceTurnController.OnLevelWon();
        //Debug.Log("Score So Far:" + totalScore);

        int newZone = Random.Range(0, 4);
        if (newZone == id){
            newZone += 1;
            if (newZone >=5){
                newZone = 0;
            }
        }

        switch(id){
            case 0:
                target = 180;
                weightedZones[id].SetActive(false);
                goalIndicators[id].SetBool("ShouldBeOn", false);
                weightedZones[newZone].SetActive(true);
                goalIndicators[newZone].SetBool("ShouldBeOn", true);
                break;
            case 1:
                target = 270;
                weightedZones[id].SetActive(false);
                goalIndicators[id].SetBool("ShouldBeOn", false);
                weightedZones[newZone].SetActive(true);
                goalIndicators[newZone].SetBool("ShouldBeOn", true);
                break;
            case 2:
                target = 270;
                weightedZones[id].SetActive(false);
                goalIndicators[id].SetBool("ShouldBeOn", false);
                weightedZones[newZone].SetActive(true);
                goalIndicators[newZone].SetBool("ShouldBeOn", true);
                break;
            case 3:
                target = 0;
                weightedZones[id].SetActive(false);
                goalIndicators[id].SetBool("ShouldBeOn", false);
                weightedZones[newZone].SetActive(true);
                goalIndicators[newZone].SetBool("ShouldBeOn", true);
                break;
            case 4:
                target = 90;
                weightedZones[id].SetActive(false);
                goalIndicators[id].SetBool("ShouldBeOn", false);
                weightedZones[newZone].SetActive(true);
                goalIndicators[newZone].SetBool("ShouldBeOn", true);
                break;
            default:
                break;

        }

    }

    public int getTotalScore(){
        return totalScore;
    }




}
