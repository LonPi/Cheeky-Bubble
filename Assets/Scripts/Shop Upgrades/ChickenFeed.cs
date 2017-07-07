using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFeed : MonoBehaviour {

    public float reducedTime, duration;
    GameObject GeneratorLeft, GeneratorRight;
    ChickenGenerator generatorLeft, generatorRight;
    bool timerStart;
    bool purchased;
    float timer;
    float timeElapsed;
	
	void Update ()
    {
		if (timerStart)
        {
            if (!UpdateTimer())
            {
                OnPurchaseExpired();
            }
        }
	}

    bool UpdateTimer()
    {
        timer -= Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            GameDataManager.instance.DecrementChickenFeedCount();
            timeElapsed = 0;
        }

        if (timer <= 0)
        {
            
            return false;
        }

        return true;
    }
    void InitReferences()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            GeneratorLeft = GameObject.Find("generator_left");
            GeneratorRight = GameObject.Find("generator_right");
            generatorLeft = GeneratorLeft.GetComponent<ChickenGenerator>();
            generatorRight = GeneratorRight.GetComponent<ChickenGenerator>();
        }

        
    }

    public void OnSceneLoaded()
    {
        InitReferences();
        if (purchased)
        {
            EquipChickenFeed();
        }

        
    }

    public void OnPurchase()
    {
        GameDataManager.instance.IncrementChickenFeedCount();
        purchased = true;
        timerStart = true;
        // time is stackable
        timer += duration;
        Debug.Log("Purchased chicken feed, count: " + GameDataManager.instance.GetChickenFeedCount() + " saving game: timer=" + timer + " count: " + GameDataManager.instance.GetChickenFeedCount());
        GameDataManager.instance.SaveGame();
    }

    void EquipChickenFeed()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            generatorLeft.EquipChickenFeed(reducedTime);
            generatorRight.EquipChickenFeed(reducedTime);
        }
        
    }

    void UnequipChickenFeed()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            generatorLeft.UnequipChickenFeed();
            generatorRight.UnequipChickenFeed();
        }
    }

    public void OnPurchaseExpired()
    {
        UnequipChickenFeed();
        purchased = false;
        timerStart = false;
        timer = 0;
        Debug.Log("Chicken Feed expired.. timer: " + timer + " penguin feed count: " + GameDataManager.instance.GetPenguinFeedCount());
        GameDataManager.instance.SaveGame();
    }

    public void OnFinishedLoading()
    {
        // if there exists chicken feed in inventory, start count down when we start the game
        if (GameDataManager.instance.GetChickenFeedCount() > 0)
        {
            timerStart = true;
            timer = GameDataManager.instance.GetLoadedChickenFeedTimer();
            purchased = true;
            EquipChickenFeed();
        }
    }

    public float RemainingTime()
    {
        return timer;
    }
}
