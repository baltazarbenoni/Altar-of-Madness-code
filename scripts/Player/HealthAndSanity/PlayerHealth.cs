using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] int playerLives;
	[SerializeField] int playerHealth;
	[SerializeField] int hostageHealth;
	[SerializeField] int maxHealth;
	//The position in the beginning of the level where the player spawns to.
	public RespawnManager respawnManager;

	[SerializeField] ParticleSystem playerBloodOne;
	[SerializeField] ParticleSystem playerBloodTwo;
	[SerializeField] ParticleSystem hostageBloodOne;
	[SerializeField] ParticleSystem hostageBloodTwo;

	void Start()
	{
		playerBloodOne = playerBloodOne.GetComponent<ParticleSystem>();
		playerBloodTwo = playerBloodTwo.GetComponent<ParticleSystem>();
		hostageBloodOne = hostageBloodOne.GetComponent<ParticleSystem>();
		hostageBloodTwo = hostageBloodTwo.GetComponent<ParticleSystem>();
		Actions.SanityZeroDeath += PlayerDead;
	}
	//Increase the health of player.
	private void GainHealth(int targetDeterminant)
	{
		//If either player or hostage have maxHealth and one of them doesn't, automatically increase the health of
		//the one that is less than maxHealth.
		bool healthCheck(int x) => x >= maxHealth;
		int healthGain(int x) => x++;
		int determineToWhom(bool x) => x ? healthGain(playerHealth) : healthGain(hostageHealth);

		if (healthCheck(playerHealth) ^ healthCheck(hostageHealth))
		{
			determineToWhom(healthCheck(hostageHealth));
		}
		//Only if neither have maxHealth, consider the assignement. 0 is for player and 1 for hostage.
		else if (targetDeterminant == 0) { playerHealth++; }

		else { hostageHealth++; }
	}
	//Subtract from playerHealth. 0 is for player and 1 for hostage.
	private void LoseHealth(bool targetDeterminant)
	{
		if (targetDeterminant)
		{ playerHealth--; playerBloodOne.Play(); playerBloodTwo.Play(); }
		else { hostageHealth--; hostageBloodOne.Play(); hostageBloodTwo.Play(); }

		if (playerHealth <= 0 || hostageHealth <= 0) { PlayerDead(); }
	}
	//If player hits a health item, increase health.
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("HealthItem"))
		{
			GainHealth(0);
		}

		if (col.CompareTag("EnemyWeapon"))
		{
			bool attackHitsPlayer = WhoSuffersDamage(col.gameObject.transform.position.x);
			LoseHealth(attackHitsPlayer);
			Actions.PlayerOrHostageLosesHealth(attackHitsPlayer);
		}
	}
	//Check if player is facing the attacking enemy. This depends on which direction the player is facing.
	bool WhoSuffersDamage(float enemyPositionX)
	{
		bool attackHitsPlayer;

		if (transform.localScale.x > 0f)
		{ attackHitsPlayer = enemyPositionX > transform.position.x; }
		else
		{ attackHitsPlayer = enemyPositionX < transform.position.x; }

		return attackHitsPlayer;
	}
	//When player dies, check if gameover. If not so, respawn player. 
	private void PlayerDead()
	{
		Actions.PlayerDead();
		playerLives--;
		if (playerLives <= 0)
		{
			Actions.GameOver();
		}
		else
		{
			// Reset health on respawn.
			playerHealth = maxHealth;
			hostageHealth = maxHealth;
			respawnManager.RespawnPlayer(this.gameObject);
		}
	}
}
