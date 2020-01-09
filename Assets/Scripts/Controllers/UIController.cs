﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    //Inventory UI
    public static UIController uiController;
    public Transform[] inventoryUI;
    public Transform itemPanel;

    //Status UI
    [SerializeField] protected List<GameObject> statusUIs = new List<GameObject>();
    [SerializeField] protected GameObject statusUIPrefab;
    [SerializeField] protected Transform statusPanel;


    IEnumerator dragSequencer;

    private void Awake()
    {
        uiController = this;
    }

    #region Inventory UI

    public void StartMovePowerUp(Transform powerUpToMove)
    {
        if(dragSequencer == null && GameController.gamePlayable)
        {
            dragSequencer = DragSequence(powerUpToMove);
            StartCoroutine(dragSequencer);
        }
    }

    public void StopMovePowerUp()
    {
        StopCoroutine(dragSequencer);
        dragSequencer = null;
    }

    IEnumerator DragSequence(Transform powerUpToMove)
    {
        //Set starting state
        GameController.gameNavicable = false;
        GameController.unitsSelectable = false;
        Vector3 origin = powerUpToMove.position;

        //Do while Dragging
        while (Input.touchCount == 1)
        {
            powerUpToMove.position = Input.mousePosition;
            Debug.Log(Input.touchCount);
            yield return null;
        }

        //Shoot a ray on the selectable layer and if it finds a selectable object, apply the powerup
        int selectableLayermask = 1 << 8;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 1000f, selectableLayermask))
        {
            if(hitInfo.collider.gameObject.GetComponent<Unit>() != null)
            {
                powerUpToMove.gameObject.GetComponent<PowerUp>().PowerUpUnit(hitInfo.collider.gameObject.GetComponent<Unit>());
            }
        }

        //Reset state
        GameController.gameNavicable = true;
        GameController.unitsSelectable = true;
        powerUpToMove.position = origin;
        StopMovePowerUp();
    }


    public void UpdateInventoryUI()
    {
        //Refresh the position of the inventory items on the UI
        for( int i = 0; i < GameController.gameController.allyPowerUps.Count; i++)
        {
            GameController.gameController.allyPowerUps[i].transform.position = inventoryUI[i].position;
        }
    }

    #endregion

    #region Status UI

    public void GenerateStatusUI()
    {
        foreach (GameObject unit in GameController.gameController.activeUnits)
        {
            GameObject statusUI = Instantiate(statusUIPrefab, statusPanel);
            statusUI.GetComponent<StatusUI>().unitToTrack = unit.transform;
            statusUIs.Add(statusUI);
        }
    }

    public void RemoveStatusUI(StatusUI statusUIToRemove)
    {
        statusUIs.Remove(statusUIToRemove.gameObject);
        Destroy(statusUIToRemove.gameObject);
    }

    public void ChangeStatusScaleUIToMatchZoom()
    {
        float startZoom = CameraController.cameraController.startZoom;
        float currentZoom = Camera.main.orthographicSize;

        foreach(GameObject statusUI in statusUIs)
        {
            statusUI.transform.localScale = Vector3.one * (startZoom / currentZoom);
        }
    }

    #endregion 

}