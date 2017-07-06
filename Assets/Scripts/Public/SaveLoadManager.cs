using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    const string SAVE_NAME = "Bubble";
    bool isSaving; // boolean to indicate whether we are saving or loading

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void SaveLocal(GameData data)
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/SavedGame.dat");
            Debug.Log("SaveLoadManager: last login time: " + data.GetLastLoginTime());
            Debug.Log("SaveLoadManager: SaveLocal(): saving save file into " + Application.persistentDataPath + "/SavedGame.dat");
            bf.Serialize(file, data);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            if (file != null)
                file.Close();
        }
    }

    public GameData LoadLocal()
    {
        FileStream file = null;
        GameData data = null;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/SavedGame.dat", FileMode.Open);
            Debug.Log("SaveLoadManager: LoadLocal(): loading save file from " + Application.persistentDataPath + "/SavedGame.dat");
            data = (GameData)bf.Deserialize(file);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            if (file != null)
                file.Close();

        }
        Debug.Log("SaveLoadManager(): LoadLocal(): data returned: " + data);
        return data;
    }

    // handles both cloud and local save
    public void SaveData(GameData data)
    {
#if UNITY_STANDALONE
        SaveLocal(data);
#endif

#if UNITY_ANDROID

        // always try to save online first
        if (Social.localUser.authenticated)
        {
            Debug.Log("SaveData(): user is authenticated.. Running OpenWithManualConflictResolution()");
            isSaving = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }

        else
        {
            Debug.Log("SaveData(): user is not authenticated.. running SaveLocal()..");
            SaveLocal(data);
        }
#endif
    }

    // handles both cloud and local load
    // will call GameManager.instance.LoadGameVariables() upon loaded
    public void LoadData()
    {

#if UNITY_STANDALONE
        LoadLocal();
#endif

#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Debug.Log("LoadData(): user is authenticated.. Running OpenWithManualConflictResolution()");
            isSaving = false;
            // will call GameManager.instance.LoadGameVariables once all chained callbacks are finished
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }

        else
        {
            Debug.Log("LoadData(): user is not authenticated.. running LoadLocal()...");
            GameData data = LoadLocal();
            GameDataManager.instance.LoadGameVariables(data);
        }
#endif
    }

#if UNITY_ANDROID
    private void ResolveConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData,
        ISavedGameMetadata unmerged, byte[] unmergedData)
    {
        if (originalData == null)
            resolver.ChooseMetadata(unmerged);
        else if (unmergedData == null)
            resolver.ChooseMetadata(original);

        else
        {
            BinaryFormatter bf = null;
            MemoryStream memStream = null;
            GameData originalObj = null;
            GameData unmergedObj = null;

            // Deserialize originalData 
            bf = new BinaryFormatter();
            memStream = new MemoryStream();
            memStream.Write(originalData, 0, originalData.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            originalObj = (GameData)bf.Deserialize(memStream);

            // Deserialize unmergedData
            bf = new BinaryFormatter();
            memStream = new MemoryStream();
            memStream.Write(unmergedData, 0, unmergedData.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            unmergedObj = (GameData)bf.Deserialize(memStream);

            // resolving strategy: pick the one with highest totalItemsPurchasedToDate
            // if both totalItemsPurchasedToDate are equal, pick the one with higher caughtCount
            if (originalObj.GetTotalItemsPurchasedToDate() > unmergedObj.GetTotalItemsPurchasedToDate())
            {
                resolver.ChooseMetadata(original);
                return;
            }
            else if (unmergedObj.GetTotalItemsPurchasedToDate() > originalObj.GetTotalItemsPurchasedToDate())
            {
                resolver.ChooseMetadata(unmerged);
                return;
            }
            else
            {
                if (originalObj.GetCaughtTotal() > unmergedObj.GetCaughtTotal())
                {
                    resolver.ChooseMetadata(original);
                }
                else
                {
                    resolver.ChooseMetadata(unmerged);
                }
            }
        }
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        // forcing cloud save for now
        if (status == SavedGameRequestStatus.Success)
        {
            if (!isSaving)
                LoadGame(game);
            else
                SaveGame(game);
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        Debug.Log("SaveGameManager: LoadGame(): loading game from cloud..");
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void SaveGame(ISavedGameMetadata game)
    {
        // Serialize game data
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream memStream = new MemoryStream();
        // get game data from Game Manager
        GameData data = GameDataManager.instance.GetGameData();
        Debug.Log("SaveLoadManager: SaveGame(): saving chicken count: " + data.GetCaughtChickenCount() + " penguin count: " + data.GetCaughtPenguinCount() + 
            "claimed reward: " + data.GetDailyRewardClaimed() + " last login time: " + data.GetLastLoginTime() + " to cloud");
        bf.Serialize(memStream, data);
        byte[] dataToSave = memStream.ToArray();
        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

        // upload to cloud
        Debug.Log("SaveLoadManager: SaveGame(): uploading to cloud..");
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, dataToSave, OnSavedGameDataWritten);

        // save a local copy as well
        Debug.Log("SaveLoadManager: SaveGame(): save a local copy after uploading to cloud..");
        SaveLocal(data);
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // Deserialize byte array
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            GameData cloudData;
            memStream.Write(savedData, 0, savedData.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            cloudData = (GameData)bf.Deserialize(memStream);

            // Get local copy of data
            GameData localData = LoadLocal();

            // load variable to game
            Debug.Log("SaveGameManager: OnSavedGameDataRead: finally desererializing data");
            GameDataManager.instance.LoadGameVariables(cloudData, localData);
        }
        else
        {
            Debug.Log("OnSavedGameDataRead(): Error: " + status);
        }
    }

    private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("OnSavedGameDataWritten(): Successfully uploaded to cloud.");
        }
        else
        {
            Debug.Log("OnSavedGameDataWritten(): Error occured while uploading to cloud. status: " + status);
        }
    }
#endif
}

[Serializable]
public class GameData
{
    // game variables
    int caughtChickenCount;
    int caughtPenguinCount;
    bool isGameStartedFirstTime;
    int bubbleGunCount;
    int magnetCount;
    int chickenFeedCount;
    int penguinFeedCount;
    int totalItemsPurchasedToDate;
    DateTime lastLoginTime;
    bool dailyRewardClaimed;

    // getters
    public DateTime GetLastLoginTime()
    {
        // set utc login time (the earliest time of the day of which user log in)
        return lastLoginTime;
    }

    public bool GetDailyRewardClaimed()
    {
        return dailyRewardClaimed;
    }

    public int GetCaughtChickenCount()
    {
        return caughtChickenCount;
    }

    public int GetCaughtPenguinCount()
    {
        return caughtPenguinCount;
    }

    public int GetCaughtTotal()
    {
        int total = caughtChickenCount + caughtPenguinCount;
        return total;
    }

    public bool GetIsGameStartedFirstTime()
    {
        return isGameStartedFirstTime;
    }

    public int GetBubbleGunCount()
    {
        return bubbleGunCount;
    }

    public int GetMagnetCount()
    {
        return magnetCount;
    }

    public int GetChickenFeedCount()
    {
        return chickenFeedCount;
    }

    public int GetPenguinFeedCount()
    {
        return penguinFeedCount;
    }

    public int GetTotalItemsPurchasedToDate()
    {
        return totalItemsPurchasedToDate;
    }

    // setters
    public void SetLastLoginTime(DateTime date)
    {
        // set utc login time (the earliest time of the day of which user log in)
        lastLoginTime = date;
    }

    public void SetDailyRewardClaimed(bool claimed)
    {
        dailyRewardClaimed = claimed;
    }

    public void SetIsGameStartedFirstTime(bool isGameStartedFirstTime)
    {
        this.isGameStartedFirstTime = isGameStartedFirstTime;
    }

    public void SetCaughtChickenCount(int count)
    {
        this.caughtChickenCount = count;
    }

    public void SetCaugthPenguinCount(int count)
    {
        this.caughtPenguinCount = count;
    }

    public void SetBubbleGunCount(int count)
    {
        this.bubbleGunCount = count;
    }

    public void SetMagnetCount(int count)
    {
        this.magnetCount = count;
    }

    public void SetChickenFeedCount(int count)
    {
        this.chickenFeedCount = count;
    }

    public void SetPenguinFeedCount(int count)
    {
        this.penguinFeedCount = count;
    }

    public void SetTotalItemsPurchasedToDate(int total)
    {
        this.totalItemsPurchasedToDate = total;
    }

}