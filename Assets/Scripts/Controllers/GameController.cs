﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] public static GameController gameController;
    [SerializeField] public static bool gameNavicable;
    [SerializeField] public static bool gamePlayable;
    [SerializeField] public static bool unitsSelectable;
    [SerializeField] protected Transform arenaBounds;
    [SerializeField] public LevelProfile_SO level;
    [SerializeField] public List<GameObject> activeUnits = new List<GameObject>();
    [SerializeField] protected List<GameObject> activeWorldPowerUps = new List<GameObject>();
    [SerializeField] public List<GameObject> allyPowerUps = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyPowerUps = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        //Assign the relevent constants
        gameController = this;
        if (arenaBounds == null) arenaBounds = GameObject.FindWithTag("MapBounds").transform;

        //Configure level
        arenaBounds.localScale = new Vector3(level.arenaRadius, arenaBounds.localScale.y, level.arenaRadius);
        SpawnUnitsInLevel();

        //Start Game
        gamePlayable = true;
        gameNavicable = true;
        unitsSelectable = true;
    }

    public void AddPowerUpToTeam(int teamIndex, GameObject powerUpPrefab)
    {
        if (teamIndex == 0)
        {
            //UPDATE INVENTORY
            allyPowerUps.Add(Instantiate(powerUpPrefab, UIController.uiController.itemPanel));
            UIController.uiController.UpdateInventoryUI();

        }
        else if (teamIndex == 1)
        {
            //APPLY TO ENEMY
            //enemyPowerUps.Add(powerUpPrefab); //In the future this can be reapplied to this list if the AI is to hold powerups
            powerUpPrefab = Instantiate(powerUpPrefab);
            Unit unitToEmpower = GetAllActiveUnitsOfTeam(teamIndex)[Random.Range(0, GetAllActiveUnitsOfTeam(teamIndex).Count - 1)].GetComponent<Unit>();
            PowerUp powerUp = powerUpPrefab.GetComponent<PowerUp>();
            powerUp.PowerUpUnit(unitToEmpower);
        }
    }


    #region SpawnHandler

    void SpawnUnitsInLevel()
    {
        int allyCount = Mathf.RoundToInt(Random.Range(level.allyQuantity.x, level.allyQuantity.y));
        int enemyCount = Mathf.RoundToInt(Random.Range(level.enemyQuantity.x, level.enemyQuantity.y));
        int powerUpCount = Mathf.RoundToInt(Random.Range(level.powerUpQuantity.x, level.powerUpQuantity.y));

        SpawnUnitsFromPool(level.alliesPool, allyCount);
        SpawnUnitsFromPool(level.enemiesPool, enemyCount);
        SpawnPowerUps(level.powerUpsPool, powerUpCount);

        UIController.uiController.GenerateStatusUI();
    }

    void SpawnUnitsFromPool(GameObject[] pool, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            //Get a random unit
            GameObject unit = pool[Random.Range(0, pool.Length)];

            //Run through until its spawned:
            bool spawned = false;
            while (!spawned)
            {
                bool intersects = false;
                //Get a spawnpoint in the arena, taking into account the width of the unit
                Vector2 spawnPoint2D = Random.insideUnitCircle * (level.arenaRadius - unit.GetComponent<SphereCollider>().radius);
                //Make it 3D and offset it with the map origin (gamecontroller)
                Vector3 spawnPoint = new Vector3(spawnPoint2D.x, level.levelYOffset, spawnPoint2D.y) + transform.position;

                //Check to see if it intersects
                foreach(GameObject activeUnit in activeUnits)
                {
                    //Get the minimum distance that should be between units
                    float minDistanceBetween = activeUnit.GetComponent<SphereCollider>().radius + unit.GetComponent<SphereCollider>().radius + level.spawnSpacingMinimum;
                    //If the spawn point and the currect unit being looped through are closer than that, stop this run of the loop
                    if (Vector3.Distance(activeUnit.transform.position, spawnPoint) < minDistanceBetween)
                    {
                        intersects = true;
                        break;
                    }
                }

                //If there are no intersection errors, instantiate the unit
                if (!intersects)
                {
                    spawned = true;
                    activeUnits.Add(Instantiate(unit, spawnPoint, Quaternion.identity));
                }
            }
        }
    }

    void SpawnPowerUps(GameObject[] pool, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //Get a random powerUp
            GameObject powerUp = pool[Random.Range(0, pool.Length)];

            //Run through until its spawned:
            bool spawned = false;
            while (!spawned)
            {
                bool intersects = false;
                //Get a spawnpoint in the arena, taking into account the width of the powerUp
                Vector2 spawnPoint2D = Random.insideUnitCircle * (level.arenaRadius - powerUp.GetComponent<SphereCollider>().radius);
                //Make it 3D and offset it with the map origin (gamecontroller)
                Vector3 spawnPoint = new Vector3(spawnPoint2D.x, level.levelYOffset, spawnPoint2D.y) + transform.position;

                //Check to see if it intersects units or other powerups
                foreach (GameObject activeUnit in activeUnits)
                {
                    //Get the minimum distance that should be between things
                    float minDistanceBetween = activeUnit.GetComponent<SphereCollider>().radius + powerUp.GetComponent<SphereCollider>().radius + level.spawnSpacingMinimum;
                    //If the spawn point and the currect unit being looped through are closer than that, stop this run of the loop
                    if (Vector3.Distance(activeUnit.transform.position, spawnPoint) < minDistanceBetween)
                    {
                        intersects = true;
                        break;
                    }
                }
                foreach (GameObject powerUps in activeWorldPowerUps)
                {
                    if (intersects == true) break;
                    //Get the minimum distance that should be between things
                    float minDistanceBetween = powerUps.GetComponent<SphereCollider>().radius + powerUp.GetComponent<SphereCollider>().radius + level.spawnSpacingMinimum;
                    //If the spawn point and the currect unit being looped through are closer than that, stop this run of the loop
                    if (Vector3.Distance(powerUps.transform.position, spawnPoint) < minDistanceBetween)
                    {
                        intersects = true;
                        break;
                    }
                }


                //If there are no intersection errors, instantiate the powerUp
                if (!intersects)
                {
                    spawned = true;
                    activeWorldPowerUps.Add(Instantiate(powerUp, spawnPoint, Quaternion.identity));
                }
            }
        }
    }

    #endregion

    #region Game Over and Unit Scrubbing

    public void RemoveUnit(GameObject unit)
    {
        //Remove a dead unit and check if it was the last in its team
        int removedUnitsTeam = unit.GetComponent<Unit>().teamIndex;
        activeUnits.Remove(unit);
        if(GetTeamUnitCountInActiveUnits(removedUnitsTeam) == 0)
        {
            //If it was, run the elimination process for that team
            TeamElimination(removedUnitsTeam);
        }
    }

    public void RemoveWorldPowerUp(GameObject powerup)
    {
        activeWorldPowerUps.Remove(powerup);
    }

    public void RemovePowerUp(GameObject gameObject, int teamIndex)
    {
        //Remove powerups of a team from their list
        if(teamIndex == 0)
        {
            foreach (GameObject powerUp in allyPowerUps)
            {
                if (gameObject == powerUp)
                {
                    allyPowerUps.Remove(powerUp);
                    break;
                }
            }
        }
        else if (teamIndex == 1)
        {
            foreach (GameObject powerUp in enemyPowerUps)
            {
                if (gameObject == powerUp)
                {
                    allyPowerUps.Remove(powerUp);
                    break;
                }
            }
        }

    }

    private int GetTeamUnitCountInActiveUnits(int teamIndex)
    {
        //Small function that returns the remaining number of teammates in a team
        int numberOfTeammates = 0;
        foreach(GameObject unit in activeUnits)
        {
            if (unit.GetComponent<Unit>().teamIndex == teamIndex) numberOfTeammates++;
        }
        return numberOfTeammates;
    }

    public List<GameObject> GetAllActiveUnitsOfTeam(int teamIndex)
    {
        List<GameObject> unitsList = new List<GameObject>();
        foreach (GameObject unit in activeUnits)
        {
            if (unit.GetComponent<Unit>().teamIndex == teamIndex) unitsList.Add(unit);
        }
        return unitsList;
    }

    public void TeamElimination(int teamIndex)
    {
        //A method of triggering the end game process at the death of certain teams
        switch (teamIndex)
        {
            case (0): //Player team
                //RUN GAME OVER SCREEN HERE
                Debug.Log("The player has lost the game.");
                break;
            case (1): //Default Enemy team
                //RUN GAME OVER SCREEN HERE
                Debug.Log("The enemies have fallen!");
                break;
            default:
                Debug.Log("That team index has not been coded in yet?!?");
                break;
        }

        //End Game
        gamePlayable = false;
        unitsSelectable = false;
    }

    public void RefreshGameLevel()
    {
        gamePlayable = true;
        gameNavicable = true;
        unitsSelectable = true;
        foreach(GameObject unit in activeUnits)
        {
            Destroy(unit);
        }
        activeUnits = new List<GameObject>();
        foreach(GameObject powerup in activeWorldPowerUps)
        {
            Destroy(powerup);
        }
        activeWorldPowerUps = new List<GameObject>();
        foreach(GameObject allyPowerUp in allyPowerUps)
        {
            Destroy(allyPowerUp);
        }
        allyPowerUps = new List<GameObject>();
        enemyPowerUps = new List<GameObject>();
        SpawnUnitsInLevel();
    }

    #endregion

}
