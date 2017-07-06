using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmLoop : MonoBehaviour {

    public AudioClip bgmStart;
    public AudioClip bgmLoop;
    AudioSource _audio;

    public static BgmLoop Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
        _audio = GetComponent<AudioSource>();
        StartCoroutine(playBgm());
	}
	
	IEnumerator playBgm()
    {
        _audio.clip = bgmStart;
        _audio.Play();
        yield return new WaitForSeconds(_audio.clip.length - 0.6f);
        _audio.clip = bgmLoop;
        _audio.Play();
    }
}
