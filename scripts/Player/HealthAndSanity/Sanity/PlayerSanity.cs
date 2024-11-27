using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

//Script to hold count of the player's sanity and make player-GO twitch if it has lost it's sanity.
public class PlayerSanity : MonoBehaviour
{
    [SerializeField] public int playerSanityPoints;
    [SerializeField] private int sanityLimitToDeath;
    private int initialSanity;
    private bool waitWithSwitchedKeys;
    private float keySwitchTimerStart;
    private float keySwitchDuration;
    [SerializeField] private float keySwitchDurationMultiplication;
   
    
    void Start()
    {
        Actions.PlayerLosesSanity += DecreaseSanity;
        Actions.IncreaseSanity += IncreaseSanity;
        initialSanity = playerSanityPoints;
    }
    void Update()
    {
        if(waitWithSwitchedKeys)
        {
            WaitAndReturnKeysToNormal();
        }
    }
    private void DecreaseSanity(int sanityLoss)
    {
        playerSanityPoints -= sanityLoss;
        if(playerSanityPoints <= sanityLimitToDeath)
        {
            Actions.SanityZeroDeath();
            playerSanityPoints = initialSanity;
        }
        else if(playerSanityPoints < initialSanity / 2)
        {
            PlayerLosesHisMind();
        }
    }
    private void IncreaseSanity()
    {
        playerSanityPoints += initialSanity; 
        if(waitWithSwitchedKeys)
        { ReturnKeysToNormal(); }
    }
    private void PlayerLosesHisMind()
    {
        //If keys are already switched, increase the current waiting time.
        if(waitWithSwitchedKeys)
        { keySwitchDuration += GetKeySwitchDuration(); }
        else
        {
            keySwitchDuration = GetKeySwitchDuration();
            SwitchKeys();
        }
    }
    private void SwitchKeys()
    {
        Actions.PlayerGoingCrazy(true);
        keySwitchTimerStart = Time.timeSinceLevelLoad;
        waitWithSwitchedKeys = true;
    }
    private void WaitAndReturnKeysToNormal()
    {
        if(Time.timeSinceLevelLoad - keySwitchTimerStart > keySwitchDuration)
        {
            ReturnKeysToNormal();
        }
    }
    private void ReturnKeysToNormal()
    {
        waitWithSwitchedKeys = false;
        Actions.PlayerGoingCrazy(false);
        keySwitchDuration = 0;
    }
    private float GetKeySwitchDuration()
    {
        if(playerSanityPoints < initialSanity / 10)
        { return initialSanity * keySwitchDurationMultiplication; }

        else if(playerSanityPoints < initialSanity / 6)
        { return (initialSanity - playerSanityPoints) * keySwitchDurationMultiplication; }

        else if(playerSanityPoints < initialSanity / 3)
        { return (initialSanity - playerSanityPoints) * keySwitchDurationMultiplication; }

        else
        { return (initialSanity - playerSanityPoints) * keySwitchDurationMultiplication; }
    }
}
