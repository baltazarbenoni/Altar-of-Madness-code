using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class DisableEnemy : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public bool enemyIsParalyzed;
    public bool enemyIsDead;
    private bool currentlyParalyzed;
    private SoldierBehaviour behaviourScript;
    private SoldierPatrol patrolScript;
    private SoldierAttack attackScript;
    private SoldierChase chaseScript;
    private PriestMovement movementScript;
    private PriestMagic magicScript;
    //Current paralysis-time.
    private float paralysisTime;
    //How long the enemy remains paralyzed.
    [SerializeField] public float paralysisTimeLimit;
    //A boolean to indicate whether the enemy in question is a priest or a patrolling soldier.
    [SerializeField] bool thisIsPatrolEnemy;
    [SerializeField] private GameObject enemyHitByMagicVFX;

    [SerializeField] ParticleSystem bloodDeathOne;
    [SerializeField] ParticleSystem bloodDeathTwo;
    private Vector3 paralysisVFXInitialPos; 
    private Vector3 VFXPosition;
    [SerializeField] private float magicPullSpeed;
    private bool enemyNotInRingOvFire;
    void Start()
    {
        currentlyParalyzed = false;
        if(thisIsPatrolEnemy)
        { InitializeSoldier(); }
        else { InitializePriest(); }

        bloodDeathOne = bloodDeathOne.GetComponent<ParticleSystem>();
        bloodDeathTwo = bloodDeathTwo.GetComponent<ParticleSystem>();
        paralysisVFXInitialPos = enemyHitByMagicVFX.transform.position;

    }
    void Update()
    {
        paralysisTime = enemyIsParalyzed? paralysisTime + Time.deltaTime : 0;

        if(enemyIsParalyzed != currentlyParalyzed)
        {
            ParalyzeOrUnparalyzeEnemy(enemyIsParalyzed);
        }
        if(enemyIsDead) { KillEnemy(); }

        if(paralysisTimeLimit < paralysisTime)
        {
            enemyIsParalyzed = false;
            ParalyzeOrUnparalyzeEnemy(enemyIsParalyzed);
        }
        if(enemyIsParalyzed)
        {
            DrawEnemyToRingOfFire();
        }
    }
    void InitializePriest()
    {
        movementScript = GetComponent<PriestMovement>();
        magicScript = GetComponent<PriestMagic>();
    }
    void InitializeSoldier()
    {
        behaviourScript = GetComponent<SoldierBehaviour>();
        patrolScript = GetComponent<SoldierPatrol>();
        attackScript = GetComponent<SoldierAttack>();
        chaseScript = GetComponent<SoldierChase>();
    }
    private void ParalyzeOrUnparalyzeEnemy(bool isParalyzed)
    {
        if(thisIsPatrolEnemy)
        { DisableOrEnableSoldier(isParalyzed); }

        else
        { DisableOrEnablePriest(isParalyzed); }

        PlayVisualEffectOnParalysis(isParalyzed);
        currentlyParalyzed = isParalyzed;
    }
    private void DisableOrEnableSoldier(bool isParalyzed)
    {
        behaviourScript.enabled = !isParalyzed;
        patrolScript.enabled = !isParalyzed;
        chaseScript.enabled = !isParalyzed;
        attackScript.enabled = !isParalyzed;
    }
    private void DisableOrEnablePriest(bool isParalyzed)
    {
        if(isParalyzed)
        {
            magicScript.PriestParalyzedWhileAttacking();
        }
        movementScript.enabled = !isParalyzed;
        magicScript.enabled = !isParalyzed;
    }
    private void PlayVisualEffectOnParalysis(bool enemyIsParalyzed)
    {
        if(enemyIsParalyzed)
        {
            InvokeVisualEffect();
        }
        else
        {
            EndParalysisVFX();
        }
    }
    private void InvokeVisualEffect()
    {
        if(thisIsPatrolEnemy)
        {
            VFXPosition = behaviourScript.GetPatrolFeetPosition() == Vector3.zero ?
                            enemyHitByMagicVFX.transform.position : behaviourScript.GetPatrolFeetPosition();
            enemyHitByMagicVFX.transform.parent = null;
            enemyHitByMagicVFX.transform.position = VFXPosition;
        }
        enemyHitByMagicVFX.SetActive(true);
    }
    private void EndParalysisVFX()
    {
        enemyHitByMagicVFX.SetActive(false);

        if(thisIsPatrolEnemy)
        {
            enemyHitByMagicVFX.transform.parent = this.transform;
            enemyHitByMagicVFX.transform.position = paralysisVFXInitialPos;
        }
    }
    private void KillEnemy()
    {
        enemyHitByMagicVFX.SetActive(false);
        if (thisIsPatrolEnemy)
        { Actions.Alarm(false, patrolScript); }

        else if(magicScript.priestIsMagicking)
        { magicScript.EndAttack(); }

        Instantiate(bloodDeathOne, transform.position, Quaternion.identity);
        Instantiate(bloodDeathTwo, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }
    private void DrawEnemyToRingOfFire()
    {
        enemyNotInRingOvFire = Mathf.Abs(transform.position.x - VFXPosition.x) > 0.2f;
        if(enemyNotInRingOvFire)
        {
            Vector3 pullVector = VFXPosition.x > transform.position.x ? Vector3.right : Vector3.left;
            transform.position += pullVector * magicPullSpeed * Time.deltaTime;
        }
    }
}
