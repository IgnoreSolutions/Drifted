using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using UnityEngine;

public class SaveSystemData : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    private void Awake()
    {
        if (player == null) player = DriftedConstants.Instance.Player().Marty;

        /*
        if(Game_Manager.LoadingFile == true)
        {
            Debug.Log("Loading existing player pos data");
            LoadPlayer();
            Game_Manager.LoadingFile = false;
        }
        */

    }
    public void SavePlayer()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Saving position");
            Debug.Log(player.transform.position);
            SaveSystem.SavePlayer(player);
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            this.LoadPlayer();
        }
        else { };
    }

    public void Update()
    {
        SavePlayer();
    }

    public void LoadPlayer()
    {
        Debug.Log("Loading file..");
        PlayerData data = SaveSystem.LoadPlayer();
        player.transform.position = data.PlayerPosition.ToUnityVector();

        Debug.Log("Set playing position to " + data.PlayerPosition.ToString());
    }
}
