﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetBubble : MonoBehaviour {

    public float buffRadius, duration;
    bool purchased;
    float timer;
    float timeElapsed;
    bool timerStart;
    Bubble bubble;
    List<GameObject> bubbleList = new List<GameObject>();

    void Update()
    {
        if (timerStart)
        {
            if (!UpdateTimer())
                OnPurchaseExpired();
        }
    }

    bool UpdateTimer()
    {
        timer -= Time.deltaTime;
        timeElapsed += Time.deltaTime;

        // update count
        if (timeElapsed >= duration)
        {
            GameDataManager.instance.DecrementMagnetCount();
            timeElapsed = 0;
        }

        if (timer <= 0)
        {
            // release the list
            UnequipMagnet();
            return false;
        }
        return true;
    }

    void UnequipMagnet()
    {
        foreach (GameObject bubble in bubbleList)
        {
            bubble.GetComponent<Bubble>().UnequipMagnet();
        }

        bubbleList.Clear();
    }

    public void OnSceneLoaded()
    {
        if (purchased)
        {
            EquipMagnetBubble();
        }
    }

    public void OnPurchase()
    {
        GameDataManager.instance.IncrementMagnetCount();
        Debug.Log("Purchased magnet bubble. Count: " + GameDataManager.instance.GetMagnetCount());
        timerStart = true;
        timer += duration;
        purchased = true;
        GameDataManager.instance.SaveGame();
    }

    void EquipMagnetBubble()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            // need this variable to globally control all the bubbles in the scene
            BlowBubble.Instance.MagnetBubblePurchased = true;
        }
    }

    void UnequipMagnetBubble()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            // need this variable to globally control all the bubbles in the scene
            BlowBubble.Instance.MagnetBubblePurchased = false;
        }
    }

    public void OnPurchaseExpired()
    {
        UnequipMagnet();
        purchased = false;
        timerStart = false;
        timer = 0;
        Debug.Log("Magnet Bubble expired.. timer: " + timer + " magnet count: " + GameDataManager.instance.GetMagnetCount());
        GameDataManager.instance.SaveGame();
    }

    public void OnFinishedLoading()
    {
        // if there exists magnet in inventory, start count down when we start the game
        if (GameDataManager.instance.GetMagnetCount() > 0)
        {
            timerStart = true;
            timer = GameDataManager.instance.GetLoadedMagnetTimer();
            purchased = true;
            EquipMagnetBubble();
        }
    }

    public void SetAndAddToList(GameObject bubble)
    {
        bubbleList.Add(bubble);
        bubble.GetComponent<Bubble>().EquipMagnet(buffRadius);
    }

    public float RemainingTime()
    {
        return timer;
    }
}
