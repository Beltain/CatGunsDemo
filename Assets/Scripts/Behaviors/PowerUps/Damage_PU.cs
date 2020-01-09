using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_PU : PowerUp
{
    [SerializeField] protected float damageMultiplier = 1.2f;

    public override IEnumerator ApplyPowerUp(Unit unit)
    {
        unit.attackDamageBase *= damageMultiplier;
        yield return null;
    }
}
