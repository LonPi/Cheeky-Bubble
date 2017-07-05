using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownOverlay : MonoBehaviour {

    public float timerStartValue, fadeoutTime;
    public Text timerText;
    public Image fadeoutImage;
    bool start, fadeout;
    float timer;
    bool isRunningCoroutine;

	void Start ()
    {
        timer = timerStartValue;
        fadeoutImage = GetComponent<Image>();
	}
	
	void Update ()
    {
        if (start)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
            if (timer <= 0)
            {
                timer = 0;
                start = false;
                StartCoroutine(_DisplayStartText());
                
            }
        }

        else
        {
            if (!isRunningCoroutine)
                StartCoroutine(_FadeOut());
        }
    }

    public void StartCountdownTimer()
    {
        start = true;
        timer = timerStartValue;
        GameManager.instance.DisableUserInput();
    }

    IEnumerator _DisplayStartText()
    {
        timerText.text = "START!";
        yield return new WaitForSeconds(0.5f);
        GameManager.instance._canvasUI.StartCompetitiveGame();
    }

    IEnumerator _FadeOut()
    {
        Color color = fadeoutImage.color;
        isRunningCoroutine = true;
        while (color.a > 0)
        {
            color.a -= 0.01f;
            fadeoutImage.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        gameObject.SetActive(false);
        
        isRunningCoroutine = false;
    }
}
