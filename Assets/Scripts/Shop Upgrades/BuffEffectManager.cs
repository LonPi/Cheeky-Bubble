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
	
    void Update ()
    {
        chickenFeedTimer = chickenFeed.RemainingTime();
        penguinFeedTimer = penguinFeed.RemainingTime();
        bubbleGunTimer = bubbleGun.RemainingTime();
        magnetTimer = magnetBubble.RemainingTime();
    }

    public float BuffTimer (string buffType)
    {
        if (buffType == "chickenFeed")
            return chickenFeedTimer;
        if (buffType == "penguinFeed")
            return penguinFeedTimer;
        if (buffType == "bubbleGun")
            return bubbleGunTimer;
        if (buffType == "magnet")
            return magnetTimer;

        return 0;
    }
}
