using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

    public static PoolManager instance = null;
    public GameObject[] prefabs;
    public int InitialPoolSize;
    public bool IsInitialized { get; private set; }

    Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CreateObjectPool();
        IsInitialized = true;
    }

    public GameObject GetObjectfromPool(GameObject prefab)
    {
        //Debug.Log("GetObjectFromPool:" +
         //   "prefab id: " + prefab.GetInstanceID() + " name: " + prefab.name);
        int poolKey = prefab.GetInstanceID();
        if (poolDictionary[poolKey].Count  == 0)
            CreateNewObjectIntoPool(prefab);

        GameObject reusedObject = poolDictionary[poolKey].Dequeue();
        reusedObject.SetActive(true);
        return reusedObject;
    }

    GameObject CreateNewObjectIntoPool(GameObject prefab)
    {
        int poolKey = prefab.GetInstanceID();
        //Debug.Log("CreateNewObjectIntoPool: " +
        //    "prefab id: " + prefab.GetInstanceID() + " name: " + prefab.name);

        GameObject newObject = Instantiate(prefab, transform);
        newObject.GetComponent<PoolObject>().SetParams(poolKey);
        poolDictionary[poolKey].Enqueue(newObject);
        newObject.SetActive(false);
        return newObject;
    }

    public void ReturnObjectToPool(GameObject objectInstance)
    {
        int poolKey = objectInstance.GetComponent<PoolObject>().GetPrefabInstanceID();
        poolDictionary[poolKey].Enqueue(objectInstance);
        objectInstance.SetActive(false);
    }

    void CreateObjectPool()
    {
        foreach(GameObject prefab in prefabs)
        {
            //Debug.Log("CreateObjectPool: " +
            //"prefab id: " + prefab.GetInstanceID() + " name: " + prefab.name);
            int poolKey = prefab.GetInstanceID();
            if (!poolDictionary.ContainsKey(poolKey))
                poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < InitialPoolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, transform);
                newObject.GetComponent<PoolObject>().SetParams(poolKey);
                poolDictionary[poolKey].Enqueue(newObject);
                newObject.SetActive(false);
            }
        }
    }

    public void OnSceneLoaded()
    {
        foreach (Transform child in transform)
        {
            // avoid adding multiple objects of the same instance id when reloading scene
            if (child.gameObject.activeInHierarchy)
            {
                ReturnObjectToPool(child.gameObject);
            }
        }

    }

    // used for debugging
    public bool isInPool(GameObject objectInstance)
    {
        PoolObject poolObjectScript = objectInstance.GetComponent<PoolObject>();
        Queue<GameObject> q = poolDictionary[poolObjectScript.GetPrefabInstanceID()];

        foreach(GameObject gameObj in q)
        {
            if (gameObj.GetInstanceID() == objectInstance.GetInstanceID())
            {
                return true;
            }
        }

        return false;
    }




}
