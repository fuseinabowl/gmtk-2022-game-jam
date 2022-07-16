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
    
    [SerializeField] [Range(0.1f, 1.0f)]
    private float slowdownSpeed;
    private bool stopped = false;



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
        if(Input.GetKeyDown("space")){
            Debug.Log("stop action used");
            my_rigid.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            stopped = true;
        }
    }



    private void OnMouseDown() {
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

        Debug.Log("Angle: " + (int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up));
       

    }

    private void OnMouseUp() {
        Time.timeScale = 1.0f;
        arrowLine.enabled = false;
        stopped = false;
        Vector3 magnitude = mouseArea - startPosition;
        int newAngle = (int) Vector3.SignedAngle((mouseArea - startPosition), Vector3.right, Vector3.up);
        
        if (newAngle >= 0){
            if(newAngle >= 144){
                Debug.Log("Leftmost Action used!");
                my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
            }else if(newAngle >= 108){
                Debug.Log("Up-Left Action Used!");
                my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
            }else if(newAngle >= 72){
                Debug.Log("Up Action Used!");
                my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
            }else if(newAngle >= 36){
                Debug.Log("Up-right Action Used!");
                my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
            }else if(newAngle >= 0){
                Debug.Log("Right Action Used!");
                my_rigid.AddForce(new Vector3(magnitude.x*speed, 0.0f, magnitude.z*speed), ForceMode.Impulse);
            }
            
        }
        Debug.Log(newAngle);
        
    }
}
