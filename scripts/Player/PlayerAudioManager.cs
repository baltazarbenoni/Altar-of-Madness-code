using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource movementAudioPlayer;
    //[SerializeField] private AudioSource jumpingAudioPlayer;
    [SerializeField] private AudioSource healthAudioPlayer;
    [SerializeField] private AudioClip playerHurt;
    [SerializeField] private AudioClip hostageHurt;
    private bool playerMovingPreviousFrame;
    private bool playerInAir;
    private bool playerInAirPreviousFrame;
    void Start()
    {
        Actions.PlayerOrHostageLosesHealth += PlayHurtAudio;
        healthAudioPlayer.loop = false;
    }
    void Update()
    {
        JumpingAudioManager();
        MovementAudioManager();

    }
    void LateUpdate()
    {
        playerMovingPreviousFrame = AudioControlVariables.playerMovingHorizontal;
        playerInAirPreviousFrame = playerInAir;
    }
    //If player moving variable is different from what is was, start, playing footsteps, or stop them.
    void MovementAudioManager()
    {
        bool changeAudioState = playerMovingPreviousFrame != AudioControlVariables.playerMovingHorizontal;
        if(changeAudioState)
        { WalkingOrLandingAudio(); }
    }
    //If player walks, loop step sounds. Else, stop step sounds.
    void WalkingOrLandingAudio()
    {
        if(AudioControlVariables.playerMovingHorizontal)
        { AdjustAudioSourceSettings(movementAudioPlayer, true); }

        else if(!AudioControlVariables.playerMovingHorizontal)
        { movementAudioPlayer.Stop(); }
    }
    //Make certain audioSource play, used either for walking or jumping audioSource.
    void AdjustAudioSourceSettings(AudioSource audioSource, bool isLooping)
    {
        audioSource.loop = isLooping;
        audioSource.Play();
    }
    //Play health audio depending on who is hurt, player or his wretched bro.
    void PlayHurtAudio(bool playerDamage)
    {
        healthAudioPlayer.Stop();
        healthAudioPlayer.clip =  playerDamage ? playerHurt : hostageHurt;
        healthAudioPlayer.Play();
    }
    //If the player is in air and was not so in the previous frame, play jumping audio.
    void JumpingAudioManager()
    {
        playerInAir = AudioControlVariables.playerInAir;
        if(playerInAir != playerInAirPreviousFrame)
        {
            AbortMovementAudioOnJump();
            //PlayJumpingAudio();
        }       
    }
    void AbortMovementAudioOnJump()
    {
        if(playerInAir)
        {
            movementAudioPlayer.Pause();
        }
        else
        {
            WalkingOrLandingAudio();
        }

    }
    /*void PlayJumpingAudio()
    {
        if(playerInAir)
        { AdjustAudioSourceSettings(jumpingAudioPlayer, false); }
    }*/
}
