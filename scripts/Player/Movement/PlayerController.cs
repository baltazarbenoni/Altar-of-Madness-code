using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float speed;
	[SerializeField] float jumpForce;
	float moveInput;
	bool jumpOn;
	public bool hitGround;

	[SerializeField] Transform groundRayObject;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] float groundRayRadius;
	public bool MovementDisabled;

	[SerializeField] ParticleSystem grass;

	Rigidbody2D rb;
	private float keySwitchVar;

	void Start()
	{
		jumpOn = false;
		rb = GetComponent<Rigidbody2D>();
		grass = grass.GetComponent<ParticleSystem>();
		Actions.PlayerGoingCrazy += UpdateKeySwitchMultiplicator;
		keySwitchVar = 1;
	}

	void InputAndGroundCheck()
	{
		moveInput = MovementDisabled ? 0 : Input.GetAxis("Horizontal") * keySwitchVar;
		rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

		//In order to run the player movement audio effects, introduced a boolean variable to the script.
		AudioControlVariables.playerMovingHorizontal = moveInput != 0;
		//Debug.Log(AudioControlVariables.playerMovingHorizontal);

		bool hitGround = Physics2D.Raycast(groundRayObject.transform.position, -Vector2.up);
		// Debug.DrawRay(groundRayObject.transform.position, -Vector2.up * hitGround.distance, Color.red);

		if (Physics2D.OverlapCircle(groundRayObject.position, groundRayRadius, groundLayer))
		{
			hitGround = true;
		}
		else
		{
			hitGround = false;
		}

		if (hitGround)
		{
			if (!jumpOn)
			{
				grass.Play();
			}
			jumpOn = true;
			//Change the jumping audio variable to false when player lands on ground.
			AudioControlVariables.playerInAir = false;
			Debug.Log(AudioControlVariables.playerInAir);
		}
		else
		{
			jumpOn = false;
		}
	}

	void Update()
	{
		InputAndGroundCheck();
		Jump();
		Gravity();
		Flip();
	}

	void Jump()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (jumpOn)
			{
				rb.velocity = Vector2.up * jumpForce;
				//Set the jumping audio variable to true when player jumps.
				AudioControlVariables.playerInAir = true;
				Debug.Log(AudioControlVariables.playerInAir);
				grass.Play();
			}
			else
			{
				return;
			}
		}
	}

	void Gravity()
	{
		Vector3 velocity = rb.velocity;
		if (velocity.y < -0.1f)
		{
			rb.gravityScale = 2.5f;
		}
		else
		{
			rb.gravityScale = 1f;
		}
	}
	private void UpdateKeySwitchMultiplicator(bool keysSwitched)
	{
		keySwitchVar = keysSwitched ? -1f : 1f;
	}

	void Flip()
	{
		if (moveInput < 0)
		{
			transform.localScale = new Vector2(-1, transform.localScale.y);
		}
		else if (moveInput > 0)
		{
			transform.localScale = new Vector2(1, transform.localScale.y);
		}
	}
}
