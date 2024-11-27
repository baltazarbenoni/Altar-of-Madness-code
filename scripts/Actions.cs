using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//C 2024 Daniel Snapir alias Baltazar Benoni

public static class Actions 
{
    public static Action<int> PlayerLosesSanity;
    public static Action<bool> PlayerOrHostageLosesHealth;
    public static Action PlayerDead;
    public static Action<bool, SoldierPatrol> Alarm;
    public static Action<bool> PlayerMovementDisabledByPriest;
    public static Action<bool> PlayerMovementDisabledByOwnMagic;
    public static Action<bool> PlayerGoingCrazy;
    public static Action IncreaseSanity;
    public static Action SanityZeroDeath;
    public static Action RespawnStartMusicAgain;
    public static Action GameOver;
}
