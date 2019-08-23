using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
//        if (Drifted.DriftedConstants.Instance.UIFocused || Drifted.DriftedConstants.Instance.FullScreenUIActive) return;

        if (Input.GetKeyDown(KeyCode.Escape)) // TODO: This is wrong
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // method of resuming game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);

        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnResumeGame", SendMessageOptions.DontRequireReceiver);
        }

        GameIsPaused = false;
    }

    // method of pausing game
    public void Pause()
    {
        pauseMenuUI.SetActive(true);

        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
        }

        GameIsPaused = true;
    }

    //method of loading the game
    public void Load()
    {
        Debug.Log("Loading...");
        //TODO: put the load scene here
    }

    //method of quitting the game
    public void Quit()
    {
        Debug.Log("Quit...");
        UnityEngine.Application.Quit();
    }

    // method of going back to the main menu
    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    //method of saving the game
    public void Save()
    {
        Debug.Log("Saving...");
        //TODO: put the save scene here
    }
}
