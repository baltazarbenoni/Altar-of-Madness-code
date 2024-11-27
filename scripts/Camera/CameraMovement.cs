using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    //Indicates how far the player can drift from the center of the camera before the camera starts moving.
    [SerializeField] private float differenceToStartMoving;
    //Indicates when the camera has gotten close to the player again and can stop moving.
    [SerializeField] private float differenceToStopMoving;
    //Speed of the camera-movement.
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float cameraDistance;
    [SerializeField] private bool startMovingCamera;
    [SerializeField] private bool shouldMoveVertical;
    [SerializeField] private bool shouldMoveHorizontal;
    private bool keepMovingCamera;
    //Variables indicating the difference between the player's position to the camera's.
    private float xDifference;
    private float yDifference;
    private Vector3 playerDeadCameraOffset;
    [SerializeField] private float cameraOffsetMagnitude;
    void Start()
    {
        transform.position = new Vector3(playerPosition.position.x, playerPosition.position.y, cameraDistance);
        Actions.PlayerDead += PlayerDeadCameraShake;
        playerDeadCameraOffset = Vector3.zero;
    }
    void Update()
    {
        startMovingCamera = ShouldCameraStartMoving();
        if(startMovingCamera || keepMovingCamera)
        {
            CameraMovementMethod();
        }

        keepMovingCamera = ShouldCameraKeepMoving(); 
    }
    private bool ShouldCameraStartMoving()
    {
        //Calculate the difference between player and camera positions.
        xDifference = playerPosition.position.x - transform.position.x;
        yDifference = playerPosition.position.y - transform.position.y;
        //Compare the differences to the 'differenceToStartMoving' -value.
        shouldMoveHorizontal = Mathf.Abs(xDifference) > differenceToStartMoving;
        shouldMoveVertical = Mathf.Abs(yDifference) > differenceToStartMoving;
        return shouldMoveHorizontal || shouldMoveVertical;
    }
    private bool ShouldCameraKeepMoving()
    {
        //If the camera has gotten close enough, stop moving it.
        bool keepMovingY = Mathf.Abs(yDifference) > differenceToStopMoving;
        bool keepMovingX = Mathf.Abs(xDifference) > differenceToStopMoving;

        return keepMovingX || keepMovingY; 
    }
    private void CameraMovementMethod()
    {
        Vector3 directionVector = new Vector3(xDifference, yDifference, 0);
        Vector3 normalizedDirectionVector = Vector3.Normalize(directionVector) * cameraSpeed * Time.deltaTime;
        transform.position += normalizedDirectionVector + playerDeadCameraOffset;
    }
    private void PlayerDeadCameraShake()
    {
        StartCoroutine(MakeCameraShake());
    }
    IEnumerator MakeCameraShake()
    {
        float i;
        for(i = 0; i < cameraOffsetMagnitude; i+=0.1f)
        {
            float x = cameraOffsetMagnitude - i;
            float offset = Mathf.Sin(10 * x) * x * Time.deltaTime * 10;
            playerDeadCameraOffset.y = offset > 4 ? 3f : offset;
            transform.position += playerDeadCameraOffset;
            Debug.Log(playerDeadCameraOffset.y);
            yield return null;
        }
        playerDeadCameraOffset = Vector3.zero;
    }
}

