using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackObject;
    private bool attackKeyPressed;
    private bool attackPossible;
    public bool playerAttacks;
    private bool playerIsMagicking;
    private bool playerHitByMagick;
    private float attackTimer;
    void Start()
    {
        attackPossible = true;
        attackObject.SetActive(false);
        Actions.PlayerMovementDisabledByPriest += IsPlayerHitWithMagic;
        Actions.PlayerMovementDisabledByOwnMagic += IsPlayerDoingMagic;
        
    }

    void Update()
    {
        if(!playerAttacks && attackPossible)
        {
            CheckInput();
        }
        else if(playerAttacks)
        {
            UpdateTimerAndCheckIfAttackOver();
        }

    }
    private void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && !playerAttacks)
        {
            playerAttacks = true;
            attackObject.SetActive(true);
        }
    }
    private void UpdateTimerAndCheckIfAttackOver()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer > 0.5f)
        {
            playerAttacks = false;
            attackObject.SetActive(false);
            attackTimer = 0;
        }
    }
    private void UpdateAttackStatus()
    {
        attackPossible = !playerIsMagicking && !playerHitByMagick;
    }
    private void IsPlayerDoingMagic(bool newValue)
    {
        playerIsMagicking = newValue;
        UpdateAttackStatus();
    }
    private void IsPlayerHitWithMagic(bool newValue)
    {
        playerHitByMagick = newValue;
        UpdateAttackStatus();
    }
}
