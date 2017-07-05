using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource
        bearFxSource,
        birdFxSource,
        uiFxSource,
        miscFxSource,
        shopFxSource;
    public AudioClip
        bubbleBlow,
        bubblePop,
        birdCaught,
        birdRunAway,
        birdFall,
        bearHappy,
        bearAngry,
        bearShock,
        bearFail,
        buttonClick,
        timeOut,
        newHighScore;

    public static SoundManager Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void PlayerPlayOneShot(AudioClip clip)
    {
        bearFxSource.PlayOneShot(clip);
    }

    public void EnemyPlayOneShot(AudioClip clip)
    {
        birdFxSource.PlayOneShot(clip);
    }

    public void UiPlayOneShot(AudioClip clip)
    {
        uiFxSource.PlayOneShot(clip);
    }

    public void MiscPlayOneShot(AudioClip clip)
    {
        miscFxSource.PlayOneShot(clip);
    }

    public void BuffPlayOneShot(AudioClip clip)
    {
        shopFxSource.PlayOneShot(clip);
    }
}