using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    // true if app is opened fresh from menu (not resumed from task manager)
    bool bootFirstTime;

    // game variables
    // TODO: save game to cloud when an item expires
    bool isGameStartedFirstTime;
    bool dailyRewardClaimed;
    int bubbleGunCount;
    int magnetCount;
    int chickenFeedCount;
    int penguinFeedCount;
    int caughtChicken;
    int caughtPenguin;
    int totalItemsPurchasedToDate;
    // variables that store params from save file (read only)
    float loadedChickenFeedTimer;
    float loadedPenguinFeedTimer;
    float loadedBubbleGunTimer;
    float loadedMagnetTimer;
    DateTime lastLoginTime;  // the earliest time of the day where the user logs in
    bool asyncNotified;
    DateTime asyncReturnValue;
    GameData data;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        bootFirstTime = true; // true if application is first launch (ie not resumed from android home button)
    }

    private void Update()
    {
        
    }

    public void OnSceneLoaded()
    {
        if (GameManager.instance.GetPreviousSceneName() != null && GameManager.instance.GetPreviousSceneName() != "title")
        {
            bootFirstTime = false;
        }
        // save game every time we load a scene
        if (!bootFirstTime)
        {
            Debug.Log("!bootFirstTime.. saving game");
            SaveGame();
        }
    }

    // handle cloud and local save
    public void SaveGame()
    {
        Debug.Log("saving game: variable caughtChicken: " + caughtChicken + ", variable caughtPenguin: " +
            caughtPenguin + ", total items purchased: " + totalItemsPurchasedToDate + ", last login: " + lastLoginTime + ", reward claimed: " + dailyRewardClaimed);

        // Save all caught items and inventory items
        data = new GameData();
        data.SetIsGameStartedFirstTime(isGameStartedFirstTime);
        data.SetCaughtChickenCount(caughtChicken);
        data.SetCaugthPenguinCount(caughtPenguin);
        data.SetTotalItemsPurchasedToDate(totalItemsPurchasedToDate);
        data.SetChickenFeedCount(chickenFeedCount);
        data.SetPenguinFeedCount(penguinFeedCount);
        data.SetBubbleGunCount(bubbleGunCount);
        data.SetMagnetCount(magnetCount);
        data.SetLastLoginTime(lastLoginTime);
        data.SetDailyRewardClaimed(dailyRewardClaimed);
        data.SetChickenFeedTimer(BuffEffectManager.instance.GetChickenFeedTimer());
        data.SetPenguinFeedTimer(BuffEffectManager.instance.GetPenguinFeedTimer());
        data.SetBubbleGunTimer(BuffEffectManager.instance.GetBubbleGunTimer());
        data.SetMagnetTimer(BuffEffectManager.instance.GetMagnetTimer());
        // save first half of the data before server got back to us
        SaveLoadManager.instance.SaveData(data);
        SaveNetworkUTCTime();
    }

    void SaveNetworkUTCTime()
    {
        // DateTime dummy = new DateTime(2016, 7, 8);
        // Get UTC time
        // first check from PlayerPref and compare to current device clock
        // to determine if its necessary to get network time from server
        // we don't want to keep making server connection if its not necessary
        if (PlayerPrefs.GetInt("lastLoginDay") != DateTime.UtcNow.Day  /*dummy.Day */||
             PlayerPrefs.GetInt("lastLoginMonth") != DateTime.UtcNow.Month /*dummy.Month*/ ||
             PlayerPrefs.GetInt("lastLoginYear") != DateTime.UtcNow.Year /*dummy.Year*/)
        {
            NetworkHandler.GetNetworkUTCTimeAsync(); // save game code path = true
            StartCoroutine(RunOnMainThread());
        }
    }

    // called after async job is finished
    void SaveNetworkUTCTimeCallback(DateTime currentServerUTCTime)
    {
        Debug.Log("SaveNetworkUTCTimeCallback() callback: real server's currentTime.Date=" + currentServerUTCTime.Day + " game's lastloginTime.Date=" + lastLoginTime.Day);
        
        //currentServerUTCTime = new DateTime(2016, 7, 8);
        //Debug.Log("lastlogin time day: " + lastLoginTime.Day + " faked server utc day -1: " + currentServerUTCTime.AddDays(-1).Day);

        // check if user qualified for reward
        if (lastLoginTime.Day <= currentServerUTCTime.AddDays(-1).Day)
        {
            // Give out login bonus
            dailyRewardClaimed = false;
            Debug.Log("GIVING OUT DAILY LOGIN REWARD");
        }

        else
        {
            Debug.Log("NOT GIVING OUT DAILY LOGIN REWARD");
        }

        // update game timestamp to server's current time
        lastLoginTime = currentServerUTCTime;

        // continue saving our data after server got back to us
        // player might have caught more chicken and penguin during while we were waiting for server reply
        // save everything
        PlayerPrefs.SetInt("lastLoginDay", lastLoginTime.Day);
        PlayerPrefs.SetInt("lastLoginMonth", lastLoginTime.Month);
        PlayerPrefs.SetInt("lastLoginYear", lastLoginTime.Year);
        data = new GameData();
        data.SetIsGameStartedFirstTime(isGameStartedFirstTime);
        data.SetCaughtChickenCount(caughtChicken);
        data.SetCaugthPenguinCount(caughtPenguin);
        data.SetTotalItemsPurchasedToDate(totalItemsPurchasedToDate);
        data.SetChickenFeedCount(chickenFeedCount);
        data.SetPenguinFeedCount(penguinFeedCount);
        data.SetBubbleGunCount(bubbleGunCount);
        data.SetMagnetCount(magnetCount);
        data.SetLastLoginTime(lastLoginTime);
        data.SetDailyRewardClaimed(dailyRewardClaimed);
        data.SetChickenFeedTimer(BuffEffectManager.instance.GetChickenFeedTimer());
        data.SetPenguinFeedTimer(BuffEffectManager.instance.GetPenguinFeedTimer());
        data.SetBubbleGunTimer(BuffEffectManager.instance.GetBubbleGunTimer());
        data.SetMagnetTimer(BuffEffectManager.instance.GetMagnetTimer());
        Debug.Log("SaveNetworkUTCTimeCallback(): Going to save lastLoginTime: " + data.GetLastLoginTime());
        SaveLoadManager.instance.SaveData(data);

        // if calling from InitializeFirstTimeInstall()
        if (bootFirstTime)
        {
            Debug.Log("SaveNetworkUTCTimeCallback(): Saved data.. Finished Loading...");
            GameManager.instance.OnFinishedLoading();
        }
    }

    public void LoadGame()
    {
        SaveLoadManager.instance.LoadData();
    }

    public void NotifyAsync(DateTime currentTime)
    {
        asyncNotified = true;
        asyncReturnValue = currentTime;
    }

    void InitializeFirstTimeInstall()
    {
        // initialize all variables to 0
        PlayerPrefs.DeleteAll();
        isGameStartedFirstTime = false;
        bubbleGunCount = 0;
        magnetCount = 0;
        chickenFeedCount = 0;
        penguinFeedCount = 0;
        caughtChicken = 0;
        caughtPenguin = 0;
        totalItemsPurchasedToDate = 0;
        dailyRewardClaimed = false;
        SaveNetworkUTCTime();
    }

    // load game if user is not logged in to google play
    public void LoadGameVariables(GameData localData)
    {
        if (localData == null)
        {
            isGameStartedFirstTime = true;
        }

        if (isGameStartedFirstTime)
        {
            Debug.Log("GameDataManager: LoadGameVariables(): Detecting first time install.. initializing...");
            InitializeFirstTimeInstall();
        }

        else
        {
            if (bootFirstTime)
            {
                // get login time, check if user gets login reward
                Debug.Log("LoadGameVariables: bootFirstTime... requesting UTC time...");
                SaveNetworkUTCTime();
            }

            // user is not logged in and its not first time install, we just load the local data
            data = localData;
            isGameStartedFirstTime = data.GetIsGameStartedFirstTime();
            bubbleGunCount = data.GetBubbleGunCount();
            magnetCount = data.GetMagnetCount();
            chickenFeedCount = data.GetChickenFeedCount();
            penguinFeedCount = data.GetPenguinFeedCount();
            caughtChicken = data.GetCaughtChickenCount();
            caughtPenguin = data.GetCaughtPenguinCount();
            totalItemsPurchasedToDate = data.GetTotalItemsPurchasedToDate();
            lastLoginTime = data.GetLastLoginTime();
            dailyRewardClaimed = data.GetDailyRewardClaimed();
            loadedChickenFeedTimer = data.GetChickenFeedTimer();
            loadedPenguinFeedTimer = data.GetPenguinFeedTimer();
            loadedBubbleGunTimer = data.GetBubbleGunTimer();
            loadedMagnetTimer = data.GetMagnetTimer();
            // notify all the item class to start after finished loading
            BuffEffectManager.instance.chickenFeed.OnFinishedLoading();
            BuffEffectManager.instance.penguinFeed.OnFinishedLoading();
            BuffEffectManager.instance.bubbleGun.OnFinishedLoading();
            BuffEffectManager.instance.magnetBubble.OnFinishedLoading();
            Debug.Log("LoadGameVars(local): Finished loading");
            GameManager.instance.OnFinishedLoading();

            Debug.Log("LoadGameVariables: Finished loaded from localData: " +
            "caughtChicken: " + caughtChicken + " caughtPenguin: " + caughtPenguin +
            "chickenFeed: " + chickenFeedCount + " penguinFeed: " + penguinFeedCount + " bubbleGun: " + bubbleGunCount + " magnet: " + magnetCount +
            " total items purchased: " + totalItemsPurchasedToDate + " last login: " + lastLoginTime + " reward claimed: " + dailyRewardClaimed + 
            "chickenFeedTimer: " + loadedChickenFeedTimer + " penguinFeedTimer: " + loadedPenguinFeedTimer + " bubbleGunTimer: " + loadedBubbleGunTimer + " magnetTimer: " + loadedMagnetTimer);
        }
    }

    // load game if user is logged into google play
    // resolve conflict between local and cloud version
    public void LoadGameVariables(GameData cloudData, GameData localData)
    {
        bool isCloudDataLoaded = false;
        if (localData == null)
        {
            // load cloud Data
            isGameStartedFirstTime = cloudData.GetIsGameStartedFirstTime();
            bubbleGunCount = cloudData.GetBubbleGunCount();
            magnetCount = cloudData.GetMagnetCount();
            chickenFeedCount = cloudData.GetChickenFeedCount();
            penguinFeedCount = cloudData.GetPenguinFeedCount();
            caughtChicken = cloudData.GetCaughtChickenCount();
            caughtPenguin = cloudData.GetCaughtPenguinCount();
            totalItemsPurchasedToDate = cloudData.GetTotalItemsPurchasedToDate();
            lastLoginTime = cloudData.GetLastLoginTime();
            dailyRewardClaimed = cloudData.GetDailyRewardClaimed();
            loadedChickenFeedTimer = cloudData.GetChickenFeedTimer();
            loadedPenguinFeedTimer = cloudData.GetPenguinFeedTimer();
            loadedBubbleGunTimer = cloudData.GetBubbleGunTimer();
            loadedMagnetTimer = cloudData.GetMagnetTimer();
            isCloudDataLoaded = true;

            Debug.Log("LoadGameVariables(): cant find local save file.. loaded from cloud file");
        }

        else
        {
            GameData data;

            // first start comparing which version (cloud and local) is more up to date
            if (cloudData.GetTotalItemsPurchasedToDate() > localData.GetTotalItemsPurchasedToDate())
            {
                // pick cloud version
                Debug.Log("LoadGameVariables: cloudData is picked b/c total items is higher. " +
                    "cloudTotalItemsPurchased: " + cloudData.GetTotalItemsPurchasedToDate() + " localTotalItemsPurchased" + localData.GetTotalItemsPurchasedToDate());
                data = cloudData;
                isCloudDataLoaded = true;

            }
            else if (cloudData.GetTotalItemsPurchasedToDate() < localData.GetTotalItemsPurchasedToDate())
            {
                // pick local version
                Debug.Log("LoadGameVariables: localData is picked b/c total items is higher" +
                "cloudTotalItemsPurchased: " + cloudData.GetTotalItemsPurchasedToDate() + " localTotalItemsPurchased" + localData.GetTotalItemsPurchasedToDate());
                data = localData;
            }

            else
            {
                // pick the one that has higher caught item count
                if (cloudData.GetCaughtTotal() > localData.GetCaughtTotal())
                {
                    Debug.Log("LoadGameVariables: cloudData is picked b/c total caught is higher." +
                        "cloudGetCaughtTotal(): " + cloudData.GetCaughtTotal() + " localGetCaughtTotal(): " + localData.GetCaughtTotal());
                    data = cloudData;
                    isCloudDataLoaded = true;
                }
                else
                {
                    Debug.Log("LoadGameVariables: localData is picked b/c total caught is higher." +
                        "cloudGetCaughtTotal(): " + cloudData.GetCaughtTotal() + " localGetCaughtTotal(): " + localData.GetCaughtTotal());
                    data = localData;
                }
            }

            // then load into variables
            isGameStartedFirstTime = data.GetIsGameStartedFirstTime();
            bubbleGunCount = data.GetBubbleGunCount();
            magnetCount = data.GetMagnetCount();
            chickenFeedCount = data.GetChickenFeedCount();
            penguinFeedCount = data.GetPenguinFeedCount();
            caughtChicken = data.GetCaughtChickenCount();
            caughtPenguin = data.GetCaughtPenguinCount();
            totalItemsPurchasedToDate = data.GetTotalItemsPurchasedToDate();
            lastLoginTime = data.GetLastLoginTime();
            dailyRewardClaimed = cloudData.GetDailyRewardClaimed();
            loadedChickenFeedTimer = data.GetChickenFeedTimer();
            loadedPenguinFeedTimer = data.GetPenguinFeedTimer();
            loadedBubbleGunTimer = data.GetBubbleGunTimer();
            loadedMagnetTimer = data.GetMagnetTimer();

            // if local version is more up to date, replace cloud version (ie save this data to cloud)
            if (!isCloudDataLoaded)
            {
                Debug.Log("LoadGameVariables(): local version is used to load game vars");
                Debug.Log("GameManager: LoadGameVariables(): Local version is more up to date, replacing cloud version with local... caughtChicken: " + data.GetCaughtChickenCount() + " caughtPenguin: " + data.GetCaughtPenguinCount());
                SaveLoadManager.instance.SaveData(data);
            }
            else
            {
                Debug.Log("LoadGameVariables(): cloud version is used to load game vars");
            }
        }

        // notify all the item class to start after finished loading
        BuffEffectManager.instance.chickenFeed.OnFinishedLoading();
        BuffEffectManager.instance.penguinFeed.OnFinishedLoading();
        BuffEffectManager.instance.bubbleGun.OnFinishedLoading();
        BuffEffectManager.instance.magnetBubble.OnFinishedLoading();
        Debug.Log("LoadGameVars(cloud, local): Finished loading");
        GameManager.instance.OnFinishedLoading();

        Debug.Log("LoadGameVariables: Finished loaded... " +
            "caughtChicken: " + caughtChicken + " caughtPenguin: " + caughtPenguin +
            "chickenFeed: " + chickenFeedCount + " penguinFeed: " + penguinFeedCount + " bubbleGun: " + bubbleGunCount + " magnet: " + magnetCount +
            " total items purchased: " + totalItemsPurchasedToDate + " last login: " + lastLoginTime + " reward claimed: " + dailyRewardClaimed);
    }

    public GameData GetGameData()
    {
        return data;
    }

    // setters for catchables
    public void IncrementChickenCount()
    {
        caughtChicken++;
    }

    public void IncrementPenguinCount()
    {
        caughtPenguin++;
    }

    public void SetChickenCount(int _caughtChicken)
    {
        caughtChicken = _caughtChicken;
    }

    public void SetPenguinCount(int _caughtPenguin)
    {
        caughtPenguin = _caughtPenguin;
    }
        
    // setters for consumables
    public void IncrementPenguinFeedCount()
    {
        penguinFeedCount++;
        totalItemsPurchasedToDate++;
    }

    public void DecrementPenguinFeedCount()
    {
        penguinFeedCount--;
        penguinFeedCount = Mathf.Clamp(penguinFeedCount, 0, penguinFeedCount);
    }

    public void IncrementChickenFeedCount()
    {
        chickenFeedCount++;
        totalItemsPurchasedToDate++;
    }

    public void DecrementChickenFeedCount()
    {
        chickenFeedCount--;
        chickenFeedCount = Mathf.Clamp(chickenFeedCount, 0, chickenFeedCount);

    }

    public void IncrementBubbleGunCount()
    {
        bubbleGunCount++;
        totalItemsPurchasedToDate++;
    }

    public void DecrementBubbleGunCount()
    {
        bubbleGunCount--;
        bubbleGunCount = Mathf.Clamp(bubbleGunCount, 0, bubbleGunCount);

    }

    public void IncrementMagnetCount()
    {
        magnetCount++;
        totalItemsPurchasedToDate++;
    }

    public void DecrementMagnetCount()
    {
        magnetCount--;
        magnetCount = Mathf.Clamp(magnetCount, 0, magnetCount);
    }

    // setters for daily login
    public void SetDailyRewardClaimed(bool claimed)
    {
        dailyRewardClaimed = claimed;
    }

    // getters for catchables
    public int GetCaughtChickenCount()
    {
        return caughtChicken;
    }

    public int GetCaughtPenguinCount()
    {
        return caughtPenguin;
    }

    // getters for consumables
    public int GetChickenFeedCount()
    {
        return chickenFeedCount;
    }

    public int GetPenguinFeedCount()
    {
        return penguinFeedCount;
    }

    public int GetBubbleGunCount()
    {
        return bubbleGunCount;
    }

    public int GetMagnetCount()
    {
        return magnetCount;
    }

    // getters for consumable timers
    public float GetLoadedBubbleGunTimer()
    {
        return loadedBubbleGunTimer;
    }

    public float GetLoadedMagnetTimer()
    {
        return loadedMagnetTimer;
    }

    public float GetLoadedPenguinFeedTimer()
    {
        return loadedPenguinFeedTimer;
    }

    public float GetLoadedChickenFeedTimer()
    {
        return loadedChickenFeedTimer;
    }

    // getter for daily reward
    public bool GetDailyRewardClaimed()
    {
        return dailyRewardClaimed;
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("OnApplicationPause(): Start save");
            data = new GameData();
            data.SetIsGameStartedFirstTime(isGameStartedFirstTime);
            data.SetCaughtChickenCount(caughtChicken);
            data.SetCaugthPenguinCount(caughtPenguin);
            data.SetTotalItemsPurchasedToDate(totalItemsPurchasedToDate);
            data.SetChickenFeedCount(chickenFeedCount);
            data.SetPenguinFeedCount(penguinFeedCount);
            data.SetBubbleGunCount(bubbleGunCount);
            data.SetMagnetCount(magnetCount);
            data.SetLastLoginTime(lastLoginTime);
            data.SetDailyRewardClaimed(dailyRewardClaimed);
            data.SetChickenFeedTimer(BuffEffectManager.instance.GetChickenFeedTimer());
            data.SetPenguinFeedTimer(BuffEffectManager.instance.GetPenguinFeedTimer());
            data.SetBubbleGunTimer(BuffEffectManager.instance.GetBubbleGunTimer());
            data.SetMagnetTimer(BuffEffectManager.instance.GetMagnetTimer());
            SaveLoadManager.instance.SaveLocal(data);
            Debug.Log("OnApplicationPause(): End Save");
        }
    }

    public IEnumerator RunOnMainThread()
    {
        while(!asyncNotified)
            yield return null;

        if (asyncNotified)
        {
            Debug.Log("RunOnMainThread(): Got notified!");
            asyncNotified = false;
            SaveNetworkUTCTimeCallback(asyncReturnValue);
        }

    }
}
