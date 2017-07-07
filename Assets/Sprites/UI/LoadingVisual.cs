using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingVisual : MonoBehaviour {

    Text loadText;
    int textCount = 0;

	void Start () {
        loadText = transform.Find("LoadText").GetComponent<Text>();
	}
	
	void FixedUpdate () {
		if (textCount < 50)
        {
            if (textCount % 10 == 0)
                loadText.text += ".";
            textCount++;
        }
        else
        {
            loadText.text = "Loading";
            textCount = 0;
        }
	}
}
