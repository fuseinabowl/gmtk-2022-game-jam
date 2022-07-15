using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightController : MonoBehaviour
{
    private Rigidbody my_rigid;
    public float arrowheadSize;
    Vector3 startPosition, mouseArea;
    GameObject arrow;
    LineRenderer arrowLine;



    // Start is called before the first frame update
    void Start()
    {
        my_rigid = GetComponent<Rigidbody>();
        arrow = GameObject.FindGameObjectWithTag("Arrow");
        arrowLine = arrow.GetComponentInChildren<LineRenderer>();
        mouseArea = new Vector3();
        arrowheadSize = 0.02f;
    }


    void FixedUpdate() {
        my_rigid.AddForce(new Vector3(0.0f, 0.0f, -9.8f), ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnMouseDown() {
       //my_rigid.AddForce(new Vector3(10.0f, 0.0f, 10.0f), ForceMode.Impulse);
        mouseArea = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
        );
        startPosition = mouseArea;

    }

    private void OnMouseDrag() {
        arrowLine.enabled = true;
        DrawArrow();
    }

    private void DrawArrow(){
        mouseArea = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane
            )
        );
        float percentSize = (float) (arrowheadSize / Vector3.Distance (startPosition, mouseArea));
        arrowLine.SetPosition(0, startPosition);
        arrowLine.SetPosition(1, Vector3.Lerp(startPosition, mouseArea, 0.999f - percentSize));
        arrowLine.SetPosition(2, Vector3.Lerp(startPosition, mouseArea, 1 - percentSize));
    }
}
