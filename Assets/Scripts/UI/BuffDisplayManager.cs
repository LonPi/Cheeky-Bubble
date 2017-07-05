using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffDisplayManager : MonoBehaviour {

    GameObject chickenBuff,
        penguinBuff,
        bubbleBuff,
        magnetBuff;
    float chickenBuffTime,
        penguinBuffTime,
        bubbleBuffTime,
        magnetBuffTime;

    // Use this for initialization
    void Start () {
        chickenBuff = GameObject.Find("chickenFeed").gameObject;
        penguinBuff = GameObject.Find("penguinFeed").gameObject;
        bubbleBuff = GameObject.Find("bubbleGun").gameObject;
        magnetBuff = GameObject.Find("magnet").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        chickenBuffTime = BuffEffectManager.instance.BuffTimer("chickenFeed");
        penguinBuffTime = BuffEffectManager.instance.BuffTimer("penguinFeed");
        bubbleBuffTime = BuffEffectManager.instance.BuffTimer("bubbleGun");
        magnetBuffTime = BuffEffectManager.instance.BuffTimer("magnet");


        if (chickenBuffTime > 0f)
        {
            chickenBuff.SetActive(true);
            var minutes = Mathf.Floor(chickenBuffTime / 60f);
            var seconds = Mathf.Floor(chickenBuffTime % 60f);
            chickenBuff.transform.Find("Time").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        else
            chickenBuff.SetActive(false);

        if (penguinBuffTime > 0f)
        {
            penguinBuff.SetActive(true);
            var minutes = Mathf.Floor(penguinBuffTime / 60f);
            var seconds = Mathf.Floor(penguinBuffTime % 60f);
            penguinBuff.transform.Find("Time").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        else
            penguinBuff.SetActive(false);

        if (bubbleBuffTime > 0f)
        {
            bubbleBuff.SetActive(true);
            var minutes = Mathf.Floor(bubbleBuffTime / 60f);
            var seconds = Mathf.Floor(bubbleBuffTime % 60f);
            bubbleBuff.transform.Find("Time").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        else
            bubbleBuff.SetActive(false);

        if (magnetBuffTime > 0f)
        {
            magnetBuff.SetActive(true);
            var minutes = Mathf.Floor(magnetBuffTime / 60f);
            var seconds = Mathf.Floor(magnetBuffTime % 60f);
            magnetBuff.transform.Find("Time").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        else
            magnetBuff.SetActive(false);
    }
}
