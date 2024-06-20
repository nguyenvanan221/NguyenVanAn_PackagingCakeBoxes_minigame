using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    //UI
    public GameObject MainMenuUI;
    public GameObject GameUI;
    public GameObject inGameUI;
    public GameObject levelUI;
    public GameObject levelCompleteUI;
    public GameObject levelFailedUI;

    public void ShowUI(GameObject newUI)
    {
        GameObject[] allUI = { inGameUI, levelUI, levelCompleteUI, levelFailedUI};

        foreach (GameObject uI in allUI)
        {
            uI.SetActive(false);
        }

        newUI.SetActive(true);
    }

    public void ShowGameUI()
    {
        MainMenuUI.SetActive(false);
        GameUI.SetActive(true);
        ShowUI(levelUI);
    }

}
