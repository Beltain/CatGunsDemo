using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina_PU : PowerUp
{
    [SerializeField] protected float staminaRegenMultiplier = 1.2f;
    [SerializeField] protected float staminaRefillAmount = 0.5f;
    public override IEnumerator ApplyPowerUp(Unit unit)
    {
        unit.stamina.currentValue += staminaRefillAmount * unit.stamina.maxValue;
        unit.stamina.buffs += unit.stamina.regenRate * staminaRegenMultiplier;
        yield return null;
    }
}
