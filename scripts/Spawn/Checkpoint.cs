using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class Checkpoint : MonoBehaviour
{
    private bool playerHasReachedCheckpointBefore;
    private void Start()
    {
        playerHasReachedCheckpointBefore = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(!playerHasReachedCheckpointBefore)
            {
                RespawnManager.Instance.SetCheckpoint(transform.position);
                Actions.IncreaseSanity();
                playerHasReachedCheckpointBefore = true;
            }
        }
    }
}
