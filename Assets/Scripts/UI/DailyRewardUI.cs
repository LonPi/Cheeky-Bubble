using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardUI : MonoBehaviour
{
    // components belongs to this game object
    Button myButton;
    Image myImage;
    int rewardCount = 5;
    public GameObject coin;
    public ParticleSystem sparkle;
    bool isPlayingAnimation;

    void Start()
    {
        myButton = GetComponent<Button>();
        myImage = GetComponent<Image>();
    }

    void Update()
    {
        // enable/disable individual components so that the parent game object can still run its update loop and
        // to check the dailyRewardClaimed variable from game manager.
        // disabled gameObject will stop running its update loop and we don't want that to happen
        if (!GameDataManager.instance.GetDailyRewardClaimed())
        {
            myButton.enabled = true;
            myImage.enabled = true;
        }
        else
        {
            myButton.enabled = false;
            myImage.enabled = false;
        }
    }

    public void DisplayRewardMessage()
    {
        if (!isPlayingAnimation)
        {
            SoundManager.Instance.ShopPlayOneShot(SoundManager.Instance.shopBuy);
            StartCoroutine(CollectDailyReward());
        }

    }

    IEnumerator CollectDailyReward()
    {
        isPlayingAnimation = true;
        Instantiate(sparkle, this.transform.position, Quaternion.identity).name = "sparkle";
        for (int x = 0; x < rewardCount; x++)
        {
            //Instantiate(coin, this.transform.position, Quaternion.identity).name = "Penguin_Drop";
            GameObject coinObj = PoolManager.instance.GetObjectfromPool(coin);
            coinObj.GetComponent<CoinFly>().InitParams(transform.position, CoinFly.Type.PENGUIN);
            yield return new WaitForSeconds(0.2f);
        }
        sparkle.Stop();
        GameDataManager.instance.SetDailyRewardClaimed(true);
        Debug.Log("CollectDailyReward: penguin count: " + GameDataManager.instance.GetCaughtPenguinCount());
        yield return new WaitForSeconds(0.2f * rewardCount);
        GameDataManager.instance.SaveGame();
        isPlayingAnimation = false;
    }
}
