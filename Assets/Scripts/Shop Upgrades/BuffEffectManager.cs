using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectManager : MonoBehaviour {

    public static BuffEffectManager instance;
    public BubbleGun bubbleGun { get; set; }
    public MagnetBubble magnetBubble { get; set; }
    public ChickenFeed chickenFeed { get; set; }
    public PenguinFeed penguinFeed { get; set; }
    float chickenFeedTimer,
        penguinFeedTimer,
        bubbleGunTimer,
        magnetTimer;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        bubbleGun = GetComponent<BubbleGun>();
        magnetBubble = GetComponent<MagnetBubble>();
        chickenFeed = GetComponent<ChickenFeed>();
        penguinFeed = GetComponent<PenguinFeed>();
    }
	
    public float GetChickenFeedTimer()
    {
        return chickenFeed.RemainingTime();
    }

    public float GetPenguinFeedTimer()
    {
        return penguinFeed.RemainingTime();
    }

    public float GetBubbleGunTimer()
    {
        return bubbleGun.RemainingTime();
    }

    public float GetMagnetTimer()
    {
        return magnetBubble.RemainingTime();
    }
}
