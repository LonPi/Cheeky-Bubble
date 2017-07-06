using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUI : MonoBehaviour {

    Text chickenCount, 
        penguinCount,
        timer,
        chickenCurrent,
        chickenHigh,
        penguinCurrent,
        penguinHigh,
        comboText;
    GameObject countdownOverlay,
        creditText,
        sliderCol1,
        endGameStats;
    RectTransform optionSlider;
    Vector2 closePosition,
        openPosition;
    bool creditOpen = false;
    bool isPaused = false;
    float countdownTimer;
    bool isInCompetitiveMode, 
        start, 
        endGame;

    void Start ()
    {
        Initialize();
        creditText.SetActive(creditOpen);
        endGameStats.SetActive(false);
        timer.gameObject.SetActive(isInCompetitiveMode);
        countdownTimer = GameManager.instance.countdownTimerStartVal;
        closePosition = new Vector2(optionSlider.anchoredPosition.x, optionSlider.anchoredPosition.y);
        openPosition = new Vector2(optionSlider.anchoredPosition.x, 0f);
    }
	
	void Update ()
    {
        chickenCount.text = GameDataManager.instance.GetCaughtChickenCount().ToString();
        penguinCount.text = GameDataManager.instance.GetCaughtPenguinCount().ToString();

        if (isInCompetitiveMode && start)
        {
            timer.gameObject.SetActive(isInCompetitiveMode);
            UpdateTimer();
        }

        if (isInCompetitiveMode && !start && endGame)
        {
            // show end game stats
            ScoreManager.instance.OnGameOver();
            ShowEndGameStats();
        }

        if (isPaused)
            optionSlider.anchoredPosition = Vector2.Lerp(optionSlider.anchoredPosition, openPosition, 0.5f);

        else
            optionSlider.anchoredPosition = Vector2.Lerp(optionSlider.anchoredPosition, closePosition, 0.5f);

        if (GameManager.instance.coinToAdd > 0)
        {
            comboText.text = "Chicky + " + GameManager.instance.coinToAdd;
            StartCoroutine(FadeTextToFullAlpha(2f, comboText));
        }

        else
        {
            comboText.text = "";
            StartCoroutine(FadeTextToZeroAlpha(1f, comboText));
        }
    }

    void UpdateTimer()
    {
        if (countdownTimer <= 0)
        {
            countdownTimer = 0;
            endGame = true;
            start = false;
        }
        timer.text = Mathf.CeilToInt(countdownTimer).ToString();
        countdownTimer -= Time.deltaTime;
        
    }

    void ShowEndGameStats()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.timeOut);
        endGameStats.SetActive(true);
        chickenCurrent.text = ScoreManager.instance.highScore.currentChicken.ToString();
        chickenHigh.text = ScoreManager.instance.highScore.highestChicken.ToString();
        penguinCurrent.text = ScoreManager.instance.highScore.currentPenguin.ToString();
        penguinHigh.text = ScoreManager.instance.highScore.highestPenguin.ToString();
        GameManager.instance.OnEndGame();
    }

    // called by countdown overlay
    public void StartCompetitiveGame()
    {
        start = true;
        countdownTimer = GameManager.instance.countdownTimerStartVal;
        GameManager.instance.EnableUserInput();
        GameManager.instance.startGame = true;
        ScoreManager.instance.Initialize();
    }

    public void PressRetry()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        Time.timeScale = 1;
        GameManager.instance.Restart();
    }

    public void SwitchGameMode()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        GameManager.instance.SwitchGameMode();
    }

    public void OnSceneLoaded(bool _isInCompetitiveMode)
    {
        isInCompetitiveMode = _isInCompetitiveMode;
        Initialize();

        if (isInCompetitiveMode)
        {
            // start countdown overlay before game actually started
            countdownOverlay.gameObject.GetComponent<CountdownOverlay>().StartCountdownTimer();
        }

        else
        {
            GameManager.instance.startGame = true;
        }
    }

    void Initialize()
    {
        countdownOverlay = GameObject.Find("countdown_overlay");
        chickenCount = GameObject.Find("chicken_count").GetComponent<Text>();
        penguinCount = GameObject.Find("penguin_count").GetComponent<Text>();
        timer = GameObject.Find("Timer").GetComponent<Text>();
        creditText = GameObject.Find("CreditText").gameObject;
        endGameStats = GameObject.Find("EndGameStats").gameObject;
        optionSlider = GameObject.Find("Slider").GetComponent<RectTransform>();
        chickenCurrent = GameObject.Find("chickyCurrent").GetComponent<Text>();
        chickenHigh = GameObject.Find("chickyHigh").GetComponent<Text>();
        penguinCurrent = GameObject.Find("pengiCurrent").GetComponent<Text>();
        penguinHigh = GameObject.Find("pengiHigh").GetComponent<Text>();
        comboText = GameObject.Find("comboText").GetComponent<Text>();
    }

    public void PressCredit()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        if (creditOpen)
            creditOpen = false;
        else
            creditOpen = true;
        creditText.SetActive(creditOpen);
    }

    public void PressBackCredit()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        creditOpen = false;
        creditText.SetActive(creditOpen);
    }

    public void PressShop()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        Time.timeScale = 1;
        GameManager.instance.GoToScene("shop");
    }

    public void PressPause()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            GameManager.instance.OnPause();
        }

        else
        {
            isPaused = false;
            Time.timeScale = 1;
            GameManager.instance.OnUnpause();
        }
    }

    IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
