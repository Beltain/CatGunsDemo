using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Combat_Profile", menuName = "Game/New Combat Profile")]
public class CombatProfile_SO : ScriptableObject
{
    public int playerTeamIndex = 0;
    public float attackAngle = 75f;
    public float maxPowerRange = 12f;
    public float alliedPowerBoostLimit = 5f;
    public float gameSpeedMultiplier = 1f;
    public float secondsBeforeStart = 2f;
    public float attackChoreographyTime = 1f;
    public float unitSelectionRadius = 5f;
    
}
