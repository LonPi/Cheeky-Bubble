using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinFly : MonoBehaviour {
    GameObject Target;
    Vector2 dest;
    Vector3 finalScale = new Vector3(0,0,0);
    
	// Use this for initialization
	void Start ()
    {
        if (gameObject.name == "Chicken_Drop")
            dest = new Vector2(-2.756f, 4.541f);
        else if (gameObject.name == "Penguin_Drop")
            dest = new Vector2(-0.449f, 4.541f);
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, dest, 1.0f);
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, finalScale, Time.deltaTime*2);
        if ((Vector2)gameObject.transform.position == (Vector2)dest)
        {
            if (gameObject.name == "Chicken_Drop")
                GameDataManager.instance.IncrementChickenCount();
            else if (gameObject.name == "Penguin_Drop")
                GameDataManager.instance.IncrementPenguinCount();
            Destroy(gameObject);
        }
    }
}
