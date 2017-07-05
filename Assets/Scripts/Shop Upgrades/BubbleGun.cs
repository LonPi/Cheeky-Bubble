using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGun : MonoBehaviour {

    public float duration, sizeIncreaseFactor;
    bool timerStart;
    float timer;
    bool purchased;

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

    public void OnPurchase()
    {
        GameDataManager.instance.IncrementBubbleGunCount();
        Debug.Log("Purchased bubble gun. Count: " + GameDataManager.instance.GetBubbleGunCount());
        purchased = true;
        timerStart = true;
        timer += duration;
    }

    public void OnSceneLoaded()
    {
        if (purchased)
        {
            BlowBubble.Instance.EquipBubbleGun(sizeIncreaseFactor);
        }
    }

    void UnequipBubbleGun()
    {
        if (GameManager.instance.currentScene.name == "competitive" || GameManager.instance.currentScene.name == "casual")
        {
            BlowBubble.Instance.UnequipBubbleGun();
        }
    }
    void OnPurchaseExpired()
    {
        purchased = false;
        UnequipBubbleGun();
        timerStart = false;
        timer = 0;
    }

    public float RemainingTime()
    {
        return timer;
    }
}

