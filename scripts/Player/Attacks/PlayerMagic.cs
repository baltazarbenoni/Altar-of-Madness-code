using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PlayerMagic : MonoBehaviour
{
    //General attack-fields.
    [SerializeField] private bool aimingHasStarted;
    [SerializeField] private bool attackHasStarted;
    [SerializeField] private float attackTimer;
    [SerializeField] private int sanityDamageByKill;
    private bool playerHitWithEnemyMagic;
    private int magicAttackCounter;
    //Aiming fields.
    [SerializeField] private float maxAimDistance;
    private float aimingTimer;
    [SerializeField] private int resolveAimingTime;
    [SerializeField] private GameObject aimPointer;
    private GameObject instantiatedPointer;
    private float enemyHeightIncrement;
    //Enemy-fields.
    private Vector3 enemyPriestPosition;
    private GameObject selectedEnemy;
    private Collider2D[] attackableEnemies;
    private int selectedEnemyIndex;
    private LayerMask enemyMask;
    //Variables to disable attack and movement.
    private PlayerController movementScript;
    private bool playerMovementDisabledPreviousFrame;
    //Audio to play on magical attack.
    [SerializeField] private AudioSource playerMagicSound;
    [SerializeField] private AudioClip pretzelizeAudio;
    [SerializeField] private AudioClip magicAudio;
    [SerializeField] private float deathAudioIncrement;

    void Awake()
    {
        movementScript = gameObject.GetComponent<PlayerController>();
    }
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
        magicAttackCounter = 0;
        aimingTimer = 0;
        instantiatedPointer = null;
        playerMagicSound.clip = magicAudio;
    }
    void Update()
    {
        PlayerMovementDisabledCheck(attackHasStarted);
        playerHitWithEnemyMagic = IsPlayerHitByMagicAndCantAttack();

        if(playerHitWithEnemyMagic)
        { EndAttack(); }
        else if(aimingHasStarted)
        {
            ChangeTargetEnemy();
            ResolveMagicTimer(); 
            attackTimer = 0;
            MoveAimPointerWithEnemy();
        }
        else if(attackHasStarted)
        {
            CheckInputToPretzelizeEnemy();
            attackTimer += Time.deltaTime;
            ResolveAttackTimer();
            aimingHasStarted = false;
        }
        else
        { CheckInputToStartAiming(); aimingTimer = 0; attackTimer = 0; }
    }
    //Start attack if aiming-key is pressed.
    private void CheckInputToStartAiming()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            selectedEnemyIndex = 0;
            StartAimingAttack(selectedEnemyIndex);
        }
    }
    //Change the selected enemy (if possible) when aiming-key is pressed again. If attack-key is pressed,
    //attack the current enemy.
    private void ChangeTargetEnemy()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            TryIncrementingEnemyIndex();
            ChangeSelectedEnemy(selectedEnemyIndex);
        }

        if(Input.GetKeyDown(KeyCode.C) && magicAttackCounter == 0) { AttackThisEnemy(); }
    }
    //If killing-key is pressed, kill enemy.
    private void CheckInputToPretzelizeEnemy()
    {
        if(Input.GetKeyDown(KeyCode.X) && magicAttackCounter  == 1)
        {
            PretzelizeThisEnemy();
            EndAttack();
        }
    }
    //Method to increment the index of the selected enemy in the list containing attackable enemies.
    private void TryIncrementingEnemyIndex()
    {
        selectedEnemyIndex = selectedEnemyIndex + 1 < attackableEnemies.Length ? selectedEnemyIndex + 1 : 0;
    }
    //If no key is pressed, aiming-mode stops after 'reselveAimingTime'.
    private void ResolveMagicTimer()
    {
        bool somethingIsPressed = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C);
        aimingTimer = somethingIsPressed ? 0 : aimingTimer + Time.deltaTime;

        if(aimingTimer > resolveAimingTime) { EndAttack(); }
    }
    //Check if player is under the attack of some enemy-priest.
    private bool IsPlayerHitByMagicAndCantAttack()
    {
        bool playerHitWithMagic = gameObject.GetComponent<PlayerHitByMagic>().playerTrappedByMagic;
        return playerHitWithMagic;
    }
    //After the allotted time, resolve the attack.
    private void ResolveAttackTimer()
    {
        if(attackTimer > resolveAimingTime)
        { EndAttack(); }
    }
    //Move the aiming pointed with the enemy as it moves in the game.
    private Vector3 MoveAimPointerWithEnemy()
    {
        enemyPriestPosition = selectedEnemy.transform.position;
        enemyHeightIncrement = selectedEnemy.GetComponent<SpriteRenderer>().size.y / 2 + 1f;
        Vector3 pointerPosition = new Vector3(enemyPriestPosition.x, enemyPriestPosition.y + enemyHeightIncrement, 0); 
        if(instantiatedPointer != null) { instantiatedPointer.transform.position = pointerPosition; }
        return pointerPosition;
    }
    //Scan for attackable enemies, store them in the 'attackableEnemies'-list, return the first element
    //of the list.
    private GameObject SearchNearestEnemy(int enemyIndex)
    {
        attackableEnemies = Physics2D.OverlapCircleAll(transform.position, maxAimDistance, enemyMask);
        enemyIndex = enemyIndex < attackableEnemies.Length ? enemyIndex : 0;
        if(attackableEnemies.Length == 0) { return null; }
        else { return attackableEnemies[enemyIndex].gameObject; }
    }
    //Start attack by scanning for enemies and initializing the aiming pointer.
    private void StartAimingAttack(int enemyIndex)
    {
        selectedEnemy = SearchNearestEnemy(enemyIndex);
        if(selectedEnemy == null) { return;}

        Vector3 pointerPosition = MoveAimPointerWithEnemy();
        Quaternion pointerRotation = aimPointer.transform.rotation;
        instantiatedPointer = instantiatedPointer == null ? Instantiate(aimPointer, pointerPosition, pointerRotation)
                                : instantiatedPointer;
        aimingHasStarted = true;
    }
    //After incrementing the index, assign a new enemy to the 'selectedEnemy'-field.
    private void ChangeSelectedEnemy(int enemyIndex)
    {
        GameObject nextEnemy;
        if(enemyIndex > 0) { nextEnemy = attackableEnemies[enemyIndex].gameObject; }
        else { nextEnemy = SearchNearestEnemy(enemyIndex); }
        selectedEnemy = nextEnemy;
        MoveAimPointerWithEnemy();
    }
    private void AttackThisEnemy()
    {
        attackHasStarted = true;
        aimingHasStarted = false;
        magicAttackCounter++;
        ResetAudioSource();
        MakePlayerFaceEnemyOnAttack();
        selectedEnemy.GetComponent<DisableEnemy>().enemyIsParalyzed = true;
        RemovePointer();
    }
    private void ResetAudioSource()
    {
        playerMagicSound.volume = 1f;
        playerMagicSound.clip = magicAudio;
        playerMagicSound.Play();
        playerMagicSound.loop = false;
    }
    private void MakePlayerFaceEnemyOnAttack()
    {
        float xDirectionToEnemy = selectedEnemy.transform.position.x - this.transform.position.x;
        if(transform.right.x * xDirectionToEnemy < 0)
        {
            RotatePlayer();
        }
    }
    private void RotatePlayer()
    {
        bool facingRight = transform.right == Vector3.right;
        Vector3 rotationDirection = facingRight ? Vector3.left : Vector3.right;
        transform.rotation *= Quaternion.FromToRotation(transform.right, rotationDirection);
    }
    //If the enemy is paralyzed, assign it dead.
    private void PretzelizeThisEnemy()
    {
        PlayPretzelizeEffect();
        selectedEnemy.GetComponent<DisableEnemy>().paralysisTimeLimit += deathAudioIncrement;
        Actions.PlayerLosesSanity(sanityDamageByKill);
        Debug.Log("THIS IS PLAYING");
        StartCoroutine(KillEnemyWithDelay());
    }
    IEnumerator KillEnemyWithDelay()
    {
        yield return new WaitForSeconds(deathAudioIncrement - 0.05f);
        if(selectedEnemy.GetComponent<DisableEnemy>().enemyIsParalyzed)
        {
            selectedEnemy.GetComponent<DisableEnemy>().enemyIsDead = true;
        }
    }
    //End attack and remove the instantiated aiming pointer.
    private void EndAttack()
    {
        aimingHasStarted = false;
        attackHasStarted = false;
        magicAttackCounter = 0;
        RemovePointer();
        playerMagicSound.Stop();
    }
    private void RemovePointer()
    {
        if(instantiatedPointer != null) { Destroy(instantiatedPointer); }
    }
    //Make player stop if the character does magic.
    private void PlayerMovementDisabledCheck(bool newValue)
    {
        if(playerMovementDisabledPreviousFrame != newValue)
        {
            Actions.PlayerMovementDisabledByOwnMagic(newValue);
        }

        playerMovementDisabledPreviousFrame = newValue;
    }

    //Play magicalKill audio if player kills enemy with magic.
    void PlayPretzelizeEffect()
    {
        playerMagicSound.clip = pretzelizeAudio;
        playerMagicSound.volume = 0.7f;
        playerMagicSound.Play();
    }
    
}
