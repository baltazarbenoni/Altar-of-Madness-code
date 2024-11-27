using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

class MusicManager : MonoBehaviour
{
    //Different music tracks.
    [SerializeField] private AudioClip forestMusic;
    [SerializeField] private AudioClip templeMusic;
    [SerializeField] private AudioClip templeMusicIntro;
    [SerializeField] private AudioClip alarmMusic;
    [SerializeField] private AudioClip deathMusic;
    private float fadingDelta;
    [SerializeField] private float fadingDeltaInitial;
    private AudioClip musicBeforeAlarm;
    //AudioSource responsible for playing the music.
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource musicPlayer2;
    //The point in the scene where the music chenges to the temple theme.
    [SerializeField] private Transform templeStartingTransform;
    private Vector3 templeMusicStartPosition;
    bool playerInTempleArea;
    [SerializeField] private Transform playerPosition;
    //The song last played or currently playing.
    private AudioClip currentlyPlaying;
    private AudioSource audioSourceToChangeInto;
    private AudioSource currentAudioSource;
    private float playingTime;
    private bool templeIntroPlayed;
    private List<SoldierPatrol> alarmedSoldiers = new();
    [SerializeField] private bool alarmStateOn;
    private bool playerDead;
    
    void Awake()
    {
        InitializeActionListeners();
    }
    
    void Start()
    {
        templeMusicStartPosition = templeStartingTransform.position;
        templeIntroPlayed = false;
        InitializeMusicClips();
        musicPlayer.Play();
    }
    void Update()
    {
        if(!playerDead)
        {
            CheckIfToChange();
            AlarmCheckAndMusicChange();
        }
    }
    private void InitializeActionListeners()
    {
        Actions.Alarm += UpdateAlarmList;
        Actions.PlayerDead += DeathMusic;
        Actions.RespawnStartMusicAgain += RespawnMusicManagement;
        Actions.PlayerMovementDisabledByPriest += AdjustVolumeWhenPlayerHitByMagic; 
    }
    private void InitializeMusicClips()
    {
        currentlyPlaying = forestMusic;
        musicPlayer.clip = forestMusic;
        musicBeforeAlarm = currentlyPlaying;
    }
    IEnumerator FadeSongOutOrIn(bool fadingOut, AudioSource musicPlayer)
    {
        fadingDelta = fadingDeltaInitial;
        float startingValue = 0.01f;
        musicPlayer.volume = fadingOut ? 1 : 0;
        static bool continueIteration(float x, bool y) => y? x > 0.1f : x < 0.95f;
        static double newValue(float x, bool y) => y ? -x + 1 : x;
        bool doubleSpeedEngaged = false;
        for(float i = startingValue; continueIteration(musicPlayer.volume, fadingOut); i += 0.02f)
        {
            musicPlayer.volume = (float) newValue(i, fadingOut);
            yield return new WaitForSeconds(fadingDelta);
            
            if(fadingOut && !doubleSpeedEngaged && musicPlayer.volume < 0.5f)
            { fadingDelta /= 2; doubleSpeedEngaged = true; }
        }
        if(fadingOut) { musicPlayer.Stop();}
    }

    void UpdateAlarmList(bool alarmStarting, SoldierPatrol soldier)
    {
        if(alarmStarting && !alarmedSoldiers.Contains(soldier))
        {
            alarmedSoldiers.Add(soldier);
        }
        if(!alarmStarting && alarmedSoldiers.Contains(soldier))
        {

            alarmedSoldiers.Remove(soldier);
        }
    }

    void AlarmCheckAndMusicChange()
    {
        if(!alarmStateOn)
        {
            musicBeforeAlarm = CheckWhichClipIsCurrentlyPlaying();
        }

        if(alarmedSoldiers.Count > 0 && !alarmStateOn)
        {
            alarmStateOn = true;
            playingTime = currentAudioSource.time;
            ChangeSong(alarmMusic, 0f, true, false);
        }

        else if (alarmedSoldiers.Count == 0 && alarmStateOn)
        {
            alarmStateOn = false;
            if(musicBeforeAlarm == forestMusic)
            {
                musicBeforeAlarm = JumpFromForestToTempleMusic() ? templeMusicIntro : forestMusic; 
            }
            ChangeSong(musicBeforeAlarm, playingTime, true, false);
        }
    }
    private bool JumpFromForestToTempleMusic()
    {
        if(playerInTempleArea)
        { return true; }
        else
        { return false;}
    }

    private void AssignNewSong(AudioClip newSong, float playBackTime)
    {
        audioSourceToChangeInto.clip = newSong;
        audioSourceToChangeInto.time = playBackTime;
    }

    void ChangeSong(AudioClip newSong, float playBackTime, bool doFadeOut, bool doFadeIn)
    {
        CheckWhichAudioSourceIsPlaying();
        AssignNewSong(newSong, playBackTime);

        if(doFadeIn) { StartCoroutine(FadeSongOutOrIn(false, audioSourceToChangeInto)); }
        else { audioSourceToChangeInto.volume = 1; audioSourceToChangeInto.Play(); }

        if(doFadeOut) { StartCoroutine(FadeSongOutOrIn(true, currentAudioSource)); }
        else { currentAudioSource.Pause(); }

        if(!alarmStateOn) { currentlyPlaying = newSong; }
    }
    void CheckWhichAudioSourceIsPlaying()
    {
        audioSourceToChangeInto = musicPlayer.isPlaying ? musicPlayer2 : musicPlayer; 
        currentAudioSource = musicPlayer.isPlaying ? musicPlayer : musicPlayer2;
    }
    AudioClip CheckWhichClipIsCurrentlyPlaying()
    {
        CheckWhichAudioSourceIsPlaying();
        currentlyPlaying = currentAudioSource.clip;
        return currentlyPlaying;
    }
    void HasTempleIntroPlayed()
    {
        CheckWhichClipIsCurrentlyPlaying();
        templeIntroPlayed = currentlyPlaying == templeMusicIntro && currentAudioSource.time > currentlyPlaying.length - 2f;
    }
    void CheckIfToChange()
    {
        currentAudioSource = musicPlayer.isPlaying ? musicPlayer : musicPlayer2;
        IsPlayerInTempleArea();
        HasTempleIntroPlayed();
        if(playerInTempleArea)
        {
            if(currentlyPlaying == forestMusic)
            {
                ChangeSong(templeMusicIntro, 0f, true, false);
            }
            else if(templeIntroPlayed && !alarmStateOn && currentlyPlaying == templeMusicIntro)
            {
                ChangeSong(templeMusic, 0f, true, false);
            }
        }
    }
    private bool IsPlayerInTempleArea()
    {
        playerInTempleArea = playerPosition.position.x > templeMusicStartPosition.x;
        return playerInTempleArea;
    }
    void DeathMusic()
    {
        CheckWhichAudioSourceIsPlaying();
        ChangeSong(deathMusic, 0f, true, false);
    }
    IEnumerator WaitForDeathClip()
    {
        yield return new WaitForSeconds(deathMusic.length);
        musicPlayer.clip = currentlyPlaying;
        musicPlayer.time = playingTime;
        musicPlayer.Play();
        musicPlayer.loop = true;
    }
    private void RespawnMusicManagement()
    {
        AudioClip clipToStartPlaying = IsPlayerInTempleArea() ? templeMusicIntro : forestMusic;
        ChangeSong(clipToStartPlaying, 0f, false, false);
    }
    private void AdjustVolumeWhenPlayerHitByMagic(bool playerParalyzed)
    {
        float volumeChange = playerParalyzed ? -0.35f : 0.35f;
        CheckWhichAudioSourceIsPlaying();
        currentAudioSource.volume += volumeChange;
    }
}