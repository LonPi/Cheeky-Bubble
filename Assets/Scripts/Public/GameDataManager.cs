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
    int bubbleGunCount;
    int magnetCount;
    int chickenFeedCount;
    int penguinFeedCount;
    int caughtChicken;
    int caughtPenguin;
    int totalItemsPurchasedToDate;
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
        bootFirstTime = true;
    }

    private void Start()
    {
        // only load data if we close and start the game (not resume)
        if (bootFirstTime)
        {
            LoadGame();
            bootFirstTime = false;
        }
        else
        {
            SaveGame();
        }
    }

    public void OnSceneLoaded()
    {
        // save game every time we load a scene
        if (!bootFirstTime)
            SaveGame();
    }

    public void SaveGame()
    {
        Debug.Log("saving game: variable caughtChicken: " + caughtChicken + " variable caughtPenguin: " +
            caughtPenguin + " total items purchased: " + totalItemsPurchasedToDate + " last login: " + lastLoginTime);

        // Save all caught items and inventory items
        data = new GameData();
        data.SetCaughtChickenCount(caughtChicken);
        data.SetCaugthPenguinCount(caughtPenguin);
        data.SetTotalItemsPurchasedToDate(totalItemsPurchasedToDate);
        data.SetChickenFeedCount(chickenFeedCount);
        data.SetPenguinFeedCount(penguinFeedCount);
        data.SetBubbleGunCount(bubbleGunCount);
        data.SetMagnetCount(magnetCount);
        // Get UTC time
        // first check from PlayerPref and compare to current device clock
        // to determine if its necessary to get network time from server

        NetworkHandler.GetNetworkUTCTimeAsync();
        Debug.Log("starting coroutine...");
        StartCoroutine(RunOnMainThread());
        Debug.Log("after starting coroutine..");
    }

    // called after async job is finished
    public void SaveGame(DateTime currentTime)
    {
        Debug.Log("SaveGame(): currentTime.Date=" + currentTime + " lastloginTime.Date=" + lastLoginTime);
        // check if current UTC time is the same day as the one in our saved file
        if (currentTime.Date > lastLoginTime.Date)
        {
            Debug.LogError("SaveGame callback: current date: " + currentTime.Date + " last login time: " + lastLoginTime.Date);
            // update time
            lastLoginTime = currentTime;
            // TODO: give out login bonus
            Debug.LogError(data.GetLastLoginTime());
        }

        // continue saving our data
        data.SetLastLoginTime(lastLoginTime);
        Debug.Log("data: " + data.GetLastLoginTime());

        SaveLoadManager.instance.SaveData(data);
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

    // getters for catchables
    public int GetCaughtBirdCount()
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

    public void LoadGameVariables(GameData localData)
    {
        if (localData == null)
        {
            isGameStartedFirstTime = true;
        }

        if (isGameStartedFirstTime)
        {
            Debug.Log("GameDataManager: LoadGameVariables(): Detecting first time install.. initializing...");
            // initialize all variables to 0
            isGameStartedFirstTime = false;
            bubbleGunCount = 0;
            magnetCount = 0;
            chickenFeedCount = 0;
            penguinFeedCount = 0;
            caughtChicken = 0;
            caughtPenguin = 0;
            totalItemsPurchasedToDate = 0;
            // start async to get UTC time
            NetworkHandler.GetNetworkUTCTimeAsync();
            Debug.Log("starting RunOnMainThread() coroutine...");
            StartCoroutine(RunOnMainThread());
            Debug.Log("after RunOnMainThread() starting coroutine..");
            data = new GameData();
            data.SetIsGameStartedFirstTime(isGameStartedFirstTime);
            data.SetBubbleGunCount(bubbleGunCount);
            data.SetMagnetCount(magnetCount);
            data.SetChickenFeedCount(chickenFeedCount);
            data.SetPenguinFeedCount(penguinFeedCount);
            data.SetCaughtChickenCount(caughtChicken);
            data.SetCaugthPenguinCount(caughtPenguin);
            data.SetTotalItemsPurchasedToDate(totalItemsPurchasedToDate);

            Debug.Log("GameDataManager: LoadGameVariables(): Saving initialized data...");
            SaveLoadManager.instance.SaveData(data);
            Debug.Log("GameDataManager: LoadGameVariables(): Loading back data...");
            SaveLoadManager.instance.LoadData();
        }
        else
        {
            // user is not online, we just load the local data
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
        }

        Debug.Log("LoadGameVariables: Finished loaded from localData: " +
            "caughtChicken: " + caughtChicken + " caughtPenguin: " + caughtPenguin + 
            " total items purchased: " + totalItemsPurchasedToDate + " last login: " + lastLoginTime);
    }

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

            // if local version is more up to date, replace cloud version
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

        Debug.Log("GameManager: LoadGameVariables(): Finally loaded.. caughtChicken: " + caughtChicken + " caughtPenguin: " + caughtPenguin);
    }

    public IEnumerator RunOnMainThread()
    {
        while(!asyncNotified)
            yield return null;

        if (asyncNotified)
        {
            Debug.Log("RunOnMainThread(): Got notified!");
            asyncNotified = false;
            SaveGame(asyncReturnValue);
        }

    }
}
