using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public static bool gameNavicable;
    public static bool gamePlayable;
    public static bool unitsSelectable;
    public static bool uiNavicable;

    [Space(10)][Header("Arena State")]
    [SerializeField] protected Transform arenaBounds;
    [SerializeField] public LevelProfile_SO level;
    [SerializeField] protected int movableUnits = 0;
    [SerializeField] public float reward = 0f;

    [Space(10)][Header("Spawned Entities")]
    [SerializeField] public List<GameObject> activeUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> slainEnemyIcons = new List<GameObject>();
    [SerializeField] protected List<GameObject> activeWorldPowerUps = new List<GameObject>();
    [SerializeField] public List<PowerUp> allyPowerUps = new List<PowerUp>();
    [SerializeField] public List<PowerUp> enemyPowerUps = new List<PowerUp>();

    [Space(10)][Header("VFX")]
    [SerializeField] public GameObject allyTrail;
    [SerializeField] public GameObject enemyTrail;
    [SerializeField] public GameObject unitDeathEffect;
    [SerializeField] public GameObject healthUpEffect;
    [SerializeField] public GameObject speedUpEffect;
    [SerializeField] public GameObject strengthUpEffect;
    [SerializeField] public GameObject staminaUpEffect;
    [SerializeField] public GameObject boostEffect;
    enum GameState { MainMenu, Game, Paused, GameOver}
    GameState gameState = GameState.MainMenu;

    [Space(10)][Header("Powerups")]
    IEnumerator PowerUPSpawnSequencer;
    [SerializeField] protected float powerUpSpawnFrequency = 5f;
    public enum PowerUp { speed, strength, health, stamina }
    [SerializeField] protected float powerUpDefaultDuration = 10f;
    float[] speedPUDuration = { 0f, 0f };
    float[] staminaPUDuration = { 0f, 0f };
    float[] healthPUDuration = { 0f, 0f };
    float[] strengthPUDuration = { 0f, 0f };
    IEnumerator SpeedPUSequencer;
    IEnumerator StrengthPUSequencer;
    IEnumerator HealthPUSequencer;
    IEnumerator StaminaPUSequencer;
    IEnumerator EnemySpeedPUSequencer;
    IEnumerator EnemyStrengthPUSequencer;
    IEnumerator EnemyHealthPUSequencer;
    IEnumerator EnemyStaminaPUSequencer;

    // Start is called before the first frame update
    void Awake()
    {
        //Assign the relevent constants
        gameController = this;
        if (arenaBounds == null) arenaBounds = GameObject.FindWithTag("MapBounds").transform;

        //Set initial game state
        SetGameState(GameState.MainMenu);
    }

    #region Game State

    void SetGameState(GameState state)
    {
        switch (state)
        {
            case (GameState.MainMenu):
                UIController.uiController.SetUIPanel("MainMenu");
                gamePlayable = false;
                gameNavicable = false;
                unitsSelectable = false;
                uiNavicable = true;
                SetTimeScale("Pause");
                break;
            case (GameState.Game):
                UIController.uiController.SetUIPanel("Game");
                gamePlayable = true;
                gameNavicable = true;
                unitsSelectable = true;
                uiNavicable = false;
                SetTimeScale("Play");
                break;
            case (GameState.Paused):
                UIController.uiController.SetUIPanel("Paused");
                gamePlayable = false;
                gameNavicable = false;
                unitsSelectable = false;
                uiNavicable = true;
                SetTimeScale("Pause");
                break;
            case (GameState.GameOver):
                UIController.uiController.SetUIPanel("GameOver");
                gamePlayable = false;
                gameNavicable = false;
                unitsSelectable = false;
                uiNavicable = true;
                Time.timeScale = 1f;
                break;
            default:
                Debug.Log("Game state does not exist, cannot update state");
                break;
        }
        gameState = state;
    }

    public void StartGame()
    {
        //Play camera rotation
        CameraController.cameraController.StartStopRotation(true);
        //Configure level
        arenaBounds.localScale = new Vector3(level.arenaRadius, arenaBounds.localScale.y, level.arenaRadius);
        SpawnUnitsInLevel();
        SetGameState(GameState.Game);
        UIController.uiController.ResetPowerUpUI();
        //Start power up spawning
        PowerUPSpawnSequencer = PowerUpSpawnSequence();
        StartCoroutine(PowerUPSpawnSequencer);
    }

    public void StopGame()
    {
        //Play camera rotation
        CameraController.cameraController.StartStopRotation(false);

        //Destroy all active units and powerups
        foreach (GameObject unit in activeUnits)
        {
            Destroy(unit);
        }
        activeUnits = new List<GameObject>();
        foreach (GameObject powerup in activeWorldPowerUps)
        {
            Destroy(powerup);
        }
        slainEnemyIcons = new List<GameObject>();

        activeWorldPowerUps = new List<GameObject>();
        allyPowerUps = new List<PowerUp>();
        enemyPowerUps = new List<PowerUp>();

        //Reset reward
        reward = 0f;

        //Stop spawning power ups
        StopCoroutine(PowerUPSpawnSequencer);
    }

    int MovableUnitsUpdateAndCount()
    {
        int movableUnitsCount = 0;
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(CombatController.combatController.combat.playerTeamIndex))
        {
            if (unit.GetComponent<Unit>().combatState == Unit.UnitState.idle)
            {
                movableUnitsCount++;
                unit.GetComponent<Unit>().UpdateSelectionRIng(true, CombatController.combatController.combat.selectionRingPrefab);
            }
            else
            {
                unit.GetComponent<Unit>().UpdateSelectionRIng(false, CombatController.combatController.combat.selectionRingPrefab);
            }
        }
        movableUnits = movableUnitsCount;

        if (Input.GetMouseButton(0) && gameState == GameState.Game)
        {
            return 0;
        }
        return movableUnitsCount;
    }

    bool SlowForPlayerMove()
    {
        if (MovableUnitsUpdateAndCount() > 0) return true;
        else return false;
    }

    public void SetTimeScale(string state)
    {
        if(state == "Play")
        {
            if (SlowForPlayerMove())
            {
                Time.timeScale = CombatController.combatController.combat.playerMoveTimeSlow;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        else if(state == "Pause")
        {
            Time.timeScale = 0f;
        }
    }

    public void ResetGame()
    {
        StopGame();
        StartGame();
    }

    public void PauseGame(bool pauseState)
    {
        if (pauseState)
        {
            //Play camera rotation
            CameraController.cameraController.StartStopRotation(false);
            SetGameState(GameState.Paused);
        }
        else
        {
            //Play camera rotation
            CameraController.cameraController.StartStopRotation(true);
            SetGameState(GameState.Game);
        }
    }

    public void BackToMainMenu()
    {
        StopGame();
        SetGameState(GameState.MainMenu);
    }

    #endregion

    #region Powerups

    public void AddPowerUpToTeam(int teamIndex, PowerUp powerUpType)
    {
        if (teamIndex == 0)
        {
            //UPDATE INVENTORY
            allyPowerUps.Add(powerUpType);
            UIController.uiController.UpdatePowerUpUI();
        }
        else if (teamIndex == 1)
        {
            //APPLY TO ENEMY
            enemyPowerUps.Add(powerUpType);
            ApplyPowerUpToTeam(powerUpType, teamIndex);
        }
    }

    public int GetNumberOfPowerUpsInTeam(PowerUp powerUpType, int teamIndex)
    {
        int num = 0;
        if(teamIndex == 0)
        {
            foreach(PowerUp powerUp in allyPowerUps)
            {
                if(powerUpType == powerUp)
                {
                    num++;
                }
            }
        }
        else if(teamIndex == 1)
        {
            foreach (PowerUp powerUp in enemyPowerUps)
            {
                if (powerUpType == powerUp)
                {
                    num++;
                }
            }
        }
        return num;
    }

    public void ApplyPowerUpToTeam(PowerUp powerUp, int teamIndex)
    {
        //IF there is one of more available, apply the topup
        if(GetNumberOfPowerUpsInTeam(powerUp, teamIndex) > 0)
        {
            //Remove from inventory
            RemovePowerUpFromTeam(teamIndex, powerUp);
            if(teamIndex == 0) UIController.uiController.UpdatePowerUpUI();

            //Apply Effects
            switch (powerUp)
            {
                case (PowerUp.health):
                    if(teamIndex == 0)
                    {
                        if (HealthPUSequencer != null) StopCoroutine(HealthPUSequencer);
                        HealthPUSequencer = ApplyHealthPowerUp(teamIndex);
                        StartCoroutine(HealthPUSequencer);
                    }
                    else
                    {
                        if (EnemyHealthPUSequencer != null) StopCoroutine(EnemyHealthPUSequencer);
                        EnemyHealthPUSequencer = ApplyHealthPowerUp(teamIndex);
                        StartCoroutine(EnemyHealthPUSequencer);
                    }
                    break;
                case (PowerUp.stamina):
                    if(teamIndex == 0)
                    {
                        if (StaminaPUSequencer != null) StopCoroutine(StaminaPUSequencer);
                        StaminaPUSequencer = ApplyStaminaPowerUp(teamIndex);
                        StartCoroutine(StaminaPUSequencer);
                        break;
                    }
                    else
                    {
                        if (EnemyStaminaPUSequencer != null) StopCoroutine(EnemyStaminaPUSequencer);
                        EnemyStaminaPUSequencer = ApplyStaminaPowerUp(teamIndex);
                        StartCoroutine(EnemyStaminaPUSequencer);
                        break;
                    }
                case (PowerUp.strength):
                    if(teamIndex == 0)
                    {
                        if (StrengthPUSequencer != null) StopCoroutine(StrengthPUSequencer);
                        StrengthPUSequencer = ApplyStrengthPowerUp(teamIndex);
                        StartCoroutine(StrengthPUSequencer);
                        break;
                    }
                    else
                    {
                        if (EnemyStrengthPUSequencer != null) StopCoroutine(EnemyStrengthPUSequencer);
                        EnemyStrengthPUSequencer = ApplyStrengthPowerUp(teamIndex);
                        StartCoroutine(EnemyStrengthPUSequencer);
                        break;
                    }
                case (PowerUp.speed):
                    if(teamIndex == 0)
                    {
                        if (SpeedPUSequencer != null) StopCoroutine(SpeedPUSequencer);
                        SpeedPUSequencer = ApplySpeedPowerUp(teamIndex);
                        StartCoroutine(SpeedPUSequencer);
                        break;
                    }
                    else
                    {
                        if (EnemySpeedPUSequencer != null) StopCoroutine(EnemySpeedPUSequencer);
                        EnemySpeedPUSequencer = ApplySpeedPowerUp(teamIndex);
                        StartCoroutine(EnemySpeedPUSequencer);
                        break;
                    }
                default:
                    break;
            }
        }
    }

    public void RemovePowerUpFromTeam(int teamIndex, PowerUp powerUpType)
    {
        if (teamIndex == 0)
        {
            //UPDATE INVENTORY
            foreach(PowerUp powerUp in allyPowerUps)
            {
                if(powerUp == powerUpType)
                {
                    allyPowerUps.Remove(powerUp);
                    break;
                }
            }
        }
        else if (teamIndex == 1)
        {
            //UPDATE INVENTORY
            foreach (PowerUp powerUp in allyPowerUps)
            {
                if (powerUp == powerUpType)
                {
                    enemyPowerUps.Remove(powerUp);
                    break;
                }
            }
        }
    }

    IEnumerator ApplySpeedPowerUp(int teamIndex)
    {
        float modifier = 1.5f; //Launch speed multiplier
        float regenModifier = -5f; //cooldown time decrease

        //Apply effects
        foreach(GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().attackCooldown.buffs = regenModifier;
           // unit.GetComponent<Unit>().poweredUpSpeed = modifier;
            unit.GetComponent<Unit>().AddEffect("Speed");
        }

        //Handle duration
        speedPUDuration[teamIndex] = powerUpDefaultDuration;
        while (speedPUDuration[teamIndex] > 0f)
        {
            speedPUDuration[teamIndex] -= Time.deltaTime;
            speedPUDuration[teamIndex] = Mathf.Clamp(speedPUDuration[teamIndex], 0f, powerUpDefaultDuration);
            if (teamIndex == 0) UIController.uiController.SetCooldownUI(PowerUp.speed, speedPUDuration[teamIndex] / powerUpDefaultDuration);
            yield return null;
        }

        //Remove effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().attackCooldown.buffs = 0f;
          //  unit.GetComponent<Unit>().poweredUpSpeed = 1f;
            unit.GetComponent<Unit>().RemoveEffect("Speed");
        }
        yield return null;
    }
    IEnumerator ApplyHealthPowerUp(int teamIndex)
    {
        float modifier = 20f; //Health to heal
        float regenModifier = 5f; //regen increase from 0

        //Apply effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().health.buffs = regenModifier;
            unit.GetComponent<Unit>().TakeDamage(-modifier);
            unit.GetComponent<Unit>().AddEffect("Health");
        }

        //Handle duration
        healthPUDuration[teamIndex] = powerUpDefaultDuration;
        while (healthPUDuration[teamIndex] > 0f)
        {
            healthPUDuration[teamIndex] -= Time.deltaTime;
            healthPUDuration[teamIndex] = Mathf.Clamp(healthPUDuration[teamIndex], 0f, powerUpDefaultDuration);
            if (teamIndex == 0) UIController.uiController.SetCooldownUI(PowerUp.health, healthPUDuration[teamIndex] / powerUpDefaultDuration);
            yield return null;
        }

        //Remove effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().health.buffs = 0f;
            unit.GetComponent<Unit>().RemoveEffect("Health");
        }
        yield return null;
    }
    IEnumerator ApplyStaminaPowerUp(int teamIndex)
    {
        float modifier = 60f; //Stamina one time boost
        float regenModifier = 5f; //Stamina regen rate increase from 0

        //Apply effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().stamina.buffs = regenModifier;
            unit.GetComponent<Unit>().UseStamina(-modifier);
            unit.GetComponent<Unit>().AddEffect("Stamina");
        }


        //Handle duration
        staminaPUDuration[teamIndex] = powerUpDefaultDuration;
        while (staminaPUDuration[teamIndex] > 0f)
        {
            staminaPUDuration[teamIndex] -= Time.deltaTime;
            staminaPUDuration[teamIndex] = Mathf.Clamp(staminaPUDuration[teamIndex], 0f, powerUpDefaultDuration);
            if (teamIndex == 0) UIController.uiController.SetCooldownUI(PowerUp.stamina, staminaPUDuration[teamIndex] / powerUpDefaultDuration);
            yield return null;
        }

        //Remove effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().stamina.buffs = 0f;
            unit.GetComponent<Unit>().RemoveEffect("Stamina");
        }
        yield return null;
    }
    IEnumerator ApplyStrengthPowerUp(int teamIndex)
    {
        float modifier = 1.5f;

        //Apply effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().poweredUpStrength = modifier;
            unit.GetComponent<Unit>().AddEffect("Strength");
        }

        //Handle duration
        strengthPUDuration[teamIndex] = powerUpDefaultDuration;
        while (strengthPUDuration[teamIndex] > 0f)
        {
            strengthPUDuration[teamIndex] -= Time.deltaTime;
            strengthPUDuration[teamIndex] = Mathf.Clamp(strengthPUDuration[teamIndex], 0f, powerUpDefaultDuration);
            if (teamIndex == 0) UIController.uiController.SetCooldownUI(PowerUp.strength, strengthPUDuration[teamIndex] / powerUpDefaultDuration);
            yield return null;
        }

        //Remove effects
        foreach (GameObject unit in GetAllActiveUnitsOfTeam(teamIndex))
        {
            unit.GetComponent<Unit>().poweredUpStrength = 1f;
            unit.GetComponent<Unit>().RemoveEffect("Strength");
        }
        yield return null;
    }


    #endregion

    #region SpawnHandler


    void SpawnUnitsInLevel()
    {
        int allyCount = Mathf.RoundToInt(Random.Range(level.allyQuantity.x, level.allyQuantity.y));
        int enemyCount = Mathf.RoundToInt(Random.Range(level.enemyQuantity.x, level.enemyQuantity.y));
        int powerUpCount = Mathf.RoundToInt(Random.Range(level.powerUpQuantity.x, level.powerUpQuantity.y));

        SpawnAllUnitsInFromPool(level.alliesPool);
        SpawnUnitsFromPool(level.enemiesPool, enemyCount);
        SpawnPowerUps(level.powerUpsPool, powerUpCount);

        UIController.uiController.GenerateStatusUI();
    }

    void SpawnAllUnitsInFromPool(GameObject[] pool)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            //Get a random unit
            GameObject unit = pool[i];

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
                foreach (GameObject activeUnit in activeUnits)
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

    IEnumerator PowerUpSpawnSequence()
    {
        while (true)
        {
            if(activeWorldPowerUps.Count < level.powerUpQuantity.x)
            {
                if(Random.RandomRange(0f,1f) > 0.3f)
                {
                    SpawnPowerUps(level.powerUpsPool, 1);
                }
            }
            yield return new WaitForSeconds(powerUpSpawnFrequency);
        }
    }

    #endregion

    #region Game Over and Unit Scrubbing

    public void RemoveUnit(GameObject unit)
    {
        int removedUnitsTeam = unit.GetComponent<Unit>().teamIndex;

        //If it was an enemy, add it to the list of units slain this round
        GameObject enemyUnitIcon = unit.GetComponent<Unit>().unitIconPrefab;
        if (removedUnitsTeam != 0)
        {
            slainEnemyIcons.Add(enemyUnitIcon);
            reward += unit.GetComponent<Unit>().reward;
        }

        unit.GetComponent<Unit>().UpdateSelectionRIng(false, CombatController.combatController.combat.selectionRingPrefab);

        //Remove a dead unit and check if it was the last in its team
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

        //Stop spawning power ups
        StopCoroutine(PowerUPSpawnSequencer);

        //Set game to over
        SetGameState(GameState.GameOver);

        //A method of triggering the end game process at the death of certain teams
        switch (teamIndex)
        {
            case (0): //Player team
                //RUN GAME OVER SCREEN HERE
                Debug.Log("The player has lost the game.");
                UIController.uiController.InitiateGameOverUI(false);
                break;
            case (1): //Default Enemy team
                //RUN GAME OVER SCREEN HERE
                Debug.Log("The enemies have fallen!");
                UIController.uiController.InitiateGameOverUI(true);
                break;
            default:
                Debug.Log("That team index has not been coded in yet?!?");
                break;
        }
    }

    #endregion

}
