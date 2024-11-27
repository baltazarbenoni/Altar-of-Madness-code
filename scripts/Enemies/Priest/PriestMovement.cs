using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PriestMovement : MonoBehaviour
{
    [SerializeField] private float priestMovementSpeed;
    public int priestGazeDirection;
    [SerializeField] private int priestMovementDirection;
    [SerializeField] private float noticingDistance;
    private SpriteRenderer priestSprite; 
    //Can the priest move or is it attacking.
    private bool movementEnabled;
    private LayerMask groundLayer;
    private LayerMask playerLayer;
    private PriestMagic priestMagicScript;
    [SerializeField] private Transform playerPosition;
    void Start()
    {
        priestSprite = GetComponent<SpriteRenderer>();
        groundLayer = LayerMask.GetMask("Ground");
        playerLayer = LayerMask.GetMask("Player");
        priestMagicScript = GetComponent<PriestMagic>();
        movementEnabled = true;
    }
    void Update()
    {
        //When movement is not enabled, make the priest kinematic and vice versa.
        priestGazeDirection = CheckPriestDirection() ? 1 : -1;
        movementEnabled = !priestMagicScript.priestShouldAttack;
        //If priest is not attacking and can move, make it move!
        if(movementEnabled)
        {
            PriestPatrolling();
        }
        else
        {
            CheckIfPlayerBehindPriest();
        }
    }
    //Determine movement direction and move priest.
    private void PriestPatrolling()
    {
        DeterminePriestDirection();
        PriestMoving();
    }
    //Check if the priest is on the edge of the plaftorm and should rotate.
    //If so, rotate priest and update 'priestMovementDirection'.
    private void DeterminePriestDirection()
    {
        if(CheckIfOnEdge() || CheckIfPlayerBehindPriest() || CheckIfInFrontOfWall())
        {
            priestMovementDirection = CheckPriestDirection() ? 1 : -1;
            return;
        }
    }
    //Move this priest.
    private void PriestMoving()
    {
        priestMovementDirection = priestGazeDirection;
        transform.position += Vector3.right * priestMovementDirection * priestMovementSpeed * Time.deltaTime;
    }
    //Check whether player is behind the priest and the priest should rotate.
    private bool CheckIfPlayerBehindPriest()
    {
        bool playerVisible = Mathf.Abs(transform.position.x - playerPosition.position.x) < noticingDistance;
        Debug.Log("Player visible : " + playerVisible);
        bool playerBehindPriest = priestGazeDirection > 0 ?
            transform.position.x > playerPosition.position.x : transform.position.x < playerPosition.position.x;
        Debug.Log("Player behind : " + playerBehindPriest);
        if(playerVisible && playerBehindPriest)
        {
            RotatePriest();
            return true;
        }
        else { return false; }
    }
    private bool CheckIfInFrontOfWall()
    {
        bool turnAround = Physics2D.Raycast(transform.position, transform.right, 2, groundLayer);
        if(turnAround)
        {
            RotatePriest();
            return true;
        }
        else { return false; }
    }
    //Cast a ray to check if there is ground in front of the priest. 
    private bool CheckIfOnEdge()
    {
        float raycastY = priestSprite.size.y / 2 + 0.5f;
        Vector3 raycastDirection = new Vector3(2f * priestGazeDirection, -raycastY, 0);
        bool priestNotOnEdge = Physics2D.Raycast(transform.position, raycastDirection, 5, groundLayer) ||
            Physics2D.Raycast(transform.position, raycastDirection, 5, playerLayer);
        if(!priestNotOnEdge)
        {
            RotatePriest();
            return true;
        }
        else
        {
            return false;
        }
    }
    //Check which way the priest is facing.
    private bool CheckPriestDirection()
    {
        if(transform.right == Vector3.right)
        {
            return true;
        }
        else { return false; }
    }
    public void RotatePriest()
    {
        Vector3 rotationDirection = CheckRotationDirection();
        transform.rotation *= Quaternion.FromToRotation(transform.right, rotationDirection);
    }
    private Vector3 CheckRotationDirection()
    {
        Vector3 rotationDirection = CheckPriestDirection() ? Vector3.left : Vector3.right;
        return rotationDirection;
    }
    public Vector3 GetPriestFeetPosition()
    {
        RaycastHit2D feetRaycastHit = Physics2D.Raycast(transform.position, Vector3.down, 5, groundLayer);
        if(feetRaycastHit.collider != null)
        {
            Vector3 feetPosition = feetRaycastHit.point;
            return feetPosition;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
