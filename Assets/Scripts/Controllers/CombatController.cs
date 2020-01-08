using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    //Profiles
    public CombatProfile_SO combat;

    //Sequencers
    IEnumerator AimSequencer;


    private void Update()
    {
        CheckUnitSelectInput();
    }

    void CheckUnitSelectInput()
    {
        if (Input.GetMouseButtonDown(0) && Input.touchCount == 1)
        {
            //Shoot a ray on the selectable layer and if it finds a selectable object, start an aim routine with it
            int selectableLayermask = 1 << 8;
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 1000f, selectableLayermask))
            {
                //Get the selectable unit and if it's in the player's team, continue to aim
                Unit unit = hitInfo.collider.gameObject.GetComponent<Unit>();
                if (unit.combatState != Unit.UnitState.stunned && unit.teamIndex == combat.playerTeamIndex)
                {
                    if (AimSequencer != null) StopCoroutine(AimSequencer);
                    AimSequencer = AimSequence(unit);
                    StartCoroutine(AimSequencer);
                }
            }
        }

    }

    IEnumerator AimSequence(Unit unit)
    {
        //While they're holding
        while (Input.GetMouseButton(0) && Input.touchCount == 1)
        {
            //Face unit toward opposite of mouse position in world
            unit.AimToward((-(getMousePointInWorld() - unit.transform.position))*100f);
            yield return null;
        }
        //and Launch
        unit.Launch();
    }

    private Vector3 getMousePointInWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            return cameraRay.GetPoint(rayLength);
        }
        Debug.Log("Error fetching mouse world point, ray not hitting plane?");
        return Vector3.zero;
    }


}
