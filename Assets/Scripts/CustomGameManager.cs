using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    // fields
    public static bool LoadingFile = false;

    // properties
    public static Game_Manager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //Screen.SetResolution(1024, 768, false);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: disable once verified
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadScene("StartMenu");
            SceneManager.UnloadSceneAsync("MainSceneTest");
        }
    }

    // SCENE MANAGEMENT

    public void LoadScene_NewGame(string scene)
    {
        Game_Manager.LoadingFile = false;
        SceneManager.LoadScene(scene);
    }

    public void LoadScene(string scene)
    {
        Game_Manager.LoadingFile = true;
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        UnityEngine.Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
