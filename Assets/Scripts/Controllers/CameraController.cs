using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController cameraController;

    IEnumerator TouchChecker;
    IEnumerator TouchDeltasCalculator;
    IEnumerator CamRotator;

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
    [SerializeField] public Vector2 zoomLimits = new Vector2(15f, 25f);
    [SerializeField] protected float startStopRotationDuration = 1f;
    [SerializeField] public Quaternion startRotation;
    [SerializeField] public Quaternion stopRotation;
    [SerializeField] public float startZoom = 20f;
    [SerializeField] protected float zoomSpeed = 1f;
    [SerializeField] protected float scrollSpeed = 1f;

    private void Awake()
    {
        cam = Camera.main;
        cam.orthographicSize = Mathf.Clamp(startZoom, zoomLimits.x, zoomLimits.y);
        transform.rotation = startRotation;
        cameraController = this;
    }

    private void Update()
    {

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
            //Calculate touch drag angle from mean position and pivot position
            dragVector = startTouchPivotPosition - touchPosition;
            dragVector *= 0.015f * scrollSpeed;

            //Calculate touch mean position
            touchPosition = Input.touches[0].position; //previously: GetMeanPositionFromTouches();
            if(Input.touchCount == 1)
            {
                if(cameraMoveState == MoveStates.zooming)
                {
                    cameraMoveState = MoveStates.scrolling;
                    startTouchPivotPosition = GetMeanPositionFromTouches();
                    startPivotPosition = transform.position;
                    dragVector = Vector2.zero;
                }
                transform.position = startPivotPosition + new Vector3(dragVector.x, 0f, dragVector.y);
            }

            //If the player is using 2 fingers, calculate the change in touch if any
            else if (Input.touchCount == 2)
            {
                if (cameraMoveState == MoveStates.scrolling) cameraMoveState = MoveStates.zooming;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + -pinchDelta * 0.010f * zoomSpeed, zoomLimits.x, zoomLimits.y);
                UIController.uiController.ChangeStatusScaleUIToMatchZoom();
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
    
    public void StartStopRotation(bool state)
    {
        if (CamRotator != null) StopCoroutine(CamRotator);
        CamRotator = StartStopRotationSequence(state);
        StartCoroutine(CamRotator);
    }

    IEnumerator StartStopRotationSequence(bool state)
    {
        //Lerp the X rotation over defined duration

        Quaternion startRot;
        Quaternion stopRot;
        if (state)
        {
            stopRot = stopRotation;
        }
        else
        {
            stopRot = startRotation;
        }
        startRot = transform.rotation;
        float progress = 0.0f;

        while(progress != 1f)
        {
            progress += Time.unscaledDeltaTime * startStopRotationDuration;
            progress = Mathf.Clamp(progress, 0f, 1f);
            transform.rotation = Quaternion.Lerp(startRot, stopRot, progress);//+= Time.deltaTime / startStopRotationDuration;
            yield return null;
        }

        yield return null;
    }

}
