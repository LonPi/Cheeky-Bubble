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

    void Awake()
    {
        InitReferences();
    }
	
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
        Debug.Log("Purchased chicken feed, count: " + GameDataManager.instance.GetChickenFeedCount());

        purchased = true;
        timerStart = true;
        // time is stackable
        timer += duration;
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
        Debug.Log("Chicken feed expired");
        UnequipChickenFeed();
        purchased = false;
        timerStart = false;
        timer = 0;
    }

    public float RemainingTime()
    {
        return timer;
    }
}
