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
    private WeightController my_weight_con;
    [SerializeField]
    private DiceTurnController diceTurnController = null;
    [SerializeField]
    private TMP_Text scoreText = null;    
    [SerializeField]
    private TMP_Text finalScoreText = null;
    [SerializeField]
    private int scorePerDie;
    private int totalScore = 0;

    [SerializeField]
    private string[] responses = new string[]{
        "A one would be DEVASTATING for you now...", // Rolling a 1
        "A five would be quite a lucky roll for me...", // Rolling a 5
        "If only I could roll a five right now...", // Rolling a 5
        "I couldn't POSSIBLY roll a six...", // Rolling a 6
        "Rolling a two would be so bad for you..." // Rolling a 2
    };
        
        
    private int faceId;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 5; i++){
            weightedZones[i].SetActive(false);
            goalIndicators[i].SetBool("ShouldBeOn", false);
        }

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
        faceId = id;
        Player.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        spinning = true;
        totalScore += my_con_mov.getNumDiceRemaining() * scorePerDie;
        scoreText.text = "Score: " + totalScore.ToString();
        finalScoreText.text = "You helped the demon scam "  + totalScore.ToString() + " coins from other gamblers.";
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

    public string getCurResponse(){
        return responses[faceId];
    }

}
