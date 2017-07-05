using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgmMute : MonoBehaviour {
    public Sprite mute, unmute;
    AudioSource soundSource;
    Button btn;
    bool muted;

    // Use this for initialization
    void Start () {
        soundSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        btn = gameObject.GetComponent<Button>();
        if (soundSource.volume == 1)
        {
            btn.image.overrideSprite = unmute;
            muted = false;
        }
        else
        {
            btn.image.overrideSprite = mute;
            muted = true;
        }
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TaskOnClick();
        }
    }
    
    public void TaskOnClick()
    {
        if (muted)
        {
            soundSource.volume = 1;
            btn.image.overrideSprite = unmute;
            muted = false;
        }
        else
        {
            soundSource.volume = 0;
            btn.image.overrideSprite = mute;
            muted = true;
        }
    }
}
