using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class DisablePlayerMovement : MonoBehaviour
{
    private PlayerController movementScript;
    private bool playerDisableOwnMagic;
    private bool playerDisabledPriestMagic;

    void Start()
    {
        Actions.PlayerMovementDisabledByOwnMagic += PlayerMovementDisabledByOwnMagic;
        Actions.PlayerMovementDisabledByPriest += PlayerMovementDisabledByPriest;
        movementScript = GetComponent<PlayerController>();
    }

private void PlayerMovementDisabledByOwnMagic(bool cantMove)
    {
        playerDisableOwnMagic = cantMove;
        EnableOrDisableMovement();
    }
    private void PlayerMovementDisabledByPriest(bool cantMove)
    {
        playerDisabledPriestMagic = cantMove;
        EnableOrDisableMovement();
    }

    private void EnableOrDisableMovement()
    {
        if(playerDisabledPriestMagic || playerDisableOwnMagic)
        {
            movementScript.MovementDisabled = true;
        }
        else
        {
            movementScript.MovementDisabled = false;
        }
    }
}
