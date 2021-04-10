using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTravel : MonoBehaviour
{
    static public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    static public void GoToScene(int sceneIdx)
    {
        SceneManager.LoadScene(sceneIdx);
    }

    static public void Quit()
    {
        Application.Quit();
    }
}
