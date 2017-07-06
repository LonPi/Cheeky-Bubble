using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource
        bearFxSource,
        birdFxSource,
        uiFxSource,
        bubbleFxSource,
        shopFxSource,
        bgmSource;
    public AudioClip
        bubblePopBlank,
        bubblePopCatch,
        chickenCatch,
        penguinCatch,
        birdRunAway,
        bearHappy,
        bearAngry,
        buttonClick,
        timeOut,
        shopBuy;

    public static SoundManager Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void BearPlayOneShot(AudioClip clip)
    {
        bearFxSource.PlayOneShot(clip);
    }

    public void BirdPlayOneShot(AudioClip clip)
    {
        birdFxSource.PlayOneShot(clip);
    }

    public void UiPlayOneShot(AudioClip clip)
    {
        uiFxSource.PlayOneShot(clip);
    }

    public void BubblePlayOneShot(AudioClip clip)
    {
        bubbleFxSource.PlayOneShot(clip);
    }

    public void ShopPlayOneShot(AudioClip clip)
    {
        shopFxSource.PlayOneShot(clip);
    }
}