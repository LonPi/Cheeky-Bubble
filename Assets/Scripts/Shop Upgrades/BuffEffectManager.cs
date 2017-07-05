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
        /* For debugging and testing */
        if (Input.GetKeyDown(KeyCode.B))
        {
            bubbleGun.OnPurchase();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            magnetBubble.OnPurchase();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            chickenFeed.OnPurchase();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            penguinFeed.OnPurchase();
        }

        chickenFeedTimer = chickenFeed.RemainingTime();
        penguinFeedTimer = penguinFeed.RemainingTime();
        bubbleGunTimer = bubbleGun.RemainingTime();
        magnetTimer = magnetBubble.RemainingTime();
    }

    public float BuffTimer (string buffType)
    {
        if (buffType == "chickenFeed")
            return chickenFeedTimer;
        else if (buffType == "penguinFeed")
            return penguinFeedTimer;
        else if(buffType == "bubbleGun")
            return bubbleGunTimer;
        else if(buffType == "magnet")
            return magnetTimer;
        else
            return 0f;
    }
}
