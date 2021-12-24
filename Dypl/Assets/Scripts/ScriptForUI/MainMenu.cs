using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Test_1");
    }

    public void Settings()
    {
        //SceneManager.LoadScene("SettingsScene");
    }

    public void ExitFromGame()
    {
        Application.Quit();
    }
}
