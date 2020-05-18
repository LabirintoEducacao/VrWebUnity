using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInGame : MonoBehaviour
{
    public void backToMenu(string scene)
    {
        AudioList.instance.PlayButtonClick();
        SceneManager.LoadScene(scene);
    }

    public void quitGame()
    {
        AudioList.instance.PlayButtonClick();
        Application.Quit();
    }
}
