using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    //Profiles
    public CombatProfile_SO combat;
    public static CombatController combatController;

    //Sequencers
    IEnumerator AimSequencer;

    private void Awake()
    {
        combatController = this;
    }

    private void Update()
    {
        if(GameController.gamePlayable)CheckUnitSelectInput();
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
        float launchPower = 0f;

        //While they're holding
        while (Input.GetMouseButton(0) && Input.touchCount == 1)
        {
            //Get the potential power of the launch of the player released the aim at this moment
            launchPower = GetLaunchPower(unit);
            Debug.Log("Current launch power is " + launchPower);

            //Face unit toward opposite of mouse position in world
            unit.AimToward((-(getMousePointInWorld() - unit.transform.position))*100f);
            yield return null;
        }
        //and Launch
        unit.Launch(launchPower);
    }

    public float GetLaunchPower(Unit unit)
    {
        //what was drawn
        float pwr = Mathf.Clamp(Vector3.Distance(getMousePointInWorld(), unit.transform.position) / combat.maxPowerRange, 0f, 1f);
        //What it can be with the selected unit's stamina
        return Mathf.Clamp(pwr, 0f, unit.stamina.currentValue / (unit.attackDamageBase * pwr));
    }

    private Vector3 getMousePointInWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, GameController.gameController.level.levelYOffset);
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            return cameraRay.GetPoint(rayLength);
        }
        Debug.Log("Error fetching mouse world point, ray not hitting plane?");
        return Vector3.zero;
    }


}
