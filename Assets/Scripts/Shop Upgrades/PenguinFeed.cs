﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinFeed : MonoBehaviour {

    public float reducedTime, duration;
    GameObject GeneratorLeft, GeneratorRight;
    PenguinGenerator generatorLeft, generatorRight;
    bool timerStart;
    bool purchased;
    float timer;
    float timeElapsed;
	
    void Update()
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
            GameDataManager.instance.DecrementPenguinFeedCount();
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
        if (GameManager.instance.currentScene.name == "competitive")
        {
            GeneratorLeft = GameObject.Find("generator_left");
            GeneratorRight = GameObject.Find("generator_right");
            generatorLeft = GeneratorLeft.GetComponent<PenguinGenerator>();
            generatorRight = GeneratorRight.GetComponent<PenguinGenerator>();
            EquipPenguinFeed();
        }

        
    }

    public void OnSceneLoaded()
    {
        InitReferences();
        if (purchased)
        {
            EquipPenguinFeed();
        }
    }

    void EquipPenguinFeed()
    {
        if (GameManager.instance.currentScene.name == "competitive")
        {
            generatorLeft.EquipPenguinFeed(reducedTime);
            generatorRight.EquipPenguinFeed(reducedTime);
        }
    }

    void UnequipPenguinFeed()
    {
        if (GameManager.instance.currentScene.name == "competitive")
        {
            generatorLeft.UnequipPenguinFeed();
            generatorRight.UnequipPenguinFeed();
        }
    }

    public void OnPurchase()
    {
        GameDataManager.instance.IncrementPenguinFeedCount();
        Debug.Log("Purchased penguin feed, count: " + GameDataManager.instance.GetPenguinFeedCount());

        purchased = true;
        timerStart = true;
        // time is stackable
        timer += duration;
    }

    public void OnLoadGame()
    {
        // if there exists penguin feed in inventory, start count down when we start the game
        if (GameDataManager.instance.GetPenguinFeedCount() > 0)
        {
            timerStart = true;
            timer = GameDataManager.instance.GetPenguinFeedCount() * duration;
        }
    }

    public void OnPurchaseExpired()
    {
        Debug.Log("Penguin feed expired");
        UnequipPenguinFeed();
        purchased = false;
        timerStart = false;
        timer = 0;
    }

    public float RemainingTime()
    {
        return timer;
    }
}
