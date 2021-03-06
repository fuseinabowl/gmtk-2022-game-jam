using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeightController : MonoBehaviour
{
    private Rigidbody my_rigid;
    private Transform my_trans;

    [SerializeField][Range(0.0f, 1.0f)]
    public float arrowheadSize;
    Vector3 startPosition, mouseArea;
    public GameObject arrow;
    
    LineRenderer arrowLine;
    
    [SerializeField] [Range(0f, 1.0f)]
    private float preFlickLinearSpeedMultiplier = 1f;
    [SerializeField][Range(1, 50)]
    private float speed;
    private int prevAngle;

    private float stopTimerMax = 1.0f;
    private float curStopTimer = 0.0f;
    
    [SerializeField] [Range(0.1f, 1.0f)]
    [FormerlySerializedAs("slowdownSpeed")]
    private float clickTimeSpeedMultiplier = 1f;
    [SerializeField] [Range(0f, 1.0f)]
    private float clickLinearSpeedMultiplier = 1f;

    private bool stopped = false;

    [SerializeField]
    private float gravity = 9.8f;

    [SerializeField]
    private DiceTurnController my_die_turn_con;

    [SerializeField]
    private ConsumableMovements my_con_movements;

    private RigidbodyConstraints cachedLiveConstraints = RigidbodyConstraints.None;
    
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private JackCheckGoalZone goal_zone_check;
    private bool chained = false;
    [SerializeField][Range(0.0f, 1.0f)]
    private float chainWidth;
    //private float chainTimerMax = 1.66f;
    private float curChainTimer;
    [SerializeField]
    private LineRenderer chainLine;
    [SerializeField]
    private GameObject topChainPt;
    [SerializeField]
    private GameObject botChainPt;
    [SerializeField]
    private GameObject leftChainPt;
    [SerializeField]
    private GameObject rightChainPt;
    private float elapsedTime;
    private (GameObject chainStartObject, Vector3 chainEndPos) closestPointsPair;
    [SerializeField]
    private LivesCounter my_lives_ctr;
    [SerializeField]
    private float minMagnitude;

    
    [SerializeField]
    private Canvas jackProtractorCanvas = null;
    [SerializeField]
    private Transform jackProtactorTransform = null;
    [SerializeField]
    private JackProtractorController jackProtactorController = null;
    [SerializeField]
    private float Zpopup;
    
    // Start is called before the first frame update
    void Start()
    {
        
        my_rigid = Player.GetComponent<Rigidbody>();
        my_trans = Player.GetComponent<Transform>();

        arrowLine = arrow.GetComponentInChildren<LineRenderer>();
        mouseArea = new Vector3(); 

        cachedLiveConstraints = my_rigid.constraints;
    }


    void FixedUpdate() {
        
        if(chained){
           
            chainLine.enabled = true;
            DrawChain(closestPointsPair);
        }
        if (!stopped){
            my_rigid.AddForce(new Vector3(0.0f, 0.0f, -gravity), ForceMode.Acceleration);
        }
        
    }
    

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(curChainTimer > 0){
            curChainTimer -= Time.deltaTime;
        }
        if (curChainTimer <=0){
            chainLine.enabled = false;
            chained = false;
        }

        if(stopped){
            curStopTimer -= Time.deltaTime;
        }
        
        if (curStopTimer <= 0){
            ReleaseStop();
        }
        /*
        if(Input.GetKeyDown("space")){
           
            var myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Stop);
            bool inGoalZone = goal_zone_check.getIfInGoalZone();
            

            if(inGoalZone == true){
                Stop();
                closestPointsPair = getClosestPoint(); 
                curChainTimer = chainTimerMax;
                chained = true;
                

            }else if (myMovements > 0 ){
                Stop();
                curChainTimer = chainTimerMax;
                closestPointsPair = getClosestPoint(); 
                chained = true;
                
                //Debug.Log("stop action used");
            }
        }*/
    }

    private void Stop()
    {
        my_rigid.velocity = Vector3.Lerp(my_rigid.velocity, new Vector3(0.0f, 0.0f, 0.0f), (elapsedTime));
        my_rigid.angularVelocity = Vector3.Lerp(my_rigid.angularVelocity, new Vector3(0.0f, 0.0f, 0.0f), (elapsedTime ));

        my_rigid.constraints = RigidbodyConstraints.FreezeAll;

        curStopTimer = stopTimerMax;
        stopped = true;
    }

    private void ReleaseStop()
    {
        my_rigid.constraints = cachedLiveConstraints;

        stopped = false;
    }

    private void OnMouseDown() {
        if(my_con_movements.getIfReadyToShare()){
            var myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Up);
            //my_rigid.AddForce(new Vector3(10.0f, 0.0f, 10.0f), ForceMode.Impulse);
            mouseArea = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
            );
            my_rigid.velocity = my_rigid.velocity * clickLinearSpeedMultiplier;
            startPosition = mouseArea;
            Time.timeScale = clickTimeSpeedMultiplier;

            jackProtractorCanvas.enabled = true;
            jackProtactorTransform.position = startPosition;
        }
    }

    private void OnMouseDrag() {
        if(my_con_movements.getIfReadyToShare()){
            arrowLine.enabled = true;
            DrawArrow();

            int newAngle = (int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up);
            int myWilds = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Stop);
            bool hasWilds = myWilds > 0;
            
            if (newAngle <= -165 || newAngle >= -15){
                if(newAngle >= 144 || newAngle <= -165){
                    int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Left);
                    jackProtactorController.SetMovesToBeUsed(true, ConsumableMovements.Movement.Left, hasWilds && myMovements == 0);
                }else if(newAngle >= 108){
                    int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.LeftUp);
                    jackProtactorController.SetMovesToBeUsed(true, ConsumableMovements.Movement.LeftUp, hasWilds && myMovements == 0);
                }else if(newAngle >= 72){
                    int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Up);
                    jackProtactorController.SetMovesToBeUsed(true, ConsumableMovements.Movement.Up, hasWilds && myMovements == 0);
                }else if(newAngle >= 36){
                    int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.RightUp);
                    jackProtactorController.SetMovesToBeUsed(true, ConsumableMovements.Movement.RightUp, hasWilds && myMovements == 0);
                }else if(newAngle >= -15){
                    int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Right);
                    jackProtactorController.SetMovesToBeUsed(true, ConsumableMovements.Movement.Right, hasWilds && myMovements == 0);
                }else{
                    jackProtactorController.SetMovesToBeUsed(false, ConsumableMovements.Movement.Stop, false);
                }                
            }else{
                jackProtactorController.SetMovesToBeUsed(false, ConsumableMovements.Movement.Stop, false);
            }
        }
        
        
    }

    private void DrawChain((GameObject ChainStartPt, Vector3 ChainEndPos) chainPosTuple){
        chainLine.SetPosition(0, chainPosTuple.ChainStartPt.transform.position);
        chainLine.SetPosition(1, chainPosTuple.ChainEndPos);
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
        
    }

    private void OnMouseUp() {
        Time.timeScale = 1.0f;
        if(my_con_movements.getIfReadyToShare()){
           
            jackProtractorCanvas.enabled = false;
            arrowLine.enabled = false;
            jackProtactorController.SetMovesToBeUsed(false, ConsumableMovements.Movement.Stop, false);
            ReleaseStop();
            Vector3 magnitude = mouseArea - startPosition;
            int newAngle = (int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up);
            int myWilds = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Stop);            
            if(magnitude.magnitude >= minMagnitude){

                if (newAngle <= -165 || newAngle >= -15){
                    if(newAngle >= 144 || newAngle <= -165){
                        int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Left);
                        if( myMovements > 0){
                            Flick(new Vector3(magnitude.x, magnitude.y, Mathf.Max(magnitude.z, Zpopup)) );
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Left);
                        }else if(myWilds > 0){
                            Flick(new Vector3(magnitude.x, magnitude.y, Mathf.Max(magnitude.z, Zpopup)));
                        
                            
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Stop);
                        }
                    }else if(newAngle >= 108){
                        int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.LeftUp);
                        if( myMovements > 0){
                            Flick(magnitude);
                        
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.LeftUp);
                        }else if(myWilds > 0){
                            Flick(magnitude);
                            
                            
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Stop);
                        }
                    }else if(newAngle >= 72){
                        int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Up);
                        if( myMovements > 0){
                            Flick(magnitude);
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Up);
                        }else if(myWilds > 0){
                            Flick(magnitude);
                           
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Stop);
                        }
                    }else if(newAngle >= 36){
                        int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.RightUp);
                        if( myMovements > 0){
                            Flick(magnitude);
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.RightUp);
                        }else if(myWilds > 0){
                            Flick(magnitude);
                            
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Stop);
                        }
                    }else if(newAngle >= -15){
                        int myMovements = my_con_movements.GetAvailableMovementActions(ConsumableMovements.Movement.Right);
                        if( myMovements > 0){
                            Flick(new Vector3(magnitude.x, magnitude.y, Mathf.Max(magnitude.z, Zpopup)));
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Right);
                        }else if(myWilds > 0){
                            Flick(new Vector3(magnitude.x, magnitude.y, Mathf.Max(magnitude.z, Zpopup)));
                            
                            my_con_movements.ConsumeMovement(ConsumableMovements.Movement.Stop);
                        }
                    }
                    
                } 
            }
        }
        
        
    }

    private void Flick(Vector3 flickDirection)
    {
        my_rigid.velocity = my_rigid.velocity * preFlickLinearSpeedMultiplier;

        var flickForce = flickDirection * speed;
        my_rigid.AddForce(flickForce, ForceMode.Impulse);
    }
    

    private (GameObject, Vector3) getClosestPoint(){
        var smallestDist = Mathf.Infinity;
        GameObject closestChainPt = null;
        Vector3 closestRayHit = new Vector3(0f,0f,0f); // dummy value watch out if there are issues
        RaycastHit topHit;
        RaycastHit botHit;
        RaycastHit rightHit;
        RaycastHit leftHit;
        if (Physics.Raycast(topChainPt.transform.position, topChainPt.transform.TransformDirection(Vector3.up), out topHit, Mathf.Infinity)){
            Debug.Log("topHit dist: "+ topHit.distance);
            if (topHit.distance < smallestDist){
                smallestDist = topHit.distance;
                closestRayHit = topHit.point;
                closestChainPt = topChainPt;
            }
        }
        
        if (Physics.Raycast(botChainPt.transform.position, botChainPt.transform.TransformDirection(Vector3.down), out botHit, Mathf.Infinity)){
            Debug.Log("botHit dist: "+ botHit.distance);
            if (botHit.distance < smallestDist){
                smallestDist = botHit.distance;
                closestRayHit = botHit.point;
                closestChainPt = botChainPt;
            }
        }
        if (Physics.Raycast(leftChainPt.transform.position, leftChainPt.transform.TransformDirection(Vector3.back), out leftHit, Mathf.Infinity)){
            Debug.Log("leftHit dist: "+ leftHit.distance);
            if (leftHit.distance < smallestDist){
                smallestDist = leftHit.distance;
                closestRayHit = leftHit.point;
                closestChainPt = leftChainPt;
            }
        }
        if (Physics.Raycast(rightChainPt.transform.position, rightChainPt.transform.TransformDirection(Vector3.forward), out rightHit, Mathf.Infinity)){
            Debug.Log("rightHit dist: "+ rightHit.distance);
            if (rightHit.distance < smallestDist){
                smallestDist = rightHit.distance;
                closestRayHit = rightHit.point;
                closestChainPt = rightChainPt;
            }
        }
            
        return (closestChainPt, closestRayHit);
    }

}
