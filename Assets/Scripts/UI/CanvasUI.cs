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
        penguinHigh;
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
        chickenCount.text = GameDataManager.instance.GetCaughtBirdCount().ToString();
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
        Time.timeScale = 1;
        GameManager.instance.Restart();
    }

    public void SwitchGameMode()
    {
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
    }

    public void PressCredit()
    {
        if (creditOpen)
            creditOpen = false;
        else
            creditOpen = true;
        creditText.SetActive(creditOpen);
    }

    public void PressBackCredit()
    {
        creditOpen = false;
        creditText.SetActive(creditOpen);
    }

    public void PressShop()
    {
        Time.timeScale = 1;
        GameManager.instance.GoToScene("shop");
    }

    public void PressPause()
    {
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
}
