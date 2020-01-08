using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Level_Profile", menuName = "Game/New Level Profile")]
public class LevelProfile_SO : ScriptableObject
{
    public float arenaRadius = 20f;
    public Vector2 enemyQuantity = new Vector2(4, 5);
    public Vector2 allyQuantity = new Vector2(3, 4);
    public Vector2 powerUpQuantity = new Vector2(2, 6);
    public GameObject[] alliesPool;
    public GameObject[] enemiesPool;
    public GameObject[] powerUpsPool;
    public float spawnSpacingMinimum = 3f;
    public float levelYOffset = 0.5f;
}
