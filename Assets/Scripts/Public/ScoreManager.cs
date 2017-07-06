using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance = null;
    public HighScore highScore;
    int initChicken,
        initPenguin,
        caughtChicken,
        caughtPenguin,
        prevChicken,
        prevPenguin;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Initialize();
        LoadHighScore();
        if (prevChicken > highScore.highestChicken)
            highScore.highestChicken = prevChicken;
        if (prevPenguin > highScore.highestPenguin)
            highScore.highestPenguin = prevPenguin;
    }

    public void Initialize()
    {
        initChicken = GameDataManager.instance.GetCaughtChickenCount();
        initPenguin = GameDataManager.instance.GetCaughtPenguinCount();
    }

    void RecordHighScore()
    {
        highScore.currentChicken = caughtChicken;
        highScore.currentPenguin = caughtPenguin;
        if (caughtChicken > highScore.highestChicken)
            highScore.highestChicken = caughtChicken;
        if (caughtPenguin > highScore.highestPenguin)
            highScore.highestPenguin = caughtPenguin;
        SaveHighScore();
    }

    public void OnGameOver()
    {
        caughtChicken = GameDataManager.instance.GetCaughtChickenCount() - initChicken;
        caughtPenguin = GameDataManager.instance.GetCaughtPenguinCount() - initPenguin;
        RecordHighScore();
    }

    public struct HighScore
    {
        public int
            highestChicken,
            highestPenguin,
            currentChicken,
            currentPenguin;
    }

    void SaveHighScore()
    {
        PlayerPrefs.SetInt("Most Chicken", highScore.highestChicken);
        PlayerPrefs.SetInt("Most Penguin", highScore.highestPenguin);

#if UNITY_ANDROID
        PlayGamesScript.AddScoreToLeaderboard(GPGSIds.leaderboard_chicky_master, highScore.highestChicken);
        PlayGamesScript.AddScoreToLeaderboard(GPGSIds.leaderboard_pengi_master, highScore.highestPenguin);
#endif
    }

    void LoadHighScore()
    {
        prevChicken = PlayerPrefs.GetInt("Most Chicken");
        prevPenguin = PlayerPrefs.GetInt("Most Penguin");
    }
}
