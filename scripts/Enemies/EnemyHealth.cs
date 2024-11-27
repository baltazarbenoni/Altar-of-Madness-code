using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int enemyHealth;
    [SerializeField] private int frontsideDamage;
    [SerializeField] private int backsideDamage; 
    private bool enemyFacingRight;
    [SerializeField] private GameObject Player;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("PlayerWeapon"))
        {
            EnemyDamage();
        }
    }
    private void EnemyDamage()
    {
        int damage = DetermineIfHitInBack() ? backsideDamage : frontsideDamage;
        enemyHealth -= damage;
        if(enemyHealth < 0) { EnemyDead(); }
    }
    private bool DetermineIfHitInBack()
    {
        enemyFacingRight = transform.right == Vector3.right;

		Func<float, bool> attackFromBehindCondition = enemyFacingRight ?
            (x => x < transform.position.x)
			: (x => x > transform.position.x); 
        return attackFromBehindCondition(Player.transform.position.x);
    }
    private void EnemyDead()
    {
        Destroy(this);
    }
}
