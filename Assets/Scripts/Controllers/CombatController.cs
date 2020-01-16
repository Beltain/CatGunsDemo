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
        if(GameController.unitsSelectable)CheckUnitSelectInput();
    }

    void CheckUnitSelectInput()
    {
        if (Input.GetMouseButtonDown(0) && Input.touchCount > 0)
        {
            if (GetClosestSelectableUnit(combat.playerTeamIndex) != null)
            {
                //Get the closest unit in range and if it's in the player's team, continue to aim
                Unit unit = GetClosestSelectableUnit(combat.playerTeamIndex);
                if (unit.combatState == Unit.UnitState.idle && unit.teamIndex == combat.playerTeamIndex)
                {
                    if (AimSequencer != null) StopCoroutine(AimSequencer);
                    AimSequencer = AimSequence(unit);
                    StartCoroutine(AimSequencer);
                }
            }
            else
            {
                //Set the time scale to normal
                GameController.gameController.SetTimeScale("PlayRegardless");
            }
        }

        if (Input.GetMouseButtonUp(0) && GameController.gameController.gameState == GameController.GameState.Game)
        {
            GameController.gameController.SetTimeScale("Play");
        }

    }

    private Unit GetClosestSelectableUnit(int teamIndex)
    {
        if( Vector3.Distance(GetUnitClosestTo(GetMousePointInWorld(), teamIndex).transform.position, GetMousePointInWorld()) < combat.unitSelectionRadius)
        {
            return GetUnitClosestTo(GetMousePointInWorld(), teamIndex);
        }
        else return null;
    }

    private Unit GetUnitClosestTo(Vector3 position, int teamIndex)
    {
        List<Unit> nearbyUnits = new List<Unit>();
        Unit closestUnit = null;
        foreach(GameObject unit in GameController.gameController.GetAllActiveUnitsOfTeam(teamIndex)) nearbyUnits.Add(unit.GetComponent<Unit>());
        foreach(Unit unit in nearbyUnits)
        {
            if (closestUnit == null)
            {
                closestUnit = unit;
            }
            else if (Vector3.Distance(closestUnit.transform.position, position) > Vector3.Distance(unit.gameObject.transform.position, position))
            {
                closestUnit = unit;
            }
        }
        return closestUnit;
    }

    IEnumerator AimSequence(Unit unit)
    {
        //Stop camera movement
        GameController.gameNavicable = false;
        //Draw aim reticules
        unit.EnableAimReticule(true);

        float launchPower = 0f;
        float unscaledLaunchPower = 0f;

        //While they're holding
        while (Input.GetMouseButton(0) && Input.touchCount > 0 && unit != null)
        {
            //Get the potential power of the launch of the player released the aim at this moment
            launchPower = GetLaunchPower(unit);
            unscaledLaunchPower = Mathf.Clamp(Vector3.Distance(GetMousePointInWorld(), unit.transform.position) / combat.maxPowerRange, 0f, 1f);
            //Debug.Log("Current launch power is " + launchPower);

            //Face unit toward opposite of mouse position in world
            unit.AimToward((-(GetMousePointInWorld() - unit.transform.position))*100f, launchPower);
            yield return null;
        }
        //and Launch
        Debug.Log(unscaledLaunchPower);
        if (unscaledLaunchPower > combat.minimumLaunchPower) unit.Launch(launchPower);

        //Remove aim reticules
        unit.EnableAimReticule(false) ;

        GameController.gameNavicable = true;
    }

    public float GetLaunchPower(Unit unit)
    {
        if(unit != null)
        {
            //what was drawn
            float pwr = Mathf.Clamp(Vector3.Distance(GetMousePointInWorld(), unit.transform.position) / combat.maxPowerRange, 0f, 1f);
            //What it can be with the selected unit's stamina
            return Mathf.Clamp(pwr, 0f, unit.stamina.currentValue / (unit.attackDamageBase * pwr));
        }
        return 0f;
    }

    public Vector3 GetMousePointInWorld()
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
