using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightController : MonoBehaviour
{
    private Rigidbody my_rigid;
    private Transform my_trans;

    [SerializeField][Range(0.0f, 1.0f)]
    public float arrowheadSize;
    Vector3 startPosition, mouseArea;
    public GameObject arrow;
    
    LineRenderer arrowLine;
    
    [SerializeField][Range(1, 50)]
    private float speed;
    private int prevAngle;

    private float stopTimerMax = 1.0f;
    private float curStopTimer = 0.0f;
    
    [SerializeField] [Range(0.1f, 1.0f)]
    private float slowdownSpeed;
    private bool stopped = false;

    [SerializeField]
    private DiceTurnController my_die_turn_con;

    [SerializeField]
    private ConsumableMovements my_con_movements;

    private bool weighingDown = false;



    // Start is called before the first frame update
    void Start()
    {
        my_rigid = GetComponent<Rigidbody>();
        my_trans = GetComponent<Transform>();

        arrowLine = arrow.GetComponentInChildren<LineRenderer>();
        mouseArea = new Vector3(); 
    }


    void FixedUpdate() {
        if (!stopped){
            my_rigid.AddForce(new Vector3(0.0f, 0.0f, -9.8f), ForceMode.Acceleration);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(stopped){
            curStopTimer -= Time.deltaTime;
        }
        
        if (curStopTimer <= 0){
            stopped = false;
            curStopTimer = stopTimerMax;
        }
        if(Input.GetKeyDown("space")){
            var myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Stop);
            if (myMovements > 0){
                my_rigid.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                stopped = true;
                my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Stop);
                Debug.Log("stop action used");
            }else{
                Debug.Log("No Action to use!");
            }
            
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "GoalZone"){
            Debug.Log("Reached a zone!");
            weighingDown = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "GoalZone"){
            Debug.Log("Reached a zone!");
            weighingDown = false;
        }
    }

    private void OnMouseDown() {
        var myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Up);
       //my_rigid.AddForce(new Vector3(10.0f, 0.0f, 10.0f), ForceMode.Impulse);
        mouseArea = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
        );
        my_rigid.velocity = new Vector3(my_rigid.velocity.x/2, my_rigid.velocity.y/2, my_rigid.velocity.z/2);
        startPosition = mouseArea;
        Time.timeScale = slowdownSpeed;

    }

    private void OnMouseDrag() {
        arrowLine.enabled = true;
        DrawArrow();
        //Debug.Log(startPosition.x);
        //Debug.Log(startPosition.y);
        
    }

    private void DrawArrow(){
        mouseArea = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane
            )
        );
        float percentSize = (float) (arrowheadSize / Vector3.Distance (startPosition, mouseArea));
        //Debug.Log(Vector3.Distance (startPosition, mouseArea));
        arrowLine.SetPosition(0, startPosition);
        arrowLine.SetPosition(1, Vector3.Lerp(startPosition, mouseArea, 0.999f - percentSize));
        arrowLine.SetPosition(2, Vector3.Lerp(startPosition, mouseArea, 1 - percentSize));
        arrowLine.SetPosition(3, mouseArea);
        arrowLine.widthCurve = new AnimationCurve (
            new Keyframe(0, 0.4f),
            new Keyframe (0.999f - percentSize, 0.4f),
            new Keyframe (1 - percentSize, 1f),
            new Keyframe (1 - percentSize, 1f),
            new Keyframe (1, 0f)
        );      
        Debug.Log((int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up));
        prevAngle = (int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up);
    }

    private void OnMouseUp() {
        Time.timeScale = 1.0f;
        arrowLine.enabled = false;
        stopped = false;
        Vector3 magnitude = mouseArea - startPosition;
        int newAngle = (int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up);
        
        if (newAngle <= -165 || newAngle >= -15){
            if(newAngle >= 144 || newAngle <= -165){
                int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Left);
                if( myMovements > 0){
                    my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
                    Debug.Log("Leftmost Action used!");
                    my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Left);
                }
            }else if(newAngle >= 108){
                int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.LeftUp);
                if( myMovements > 0){
                    my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
                    Debug.Log("LeftUp Action used!");
                    my_con_movements.ConsumeMovement(ConsumableMovements.Movement.LeftUp);
                }
            }else if(newAngle >= 72){
                int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Up);
                if( myMovements > 0){
                    my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
                    Debug.Log("LeftUp Action used!");
                    my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Up);
                }
            }else if(newAngle >= 36){
                int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.RightUp);
                if( myMovements > 0){
                    my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
                    Debug.Log("Right Action used!");
                    my_con_movements.ConsumeMovement(ConsumableMovements.Movement.RightUp);
                }
            }else if(newAngle >= -15){
                int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Right);
                if( myMovements > 0){
                    my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
                    Debug.Log("Right Action used!");
                    my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Right);
                }
            }
            
        }
        
    }
}
