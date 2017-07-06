using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardUI : MonoBehaviour
{
    // components belongs to this game object
    public Text myButtonText;
    Button myButton;
    Image myImage;


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
            myButtonText.enabled = true;
        }
        else
        {
            myButton.enabled = false;
            myImage.enabled = false;
            myButtonText.enabled = false;
        }
    }

    public void DisplayRewardMessage()
    {
        // for now just credit 100 chicken into inventory
        int chickenCount = GameDataManager.instance.GetCaughtChickenCount();
        chickenCount += 100;
        Debug.Log("crediting chicken count to: " + chickenCount);
        GameDataManager.instance.SetChickenCount(chickenCount);
        GameDataManager.instance.SetDailyRewardClaimed(true);
        GameDataManager.instance.SaveGame();
    }

    public void PressOk()
    {
        // TODO: credit reward into inventory
        // set claim flag to true so that reward doesnt show anymore for that day
    }
}
