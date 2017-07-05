using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    Text timerText;
    float time = 30f;

	void Start () {
        timerText = GetComponent<Text>();
	}
	
	void Update () {
        if (time > 0f) time -= Time.deltaTime;
        timerText.text = Mathf.Floor(time).ToString();
    }
}
