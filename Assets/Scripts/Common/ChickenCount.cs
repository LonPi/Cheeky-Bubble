using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChickenCount : MonoBehaviour {

    Text chickenCount;

	// Use this for initialization
	void Start ()
    {
        chickenCount = transform.Find("chicken_count").GetComponent<Text>();	
	}
	
	// Update is called once per frame
	void Update () {
        chickenCount.text = GameDataManager.instance.GetCaughtChickenCount().ToString();
	}
}
