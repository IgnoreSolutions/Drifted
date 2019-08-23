using UnityEngine;
using System.Collections;
using Drifted.Inventory;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Drifted;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SerializableQuaternion
{
    public float x, y, z, w;

    public SerializableQuaternion()
    {

    }

    public SerializableQuaternion(float _x, float _y, float _z, float _w)
    {
        x = _x;
        y = _y;
        z = _z;
        w = _w;
    }

    public SerializableQuaternion(Quaternion quat)
    {
        x = quat.x;
        y = quat.y;
        z = quat.z;
        w = quat.w;
    }

    public Quaternion ToUnityQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

[System.Serializable]
public class PlayerInformation
{
    public SerializableVector3 playerPosition;
    public SerializableQuaternion playerRotation;

    public SerializableItemContainer[] inventory;
}

[System.Serializable]
public class SaveData
{
    string sceneName;
    PlayerInformation playerInformation;

    public SaveData(PlayerMovement player, InventoryHandler inventory)
    {
        playerInformation = new PlayerInformation();

        playerInformation.playerPosition = new SerializableVector3(player.gameObject.transform.position);
        playerInformation.playerRotation = new SerializableQuaternion(player.gameObject.transform.rotation);

        playerInformation.inventory = inventory.SerializeInventory();
        sceneName = SceneManager.GetActiveScene().name;
    }

    public string GetSceneName() => sceneName;

    public SaveData()
    {
        playerInformation = null;
    }



    public PlayerInformation GetPlayer() => playerInformation;
}

public class SaveSystem2 : MonoBehaviour
{
    [SerializeField]
    string DirectoryPrefix = "saves";

    void test()
    {
        string fileName = DirectoryPrefix.Substring(DirectoryPrefix.LastIndexOf('/'));
    }

    public void SaveData(int slot = 0)
    {
        // TODO: slots
        string fullFileName = $"./{DirectoryPrefix}/drifted_save.dat";
        string directoryName = fullFileName.Substring(0, (fullFileName.LastIndexOf('/')));

        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

        /*
        SaveData save = new SaveData(DriftedConstants.Instance.Player().Movement, DriftedConstants.Instance.Player().Inventory.Inventory);
        if(WriteSaveData(save, fullFileName))
        {
            DriftedConstants.Instance.UI().Console.AddLine($"Saved data successfully!");
        }
        else
        {
            DriftedConstants.Instance.UI().Console.AddLine($"Couldn't save data!");
        }
        */
    }

    private bool WriteSaveData(SaveData data, string path)
    {
        if (data == null) return false;

        Stream saveDataFile = null;
        try
        {
            saveDataFile = File.Create(path);
        }
        catch (IOException ioExc)
        {
            Debug.LogError($"Exception while opening file stream at {path} \n" + ioExc.Message);
        }

        if (saveDataFile == null) return false;

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(saveDataFile, data);
        saveDataFile.Close();

        return true;
    }

    private SaveData ReadSaveData(string path)
    {
        Stream saveDataFile = null;
        try
        {
            saveDataFile = File.OpenRead(path);
        }
        catch (IOException ioExc)
        {
            Debug.LogError($"Exception while opening file stream at {path} \n" + ioExc.Message);
        }

        if (saveDataFile == null) return null;

        BinaryFormatter formatter = new BinaryFormatter();
        SaveData data = (SaveData)formatter.Deserialize(saveDataFile);

        return data;
    }

    public void LoadData(int slot = 0)
    {
        SaveData loadedData = ReadSaveData($"{DirectoryPrefix}/drifted_save.dat");
        if (loadedData != null)
        {
            SetPlayer(loadedData);
            DriftedConstants.Instance.UI().Console.AddLine($"Loaded data successfully!");
        }
        else
        {
            DriftedConstants.Instance.UI().Console.AddLine($"Couldn't load data!");
        }
    }

    private void SetPlayer(SaveData saveData)
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(DriftedConstants.Instance);
        DriftedConstants.Instance.LoadScene(saveData.GetSceneName());

        DriftedConstants.Instance.Player().Movement.SetPosition(saveData.GetPlayer().playerPosition, saveData.GetPlayer().playerRotation);
        //DriftedConstants.Instance.Player().Inventory.Inventory.DeserializeInventory(saveData.GetPlayer().inventory);
    }
}
