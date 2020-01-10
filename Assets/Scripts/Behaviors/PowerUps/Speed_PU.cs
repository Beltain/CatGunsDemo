using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed_PU : PowerUp
{
    [SerializeField] protected float speedMultiplier = 1.5f;
    [SerializeField] protected float dragMultiplier = 1.2f;
    [SerializeField] protected float cooldownReduction = 1f;
    public override IEnumerator ApplyPowerUp(Unit unit)
    {
        unit.gameObject.GetComponent<Rigidbody>().drag *= dragMultiplier;
        unit.launchSpeed *= speedMultiplier;
        unit.attackCooldownBase = Mathf.Clamp(unit.attackCooldownBase - cooldownReduction, 0f, unit.attackCooldownBase);
        unit.attackCooldown.maxValue = unit.attackCooldownBase;
        yield return null;
    }
}
