using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Unit : MonoBehaviour
{
    //Attributes
    [SerializeField] protected float combatRadius = 2.5f;
    [SerializeField] protected float launchSpeed = 50f;
    [SerializeField] public int teamIndex = 0;

    //Vitality
    [SerializeField] protected Status health;
    [SerializeField] protected Status stamina;

    //State Handling
    public enum UnitState { idle, attacking, stunned };
    public UnitState combatState;

    //Sequencers
    IEnumerator LaunchSequencer;
    IEnumerator StatusTicker;

    //Components
    protected SphereCollider boundSphere;
    protected Rigidbody rigidbody;

    private void Start()
    {
        UnitSetup();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Launch();
    }

    private void UnitSetup()
    {
        boundSphere = GetComponent<SphereCollider>();
        boundSphere.radius = combatRadius;
        rigidbody = GetComponent<Rigidbody>();

        combatState = UnitState.idle;

        StartStatusTicker();
    }

    //Handling collisions
    private void OnCollisionEnter(Collision collision)
    {
        //See if the collider hit was another unit, and if it was on an enemy team
        if(collision.gameObject.GetComponent<Unit>() != null)
        {
            Unit unitHit = collision.gameObject.GetComponent<Unit>();
            if(unitHit.teamIndex != teamIndex && combatState == UnitState.attacking)
            {
                CalculateCombatOutcome(unitHit);
            }
        }
    }

    private void CalculateCombatOutcome(Unit enemyUnit)
    {
        switch (enemyUnit.combatState)
        {
            case (UnitState.attacking):
                //When this unit lands an attack but the other unit is also attacking, do this

                break;
            default:
                //When this unit lands an attack, do this
                break;
        }
    }

    #region Launching and Aiming

    public void Launch()
    {
        //Start the coroutine that will handle the launch
        if (LaunchSequencer != null) StopCoroutine(LaunchSequencer);
        LaunchSequencer = LaunchSequence();
        StartCoroutine(LaunchSequencer);
    }

    IEnumerator LaunchSequence()
    {
        //Set State
        combatState = UnitState.attacking;

        //Add initial velocity
        rigidbody.velocity += transform.forward * launchSpeed;

        //Dampen over time (using drag in RB)
        while (rigidbody.velocity.magnitude > 0.2f)
        {
            yield return null;
        }
        //Clear all residual velocity
        rigidbody.velocity = Vector3.zero;

        //Unset State
        combatState = UnitState.idle;
    }

    public void AimToward(Vector3 target)
    {
        //Look toward a spot in the world but dont look up/down
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }

    #endregion

    #region Status Handling

    void StartStatusTicker()
    {
        List<Status> vitality = new List<Status>();
        health = new Status();
        stamina = new Status();

        vitality.Add(health);
        vitality.Add(stamina);

        if (StatusTicker != null) StopCoroutine(StatusTicker);
        StatusTicker = TickVitality(vitality);
        StartCoroutine(StatusTicker);
    }

    IEnumerator TickVitality(List<Status> vitalities)
    {
        float tick = 0f;

        while (true)
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
    }

    public void TakeDamage(float damage)
    {
        health.currentValue = Mathf.Clamp(health.currentValue + damage, health.minValue, health.maxValue);
    }

    public void UseStamina(float amount)
    {
        stamina.currentValue = Mathf.Clamp(stamina.currentValue + amount, stamina.minValue, stamina.maxValue);
    }

    public void SetHealth(float value)
    {
        health.currentValue = Mathf.Clamp(health.currentValue, health.minValue, health.maxValue);
    }

    public void SetStamina(float value)
    {
        stamina.currentValue = Mathf.Clamp(stamina.currentValue, stamina.minValue, stamina.maxValue);
    }

    #endregion

}
