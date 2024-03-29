﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinFly : MonoBehaviour {
    GameObject Target;
    Vector2 dest;
    Vector3 finalScale = new Vector3(0,0,0);
    Type objType;

    public enum Type
    {
        CHICKEN,
        PENGUIN
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, dest, 0.3f);
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, finalScale, Time.deltaTime*2);
        if (gameObject.transform.position.y > 4f)
        {
            if (objType == Type.CHICKEN)
                GameDataManager.instance.IncrementChickenCount();
            if (objType == Type.PENGUIN)
                GameDataManager.instance.IncrementPenguinCount();
            PoolManager.instance.ReturnObjectToPool(gameObject);
        }
    }

    public void InitParams(Vector2 position, Type type)
    {
        transform.position = position;
        if (objType == Type.CHICKEN)
            dest = new Vector2(-2.756f, 4.541f);
        if (objType == Type.PENGUIN)
            dest = new Vector2(-0.449f, 4.541f);
        objType = type;
    }
}
