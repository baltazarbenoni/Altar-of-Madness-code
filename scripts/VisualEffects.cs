using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class VisualEffects : MonoBehaviour
{
    private VisualEffect thisEffect;

    void Awake()
    {
        thisEffect = GetComponent<VisualEffect>();
    }
    void OnEnable()
    {
        if(thisEffect != null)
        {
            thisEffect.Stop();
            thisEffect.Reinit();
            if(!thisEffect.HasAnySystemAwake())
            {
                thisEffect.Play();
            }
        }
    }
    void OnDisable()
    {
        if(thisEffect != null)
        {
            thisEffect.Stop();
        }
    }
}
