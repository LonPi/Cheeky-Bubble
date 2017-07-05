using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    int prefabInstanceID;

    public void SetParams(int id)
    {
        this.prefabInstanceID = id;
    }

    public int GetPrefabInstanceID()
    {
        return this.prefabInstanceID;
    }
}
