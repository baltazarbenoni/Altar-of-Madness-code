using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class IntroMusicManager : MonoBehaviour
{
    [SerializeField] GameObject GameTitle;
    [SerializeField] private AudioSource introSceneMusic;
    private float originalVolume;
    [SerializeField] private float changeSceneTimer;

    void Start()
    {
        originalVolume = introSceneMusic.volume;
        changeSceneTimer = changeSceneTimer > 215f ? changeSceneTimer : 220f;
        Invoke("ChangeScene", 225f);
    }

    private void ChangeScene()
    {
        Debug.Log("Checking if fadeout");
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        for(int i = 0; introSceneMusic.volume > 0.1; i++)
        {
            introSceneMusic.volume = originalVolume - i*0.01f;
            yield return new WaitForSeconds(0.02f);
        }
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
}
