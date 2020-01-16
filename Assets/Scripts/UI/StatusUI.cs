using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public Transform unitToTrack = null;
    public Unit unit;
    [SerializeField] protected Vector2 healthBarFillLimits = new Vector2(0.18f, 0.82f);
    [SerializeField] protected Vector2 staminaBarFillLimits = new Vector2(0.18f, 0.82f);
    [SerializeField] protected Vector2 cooldownFillLimits = new Vector2(0.18f, 0.82f);
    [SerializeField] Image healthFill;
    [SerializeField] Image healthTrail;
    [SerializeField] Image staminaFill;
    [SerializeField] Image staminaTrail;
    [SerializeField] Image cooldownFill;
    [SerializeField] Image cooldownTrail;
    [SerializeField] GameObject readyIndicator;
    bool unitIsReady = true;


    // Start is called before the first frame update
    void Start()
    {
        unit = unitToTrack.GetComponent<Unit>();
        if(unit.teamIndex != 0)
        {
            readyIndicator.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(unitToTrack != null)
        {
            //Update bars
            transform.position = Camera.main.WorldToScreenPoint(unitToTrack.position);
            healthFill.fillAmount = GetFillAmount(healthBarFillLimits, unit.health, unit.health.currentValue);
            healthTrail.fillAmount = GetFillAmount(healthBarFillLimits, unit.health, unit.health.smoothedValue);
            cooldownFill.fillAmount = GetFillAmount(cooldownFillLimits, unit.attackCooldown, unit.attackCooldown.maxValue - unit.attackCooldown.currentValue);
            cooldownTrail.fillAmount = GetFillAmount(cooldownFillLimits, unit.attackCooldown, unit.attackCooldown.maxValue - unit.attackCooldown.smoothedValue);
            staminaTrail.fillAmount = GetFillAmount(staminaBarFillLimits, unit.stamina, unit.stamina.smoothedValue);
            if (unit.nextAttackAllieBoosted != 0)
            {
                //Show the transference of mana from another unit to this one
                float fillAmount = GetFillAmount(staminaBarFillLimits, unit.stamina, unit.stamina.currentValue);
                staminaFill.fillAmount = fillAmount + (unit.nextAttackAllieBoosted * fillAmount);
            }
            else staminaFill.fillAmount = GetFillAmount(staminaBarFillLimits, unit.stamina, unit.stamina.currentValue);

            //Update ready indicator
            if (!unitIsReady && unit.teamIndex == 0)
            {
                if(unit.combatState == Unit.UnitState.idle && unit.attackCooldown.currentValue == 0)
                {
                    readyIndicator.SetActive(true);
                    unitIsReady = true;
                }
            }
            else if(unit.attackCooldown.currentValue != 0 && unit.teamIndex == 0)
            {
                readyIndicator.SetActive(false);
                unitIsReady = false;
            }

        }
        else UIController.uiController.RemoveStatusUI(this);
    }

    private float GetFillAmount(Vector2 limits, Status status, float valueToCheck)
    {
        float percentFilled = valueToCheck / (status.maxValue - status.minValue);
        return limits.x + percentFilled * Mathf.Abs(limits.y - limits.x);
    }


}
