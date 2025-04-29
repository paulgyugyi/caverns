using System;
using System.Collections;
using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    public AudioSource[] audioSource = new AudioSource[2];
    public AudioClip playerIdleClip = null;
    public AudioClip playerAttackClip = null;
    public AudioClip playerBuffedClip = null;
    public AudioClip playerUnhealthyClip = null;
    public AudioClip enemyIdleClip = null;
    public AudioClip enemyAttackClip = null;
    public AudioClip enemyBuffedClip = null;
    public AudioClip enemyUnhealthyClip = null;
    public AudioClip playerTravelClip = null;
    public bool randomMusic = false;
    bool keepPlaying = false;
    int sourceIndex = 0;

    public enum MusicContext { PlayerIdle, PlayerAttack, PlayerBuffed, PlayerUnhealthy, EnemyIdle, EnemyAttack, EnemyBuffed, EnemyUnhealthy, PlayerTravel }
    public MusicContext musicContext;

    public MusicContext Context
    {
        get { return musicContext; }
        set
        {
            if (musicContext != value)
            {
                musicContext = value;
                //Debug.Log("Set context to: " + value);
                audioSource[sourceIndex].Stop();
                sourceIndex = 1 - sourceIndex;
                StartMusic(musicContext);
            }
        }
    }

    void Start()
    {
        musicContext = MusicContext.PlayerTravel;
        StartDynamicMusic();
    }

    void Update()
    {
    }

    void StartDynamicMusic()
    {
        if (!keepPlaying)
        {
            keepPlaying = true;
            StartCoroutine(RepeatedClips());
        }
    }

    void StopDynamicMusic()
    {
        keepPlaying = false;
    }

    void StartMusic(MusicContext musicContext)
    {
        float volume = 0f;

        //Debug.Log("Queuing music for context: " + musicContext.ToString());
        if (randomMusic)
        {
            audioSource[sourceIndex].pitch = 1.0f;
            volume = 0.0625f;
        }
        else
        {
            audioSource[sourceIndex].pitch = GetPitch(musicContext);
            volume = GetVolume(musicContext);
        }
        AudioClip audioClip = null;
        switch (musicContext)
        {
            case MusicContext.PlayerIdle:
                audioClip = playerIdleClip;
                break;
            case MusicContext.PlayerAttack:
                audioClip = audioClip = playerAttackClip;
                break;
            case MusicContext.PlayerBuffed:
                audioClip = playerBuffedClip;
                break;
            case MusicContext.PlayerUnhealthy:
                audioClip = playerUnhealthyClip;
                break;
            case MusicContext.EnemyIdle:
                audioClip = enemyIdleClip;
                break;
            case MusicContext.EnemyAttack:
                audioClip = enemyAttackClip;
                break;
            case MusicContext.EnemyBuffed:
                audioClip = enemyBuffedClip;
                break;
            case MusicContext.EnemyUnhealthy:
                audioClip = enemyUnhealthyClip;
                break;
            case MusicContext.PlayerTravel:
                audioClip = playerTravelClip;
                break;
            default:
                audioClip = playerIdleClip;
                break;
        }
        if (audioClip != null)
        {
            audioSource[sourceIndex].PlayOneShot(audioClip, volume);
        }
        else
        {
            Debug.LogWarning("Audio clip is null for " + musicContext);
        }
    }
    float GetVolume(MusicContext context)
    {
        float volume = 0.125f; ;

        switch (context)
        {
            case MusicContext.PlayerIdle:
                volume = 0.0625f;
                break;
        }
        return volume;
    }


    float GetPitch(MusicContext context)
    {
        float pitch = 1.0f;

        switch (context)
        {
            case MusicContext.PlayerBuffed:
                pitch = 1.5f;
                break;
            case MusicContext.PlayerUnhealthy:
                pitch = 0.5f;
                break;
            case MusicContext.EnemyBuffed:
                pitch = 1.5f;
                break;
            case MusicContext.EnemyUnhealthy:
                pitch = 0.5f;
                break;
            default:
                pitch = 1.0f;
                break;
        }
        return pitch;
    }

    MusicContext PickRandomContext(MusicContext context)
    {
        MusicContext nextContext = context;
        if (UnityEngine.Random.Range(0,2) == 0)
        {
            var values = Enum.GetValues(typeof(MusicContext));
            int randomIndex = UnityEngine.Random.Range(0, values.Length);
            nextContext = (MusicContext)values.GetValue(randomIndex);

        }
        return nextContext;
    }
    IEnumerator RepeatedClips()
    {
        while (keepPlaying)
        {
            yield return new WaitUntil(() => !audioSource[sourceIndex].isPlaying);
            if (audioSource[sourceIndex].time == 0f)
            {
                audioSource[sourceIndex].Stop();
                sourceIndex = 1 - sourceIndex;

                if (randomMusic)
                {
                    musicContext = PickRandomContext(musicContext);
                }
                StartMusic(musicContext);
            }
        }
    }
}
