using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinFeed : MonoBehaviour {

    public float reducedTime, duration;
    GameObject GeneratorLeft, GeneratorRight;
    PenguinGenerator generatorLeft, generatorRight;
    bool timerStart;
    bool purchased;
    float timer;

    void Awake ()
    {
        InitReferences();
    }
	
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
