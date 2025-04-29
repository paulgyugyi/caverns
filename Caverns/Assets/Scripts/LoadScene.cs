using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public int sceneToLoad = 0;
    public void Load()
    {
        if (sceneToLoad < 0)
        {
            // Quit Game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        else
        {

            SceneManager.LoadScene(sceneToLoad);
        }
    }

}
