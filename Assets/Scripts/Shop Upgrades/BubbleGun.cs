using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGun : MonoBehaviour {

    public float duration, sizeIncreaseFactor;
    bool timerStart;
    float timer;
    bool purchased;
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
            GameDataManager.instance.DecrementBubbleGunCount();
            timeElapsed = 0;
        }

        if (timer <= 0)
        {
            return false;
        }
        return true;
    }

    public void OnPurchase()
    {
        GameDataManager.instance.IncrementBubbleGunCount();
        Debug.Log("Purchased bubble gun. Count: " + GameDataManager.instance.GetBubbleGunCount());
        purchased = true;
        timerStart = true;
        timer += duration;
        GameDataManager.instance.SaveGame();
    }

    public void OnSceneLoaded()
    {
        if (purchased)
        {
            EquipBubbleGun();
        }
    }

    void UnequipBubbleGun()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            BlowBubble.Instance.UnequipBubbleGun();
        }
    }

    void EquipBubbleGun()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            BlowBubble.Instance.EquipBubbleGun(sizeIncreaseFactor);
        }
    }

    public void OnFinishedLoading()
    {
        // if there exists bubble gun in inventory, start count down when we start the game
        if (GameDataManager.instance.GetBubbleGunCount() > 0)
        {
            timerStart = true;
            timer = GameDataManager.instance.GetLoadedBubbleGunTimer();
            purchased = true;
            // equip it
            EquipBubbleGun();
        }
    }

    void OnPurchaseExpired()
    {
        purchased = false;
        UnequipBubbleGun();
        timerStart = false;
        timer = 0;
        Debug.Log("Bubble Gun expired.. timer: " + timer + " bubble gun count: " + GameDataManager.instance.GetBubbleGunCount());
        GameDataManager.instance.SaveGame();
    }

    public float RemainingTime()
    {
        return timer;
    }
}

