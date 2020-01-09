using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_PU : PowerUp
{
    [SerializeField] protected float healthRegenMultiplier = 1.2f;
    [SerializeField] protected float healthRefillAmount = 0.5f;
    public override IEnumerator ApplyPowerUp(Unit unit)
    {
        unit.health.currentValue += healthRefillAmount * unit.health.maxValue;
        unit.health.buffs += unit.health.regenRate * healthRegenMultiplier;
        yield return null;
    }
}
