using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Vector3 currentCheckpoint;

    private void Awake()
    {
        // Ensure there's only one instance of RespawnManager (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetCheckpoint(transform.position);
    }

    // Set the position of the latest checkpoint
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpoint = checkpointPosition;
    }

    // Respawn the player at the latest checkpoint
    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = currentCheckpoint;
        // Additional respawn logic like resetting animations, etc.
        Actions.RespawnStartMusicAgain();
    }

}