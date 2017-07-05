using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCanvas : MonoBehaviour
{

    public void PressSave()
    {
        GameDataManager.instance.SaveGame();
    }

    public void PressLoad()
    {
        GameDataManager.instance.LoadGame();
    }

    public void GoToCompetitive()
    {
        GameManager.instance.GoToScene("competitive");
    }

    public void GoToCasual()
    {
        GameManager.instance.GoToScene("casual");
    }

    public void GoToShop()
    {
        GameManager.instance.GoToScene("shop");
    }
}
