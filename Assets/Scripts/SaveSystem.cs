using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class SaveSystem
{



   public static void SavePlayer (GameObject player)
    {
       
        BinaryFormatter formatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/player.drifted";
        string path = @"player.drifted";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        //string path = Application.persistentDataPath + "/player.drifted";
        string path = @"player.drifted";
        if (File.Exists(path))
        {
            Debug.Log("Loading");
            IFormatter formatter = new BinaryFormatter();
            using (Stream strm = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                PlayerData playerData = (PlayerData)formatter.Deserialize(strm);
                Debug.Log(playerData.PlayerPosition.ToUnityVector());
                return playerData;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
