using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Unit : MonoBehaviour
{
    //Attributes
    [SerializeField] protected float combatRadius = 2.5f;
    [SerializeField] public float launchSpeed = 50f;
    [SerializeField] public int teamIndex = 0;
    [SerializeField] public float attackDamageBase = 50f;

    //Current States
    public float attackCooldownBase;
    public Vector2 attackCooldownRange = new Vector2(1f, 4f);
    protected float launchPower = 1f; //Range: 0 - 1
    public float nextAttackAllieBoosted = 0f; //Can increase indefinitely
    protected float currentAttackAllieBoosted = 0f;
    bool justReceivedDamage = false;

    //Vitality
    [SerializeField] public Status attackCooldown;
    [SerializeField] public Status health = new Status();
    [SerializeField] public Status stamina = new Status();

    //State Handling
    public enum UnitState { idle, attacking, stunned };
    public UnitState combatState;

    //Sequencers
    IEnumerator LaunchSequencer;
    IEnumerator StatusTicker;

    //Components
    protected SphereCollider boundSphere;
    protected Rigidbody rigidbody;
    [SerializeField] protected Transform[] aimReticule = { null, null };
    [SerializeField] protected Color[] aimReticuleStageGradient;

    private void Start()
    {
        UnitSetup();
    }

    private void UnitSetup()
    {
        boundSphere = GetComponent<SphereCollider>();
        boundSphere.radius = combatRadius;
        rigidbody = GetComponent<Rigidbody>();

        attackCooldownBase = Random.Range(attackCooldownRange.x, attackCooldownRange.y);
        attackCooldown = new Status(attackCooldownBase, 0f, 0f, -1f, 0f);

        combatState = UnitState.idle;
        
        FaceCamera();
        EnableAimReticule(false);

        StartStatusTicker();
    }

    #region Attack Handling

    //Handling collisions
    private void OnCollisionEnter(Collision collision)
    {
        //See if the collider hit was another unit, and if it was on an enemy team
        if(collision.gameObject.GetComponent<Unit>() != null)
        {
            
            Unit unitHit = collision.gameObject.GetComponent<Unit>();
            if(unitHit.teamIndex != teamIndex && combatState == UnitState.attacking)
            {
                //Combat Code
                CalculateCombatOutcome(unitHit);

                Debug.Log(collision.gameObject.name);
            }

            else if(unitHit.teamIndex == teamIndex && combatState == UnitState.attacking)
            {
                //Allie Boost Code
                UIController.uiController.PopupAlertUI(UIController.boostedMessage, unitHit.transform.position);
                unitHit.nextAttackAllieBoosted += launchPower + currentAttackAllieBoosted;
                currentAttackAllieBoosted = 0f;
            }
        }
    }

    private void CalculateCombatOutcome(Unit enemyUnit)
    {
        //Debug.Log("Combat Initiated");
        switch (enemyUnit.combatState)
        {
            case (UnitState.attacking):
                //Check to make sure the enemy unit is within the attack angle
                if(Vector3.Angle(transform.forward, enemyUnit.transform.position - transform.position) <= CombatController.combatController.combat.attackAngle)
                {
                    //Debug.Log("Enemy Unit within the attack radius of " + gameObject.name);
                    //When this unit lands an attack but the other unit is also attacking, do this
                    if (Vector3.Angle(enemyUnit.transform.forward, transform.position - enemyUnit.transform.position) <= CombatController.combatController.combat.attackAngle)
                    {
                        //Parry Code
                        //Isolate one unit to play the parry effects, as both would if not specified
                        if(enemyUnit.teamIndex > teamIndex)
                        {
                            //Debug.Log(enemyUnit.gameObject.name + " and " + gameObject.name + " Parried");
                            UIController.uiController.PopupAlertUI(UIController.parriedMessage, (transform.position + enemyUnit.transform.position) / 2f);
                        }
                    }
                    else
                    {
                        //Deal Damage Code
                        //Debug.Log("Attack landed on " + enemyUnit.gameObject.name + " despite them attack as well");
                        DealAttack(enemyUnit);
                    }
                }
                break;

            default:
                //When this unit lands an attack, do this
                //Interrupt the other units attack
                if(enemyUnit.teamIndex == 1)
                {
                    enemyUnit.GetComponent<BasicEnemyAI>().StopLaunch();
                }

                //check if the enemy unit is in the attack radius
                //Debug.Log("Attack landed on " + enemyUnit.gameObject.name);
                if (Vector3.Angle(transform.forward, enemyUnit.transform.position - transform.position) <= CombatController.combatController.combat.attackAngle)
                {
                    //Deal damage code
                    DealAttack(enemyUnit);
                }
                break;
        }
    }

    private void DealAttack(Unit enemyUnit)
    {
        enemyUnit.TakeDamage(attackDamageBase * (launchPower + currentAttackAllieBoosted) );
        //Debug.Log(enemyUnit.gameObject.name + " took damage");
        currentAttackAllieBoosted = 0f;
    }

#endregion

    #region Launching and Aiming

    public void Launch(float _launchPower)
    {
        //Reset power variables
        launchPower = _launchPower;
        currentAttackAllieBoosted = nextAttackAllieBoosted;
        nextAttackAllieBoosted = 0f;

        //Start the coroutine that will handle the launch
        if (LaunchSequencer != null) StopCoroutine(LaunchSequencer);
        LaunchSequencer = LaunchSequence();
        StartCoroutine(LaunchSequencer);
    }

    IEnumerator LaunchSequence()
    {
        //Use Stamina
        UseStamina(attackDamageBase * launchPower);
        //Start Cooldown
        attackCooldown.currentValue = attackCooldownBase;
        //remove aim
        EnableAimReticule(false);

        //Set State
        combatState = UnitState.attacking;

        //Add initial velocity and make sure the unit can travel further than their max stamina allows if boosted by an allie
        float launchVelocityMultiplier = Mathf.Clamp(launchPower + currentAttackAllieBoosted, 0f, 1f);
        rigidbody.velocity += transform.forward * launchSpeed * launchVelocityMultiplier;

        //Dampen over time (using drag in RB)
        while (rigidbody.velocity.magnitude > 0.2f)
        {
            yield return null;
        }
        //Clear all residual velocity and reset launch power
        rigidbody.velocity = Vector3.zero;
        launchPower = 0f;
        currentAttackAllieBoosted = 0f;

        //Set inactive during cooldown and face the unit forward
        StartCoroutine(CoolDownSequence());

    }

    public void AimToward(Vector3 target, float power)
    {
        //Look toward a spot in the world but dont look up/down
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));

        //Adjust according to draw power
        AdjustAimReticuleSizeAndColor(power);
    }

    #endregion

    #region Aiming Visuals
    public void EnableAimReticule(bool state)
    {
        foreach (Transform aimRet in aimReticule)
        {
            aimRet.gameObject.SetActive(state);
        }
    }

    public void AdjustAimReticuleSizeAndColor(float size)
    {
        //get the current power bracket and assign it the right color
        Color sizeChosenColor;
        if (size <= 0.2f) sizeChosenColor = aimReticuleStageGradient[0];
        else if (size > 0.2f && size <= 0.4f) sizeChosenColor = aimReticuleStageGradient[1];
        else if (size > 0.4f && size <= 0.6f) sizeChosenColor = aimReticuleStageGradient[2];
        else if (size > 0.6f && size <= 0.8f) sizeChosenColor = aimReticuleStageGradient[3];
        else sizeChosenColor = aimReticuleStageGradient[4];

        //Change the size of the reticules according to game settings
        size *= CombatController.combatController.combat.maxPowerRange/16f;
        size = Mathf.Clamp(size, 0f, CombatController.combatController.combat.maxPowerRange/16f);
        foreach (Transform aimRet in aimReticule)
        {
            aimRet.localScale = Vector3.one * size;
            //Get each renderer, go through it and make sure all materials are changed
            Renderer[] rendersToChange = aimRet.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rendersToChange)
            {
                rend.material.SetColor("_BaseColor", sizeChosenColor);
            }
        }
    }

    public void FaceCamera()
    {
        transform.LookAt(new Vector3(Camera.main.gameObject.transform.position.x, transform.position.y, -10000f));
    }

    #endregion

    #region Status Handling

    void StartStatusTicker()
    {
        List<Status> vitality = new List<Status>();

        vitality.Add(health);
        vitality.Add(stamina);
        vitality.Add(attackCooldown);

        if (StatusTicker != null) StopCoroutine(StatusTicker);
        StatusTicker = TickVitality(vitality);
        StartCoroutine(StatusTicker);
    }

    IEnumerator TickVitality(List<Status> vitalities)
    {
        while (true)
        {
            if (GameController.gamePlayable)
            {

                foreach (Status vitality in vitalities)
                {
                    //Tick the value of the vitality each frame
                    vitality.currentValue = Mathf.Clamp(vitality.currentValue + (vitality.regenRate + vitality.buffs) * Time.deltaTime, vitality.minValue, vitality.maxValue);

                    //And smooth the smoothed value to the current over time
                    if (vitality.smoothedValue != vitality.currentValue)
                    {
                        vitality.smoothedValue = Mathf.Lerp(vitality.smoothedValue, vitality.currentValue, Time.deltaTime * 1f);
                        if (Mathf.Abs(vitality.currentValue - vitality.smoothedValue) < 0.1f) vitality.smoothedValue = vitality.currentValue;
                    }
                }
                yield return null;
            }
            else yield return null;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!justReceivedDamage)
        {
            justReceivedDamage = true;
            health.currentValue = Mathf.Clamp(health.currentValue - damage, health.minValue, health.maxValue);
            UIController.uiController.PopupAlertUI(Mathf.RoundToInt(damage).ToString(), transform.position);
            if (health.currentValue == 0f) Die();
            Invoke("JustReceivedDamage", 0.1f);
        }
    }
    private void JustReceivedDamage()
    {
        justReceivedDamage = false;
    }

    public void UseStamina(float amount)
    {
        stamina.currentValue = Mathf.Clamp(stamina.currentValue - amount, stamina.minValue, stamina.maxValue);
    }

    public void SetHealth(float value)
    {
        health.currentValue = Mathf.Clamp(health.currentValue, health.minValue, health.maxValue);
        if (health.currentValue == 0f) Die();
    }

    public void SetStamina(float value)
    {
        stamina.currentValue = Mathf.Clamp(stamina.currentValue, stamina.minValue, stamina.maxValue);
    }

    IEnumerator CoolDownSequence()
    {
        //keep unit stunned until it's cooldown has lapsed
        combatState = UnitState.stunned;
        while (attackCooldown.currentValue != 0f)
        {
            yield return null;
        }
        combatState = UnitState.idle;
        FaceCamera();
    }

    private void Die()
    {
        GameController.gameController.RemoveUnit(this.gameObject);
        StopAllCoroutines();
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        //Add all death processes here

        Destroy(gameObject);

        yield return null;
    }

    #endregion

}
