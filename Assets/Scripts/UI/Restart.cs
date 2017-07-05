using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Restart : MonoBehaviour {

    public void PressRestart()
    {
        GameManager.instance.Restart();
    }
}
