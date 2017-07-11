using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float countdownTimerStartVal;
    public static GameManager instance = null;
    public Camera _cameraRef { get; set; }
    public GameObject _blowBubbleRef { get; set; }
    public ChickenGenerator _chickenGeneratorLeft { get; set; }
    public ChickenGenerator _chickenGeneratorRight { get; set; }
    public PenguinGenerator _penguinGeneratorLeft { get; set; }
    public PenguinGenerator _penguinGeneratorRight { get; set; }
    public Scene currentScene { get; set; }
    public CanvasUI _canvasUI { get; set; }
    public bool startGame { get; set; }
    public bool isLoadingGameFile { get; set; }
    public int coinToAdd { get; set; }
    public int combo { get; set; }
    public int multiplier { get; set; }

    string previousSceneName;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        isLoadingGameFile = true; // google play script sign in flag
    }

    void Start()
    {
        InitReferences();
    }

    void InitReferences()
    {
        _cameraRef = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        if (currentScene.name == "casual")
        {
            _chickenGeneratorLeft = GameObject.Find("generator_left").GetComponent<ChickenGenerator>();
            _chickenGeneratorRight = GameObject.Find("generator_right").GetComponent<ChickenGenerator>();
            _canvasUI = GameObject.Find("CanvasUI").GetComponent<CanvasUI>();
            _blowBubbleRef = GameObject.Find("blow_bubble_start");
        }

        if (currentScene.name == "competitive")
        {
            _canvasUI = GameObject.Find("CanvasUI").GetComponent<CanvasUI>();
            _blowBubbleRef = GameObject.Find("blow_bubble_start");
            // left generators
            GameObject go = GameObject.Find("generator_left");
            _chickenGeneratorLeft = go.GetComponent<ChickenGenerator>();
            _penguinGeneratorLeft = go.GetComponent<PenguinGenerator>();
            // right generators
            go = GameObject.Find("generator_right");
            _chickenGeneratorRight = go.GetComponent<ChickenGenerator>();
            _penguinGeneratorRight = go.GetComponent<PenguinGenerator>();
        }

        coinToAdd = 0;
        combo = 0;
        multiplier = 5;
    }

    public void DisableUserInput()
    {
        _blowBubbleRef.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void EnableUserInput()
    {
        _blowBubbleRef.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void Restart()
    {
        StartCoroutine(_ReloadScene());
    }

    public void SetCoin(int add)
    {
        if (add == 0)
        {
            combo = 0;
            coinToAdd = 0;
        }
        else
        {
            combo++;
            coinToAdd = combo / multiplier;
        }
        //Debug.Log(combo + " - " + multiplier + " to " + coinToAdd);
    }

    // get the previous scene
    public string GetPreviousSceneName()
    {
        return previousSceneName;
    }

    // go to scene directly
    public void GoToScene(string sceneName)
    {
        StartCoroutine(_SwitchScene(sceneName));
    }

    // toggle between casual and competitive
    public void SwitchGameMode()
    {
        // unpause the game
        Time.timeScale = 1;
        if (currentScene.name == "casual")
        {
            StartCoroutine(_SwitchScene("competitive"));
        }

        if (currentScene.name == "competitive")
        {
            StartCoroutine(_SwitchScene("casual"));
        }
    }

    public void OnEndGame()
    {
        // disable input
        DisableUserInput();

        // release bubble
        _blowBubbleRef.GetComponent<BlowBubble>().OnEndGame();

        // stop generators
        if (currentScene.name == "casual")
        {
            _chickenGeneratorLeft.gameObject.SetActive(false);
            _chickenGeneratorRight.gameObject.SetActive(false);
        }

        if (currentScene.name == "competitive")
        {
            _chickenGeneratorLeft.gameObject.SetActive(false);
            _penguinGeneratorLeft.gameObject.SetActive(false);
            _chickenGeneratorRight.gameObject.SetActive(false);
            _penguinGeneratorRight.gameObject.SetActive(false);
        }
    }

    public void OnPause()
    {
        DisableUserInput();
    }

    public void OnUnpause()
    {
        EnableUserInput();
    }

    public void OnFinishedLoading()
    {
        GoToScene("casual");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void PositionGenerator()
    {
        // position our generator at the edge of the screen
        _chickenGeneratorRight.gameObject.transform.position = _cameraRef.ViewportToWorldPoint(new Vector2(0.99f, 0.5f));
        _chickenGeneratorLeft.gameObject.transform.position = _cameraRef.ViewportToWorldPoint(new Vector2(0.01f, 0.5f));

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = SceneManager.GetActiveScene();
        InitReferences();
        if (currentScene.name == "casual" || currentScene.name == "competitive")
        {
            PositionGenerator();
            _canvasUI.OnSceneLoaded(scene.name == "competitive" ? true : false);
            // activate the relevant object
            BuffEffectManager.instance.chickenFeed.OnSceneLoaded();
            BuffEffectManager.instance.penguinFeed.OnSceneLoaded();
            BuffEffectManager.instance.bubbleGun.OnSceneLoaded();
            BuffEffectManager.instance.magnetBubble.OnSceneLoaded();
        }

        PoolManager.instance.OnSceneLoaded();
        GameDataManager.instance.OnSceneLoaded();
    }

    IEnumerator _SwitchScene(string sceneName)
    {
        // save previous scene
        previousSceneName = SceneManager.GetActiveScene().name;
        float fadeTime = GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        if (sceneName == null)
            SceneManager.LoadScene(previousSceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    IEnumerator _ReloadScene()
    {
        // save previous scene
        previousSceneName = SceneManager.GetActiveScene().name;
        float fadeTime = GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
