using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    IEnumerator TouchChecker;
    IEnumerator TouchDeltasCalculator;

    Vector2 dragVector;
    float touchDelta = 0f;
    Vector2 previousPos;
    Vector2 currentPos;
    float pinchDelta = 0f;
    float previousPinch = 0f;
    float currentPinch = 0f;
    [SerializeField] protected Vector2 pinchStartStopTriggers = new Vector2(3f, 20f);
    [SerializeField] protected Vector2 touchDeltaStartStayTrigger = new Vector2(20f, 60f);
    Vector2 touchPosition = Vector2.zero;

    public enum MoveStates {zooming, scrolling};
    public MoveStates cameraMoveState = MoveStates.scrolling;


    protected Camera cam;
    [SerializeField] protected Vector2 zoomLimits = new Vector2(10f, 15f);
    [SerializeField] protected float zoomSpeed = 1f;
    [SerializeField] protected float scrollSpeed = 1f;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        GetTouchInput();
    }

    private void GetTouchInput()
    {
        //If an input from touch is found, start a coroutine to check that the player using 2 fingers every frame
        if (Input.GetMouseButtonDown(0) && TouchChecker == null)
        {
            TouchChecker = CheckTouchUpdate();
            StartCoroutine(TouchChecker);
        }
    }

    IEnumerator CheckTouchUpdate()
    {
        //Get touch start positions
        Vector3 startPivotPosition = transform.position;
        Vector2 startTouchPivotPosition = Vector2.zero;

        startTouchPivotPosition = GetMeanPositionFromTouches();
        touchPosition = startTouchPivotPosition;

        //Start TouchDelta Calculations
        if (TouchDeltasCalculator == null) TouchDeltasCalculator = CalculateTouchDeltas();
        StartCoroutine(TouchDeltasCalculator);

        //CheckTouch Updater
        while (Input.touchCount > 0)
        {
            //Calculate touch mean position
            touchPosition = GetMeanPositionFromTouches();


            //If the player is using 2 fingers, calculate the change in touch if any
            if (Input.touchCount == 2)
            {
                //Calculate touch drag angle from mean position and pivot position
                dragVector = startTouchPivotPosition - touchPosition;
                dragVector *= 0.015f * scrollSpeed;

                if (cameraMoveState == MoveStates.zooming)
                {
                    //Check if it's still zooming, if it is, change the zoom
                    if (Mathf.Abs(pinchDelta) < pinchStartStopTriggers.x && touchDelta > touchDeltaStartStayTrigger.x)
                    {
                        cameraMoveState = MoveStates.scrolling;
                        startTouchPivotPosition = GetMeanPositionFromTouches();
                        startPivotPosition = transform.position;
                        dragVector = Vector2.zero;
                    }
                    else cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + -pinchDelta * 0.010f * zoomSpeed, zoomLimits.x, zoomLimits.y);
                }
                else if(cameraMoveState == MoveStates.scrolling)
                {
                    //Check if it's still scrolling, if it is, change the camera transform
                    if (Mathf.Abs(pinchDelta) > pinchStartStopTriggers.y && touchDelta < touchDeltaStartStayTrigger.y) cameraMoveState = MoveStates.zooming;
                    else transform.position = startPivotPosition + new Vector3(dragVector.x, 0f, dragVector.y);
                }
            }
            yield return null;
        }

        ResetTouchCheckers();
        yield return null;
    }

    IEnumerator CalculateTouchDeltas()
    {
        currentPos = touchPosition;
        previousPos = touchPosition;

        bool holdPinch = false;

        while (true)
        {
            //Calculate Touch Mean Position Delta
            currentPos = touchPosition;
            touchDelta = Mathf.Abs(Vector2.Distance(previousPos, currentPos));
            previousPos = currentPos;

            //Calculate Touch Pinch Mean
            if (Input.touchCount >= 2)
            {
                currentPinch = 0f;
                foreach (Touch touch in Input.touches) currentPinch += Mathf.Abs(Vector2.Distance(touch.position, touchPosition));
                currentPinch /= Input.touches.Length;
                if (!holdPinch) pinchDelta = 0f; else pinchDelta = currentPinch - previousPinch;
                previousPinch = currentPinch;

                holdPinch = true;
            }
            else holdPinch = false;

            yield return new WaitForSecondsRealtime(0.05f);
        }


    }

    private Vector2 GetMeanPositionFromTouches()
    {
        Vector2 meanPosition = Vector2.zero;
        foreach (Touch touch in Input.touches)
        {
            meanPosition += touch.position;
        }
        meanPosition /= Input.touches.Length;
        return meanPosition;
    }

    private void ResetTouchCheckers()
    {
        StopCoroutine(TouchChecker);
        TouchChecker = null;
        StopCoroutine(TouchDeltasCalculator);
        TouchDeltasCalculator = null;
    }
    
}
