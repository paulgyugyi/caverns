using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    public GameObject popupMenu = null;
    public GameObject continueButton = null;
    private float oldTimeScale;
    GameObject savedFocus = null;


    void Start()
    {
        
    }

    bool MenuActive()
    {
        if (popupMenu != null && popupMenu.activeSelf)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!MenuActive())
            {
                oldTimeScale = Time.timeScale;
                Time.timeScale = 0;
                savedFocus = EventSystem.current.currentSelectedGameObject;
                popupMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);
            }
            else
            {
                MenuContinue();
            }
        }
    }
    public void MenuContinue()
    {
        Time.timeScale = oldTimeScale;
        popupMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(savedFocus);
    }

    public void MenuMain()
    {
        Time.timeScale = oldTimeScale;
        SceneManager.LoadScene(0);
    }

}
