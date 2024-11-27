using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class VFXRateChange : MonoBehaviour
{
    private VisualEffect VFXGraph;
    [SerializeField] private GameObject player;
    private int currentSanity;
    private int initialSanity;
    void Start()
    {
        initialSanity = player.GetComponent<PlayerSanity>().playerSanityPoints;
        Debug.Log("Initial sanity : " + initialSanity);
        currentSanity = initialSanity;
        Actions.PlayerLosesSanity += UpdateVFX;
        Actions.IncreaseSanity += ResetFullSanity;
        AdjustVisualEffectRate(currentSanity);
    }

    void AdjustVisualEffectRate(int currentSanity)
    {
        int newRate = GetVFXRateFromSanity(currentSanity);
        VFXGraph = GetComponent<VisualEffect>();
        VFXGraph.SetInt("Rate", newRate);
        Debug.Log(newRate);
    }
    void ResetFullSanity()
    {
        currentSanity = initialSanity; 
        AdjustVisualEffectRate(currentSanity);
    }
    void UpdateVFX(int sanityChange)
    {
        currentSanity -= sanityChange;
        AdjustVisualEffectRate(currentSanity);
    }
    int GetVFXRateFromSanity(int x)
    {
        if(x <= 0) { return 500; }
        int seedValue = 10 - x;
        double newRate = Mathf.Pow(seedValue, 3) / 2;
        int intRate = (int)newRate;
        return intRate;
    }
}

