using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer thisMixer;
    [SerializeField] private AudioSource audioSource;

    void Update()
    {
        
    }
    public void OnChangeSlider(float newValue)
    {
        thisMixer.SetFloat("Volume", Mathf.Log10(newValue) * 20);


    }
}
