using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class BasicEnemyAI : MonoBehaviour
{
    private enum Strategy { aggressive, protective, assistive };
    private Strategy strat = Strategy.aggressive;

    Unit unit;

    [SerializeField] protected Vector2 timeRangeBetweenMoves = new Vector2(2f, 5f);
    [SerializeField] protected Vector2 attackPowerRange = new Vector2(0.5f, 1f);
    [SerializeField] protected Vector2 protectiveBubbleRadius = new Vector2(4f, 6f);
    [SerializeField] protected float escapeSpeedMultiplier = 2f;
    IEnumerator AISequencer;
    IEnumerator StrategySequencer;


    private void Start()
    {
        //Get unit
        unit = GetComponent<Unit>();

        //Start AI
        AISequencer = AISequence();
        StartCoroutine(AISequencer);
    }

    IEnumerator AISequence()
    {
        //Pause to allow player to get accustomed to the level
        yield return new WaitForSecondsRealtime(CombatController.combatController.combat.secondsBeforeStart);

        //Every random amount of seconds, choose a strategy and perform it
        float timeTillNextChange = 0f; 
        while (true)
        {
            if (GameController.gamePlayable)
            {
                //cease previous strat
                if (StrategySequencer != null) StopCoroutine(StrategySequencer);

                timeTillNextChange += Random.Range(timeRangeBetweenMoves.x, timeRangeBetweenMoves.y) * CombatController.combatController.combat.gameSpeedMultiplier;
                //Randomise the next strat
                float stratChangeNumber = Random.Range(0f, 1f);
                if (0f <= stratChangeNumber && stratChangeNumber < 0.5f)
                {
                    //AGGRESIVE FOCUS
                    StrategySequencer = AggresiveStrategy(timeTillNextChange);
                }
                else if (0.5 <= stratChangeNumber && stratChangeNumber < 0.8f)
                {
                    //PROTECTIVE FOCUS
                    StrategySequencer = ProtectiveStrategy(timeTillNextChange);
                }
                else if (0.8 <= stratChangeNumber && stratChangeNumber < 1.0f)
                {
                    //ASSISTIVE FOCUS
                    StrategySequencer = AssistiveStrategy(timeTillNextChange);
                }

                StartCoroutine(StrategySequencer);

                //WAIT FOR THE NEXT CHANGE
                while (timeTillNextChange > 0f)
                {
                    timeTillNextChange -= Time.deltaTime;
                    yield return null;
                }

            }
            else
            {
                break;
            }
        }
    }

    //AI Attack behavior
    IEnumerator AggresiveStrategy(float duration)
    {
        strat = Strategy.aggressive;

        //Get a target to aim for
        List<GameObject> enemyTeam = GameController.gameController.GetAllActiveUnitsOfTeam(0);
        Vector3 target = enemyTeam[Random.Range(0, enemyTeam.Count-1)].transform.position;

        //Launch at
        StartCoroutine(LaunchUnitAt(target, duration));

        yield return null;
    }

    IEnumerator ProtectiveStrategy(float duration)
    {
        strat = Strategy.protective;

        //Get randomised protective radius
        float protectiveRadius = Random.Range(protectiveBubbleRadius.x, protectiveBubbleRadius.y);
        bool enemyBreached = false;
        List<GameObject> enemyTeam = GameController.gameController.GetAllActiveUnitsOfTeam(0);

        while (duration > 0f)
        {
            //The unit will only succesfully evade attacks once
            if (!enemyBreached)
            {
                foreach (GameObject enemy in enemyTeam)
                {
                    if (Vector3.Distance(enemy.transform.position, transform.position) < protectiveRadius)
                    {
                        //If an enemy has breached the detection radius, get the shit outa there faster than usual
                        StartCoroutine(LaunchUnitAt(-(enemy.transform.position - transform.position) * 100f, duration / escapeSpeedMultiplier));
                        enemyBreached = true;
                        break;
                    }
                }
            }
            else break;
            duration -= Time.deltaTime;
        }

        yield return null;
    }

    IEnumerator AssistiveStrategy(float duration)
    {
        strat = Strategy.assistive;

        //Find all nearby allies
        List<GameObject> nearbyAllies = GameController.gameController.GetAllActiveUnitsOfTeam(1);
        Transform nearestAlly = null;

        foreach(GameObject ally in nearbyAllies)
        {
            //find the nearest ally to the AI
            if (nearestAlly == null) nearestAlly = ally.transform;
            else if(Vector3.Distance(nearestAlly.position, transform.position) > Vector3.Distance(ally.transform.position, transform.position))
            {
                nearestAlly = ally.transform;
            }
        }

        //Fire toward the nearest ally
        StartCoroutine(LaunchUnitAt(nearestAlly.position, duration));

        yield return null;
    }

    IEnumerator LaunchUnitAt(Vector3 target, float maxDuration)
    {
        //look at target
        unit.AimToward(target);

        //Get randomised launch force
        float launchPower = Random.Range(attackPowerRange.x, attackPowerRange.y);
        launchPower = Mathf.Clamp(launchPower, 0f, unit.stamina.currentValue / (unit.attackDamageBase * launchPower));

        //Choreograph move, making sure the choreography doesnt take too much time up
        float drawTime = Mathf.Clamp(CombatController.combatController.combat.attackChoreographyTime, 0f, maxDuration * 0.8f);
        while (drawTime > 0f)
        {
            //DO THIS BEFORE LAUNCHING
            Debug.Log("Launching in " + drawTime + " seconds");
            drawTime -= Time.deltaTime;
            yield return null;
        }

        //Launch
        unit.Launch(launchPower);

    }

}
