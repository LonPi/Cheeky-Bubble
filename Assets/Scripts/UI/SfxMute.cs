using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SfxMute : MonoBehaviour {
    public Sprite mute, unmute;
    AudioSource bearFx,
        birdFx,
        uiFx,
        bubbleFx,
        shopFx;
    Button btn;
    bool muted;

    // Use this for initialization
    void Start () {
        bearFx = GameObject.Find("BearFX").GetComponent<AudioSource>();
        birdFx = GameObject.Find("BirdFX").GetComponent<AudioSource>();
        uiFx = GameObject.Find("UiFX").GetComponent<AudioSource>();
        bubbleFx = GameObject.Find("BubbleFX").GetComponent<AudioSource>();
        shopFx = GameObject.Find("ShopFX").GetComponent<AudioSource>();
        btn = gameObject.GetComponent<Button>();

        if (bearFx.volume == 1)
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
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        if (muted)
        {
            bearFx.volume = 1;
            birdFx.volume = 1;
            uiFx.volume = 1;
            bubbleFx.volume = 1;
            shopFx.volume = 1;
            btn.image.overrideSprite = unmute;
            muted = false;
        }
        else
        {
            bearFx.volume = 0;
            birdFx.volume = 0;
            uiFx.volume = 0;
            bubbleFx.volume = 0;
            shopFx.volume = 0;
            btn.image.overrideSprite = mute;
            muted = true;
        }
    }
}
