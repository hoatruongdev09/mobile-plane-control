using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour {
    public static SaveLoadManager Instance { get; private set; }

    [SerializeField]
    private UserData userData;

    private void Start () {
        userData = Load ();
        if (userData == null) {
            userData = new UserData ();
        }
        Instance = this;

    }
    public LevelData GetLevelData (string buildSceneName) {
        LevelData lvData = userData.listLevelData.Find (x => x.buildSceneName == buildSceneName);
        return lvData;
    }
    public void AddLevelData (LevelData lvData) {
        userData.listLevelData.Add (lvData);
    }
    public void AddTotalLandedPlane (float amount) {
        userData.landedPlane += amount;
    }
    public void SaveUserData () {
        Save (userData);
    }
    public static UserData Load () {
        string path = Application.persistentDataPath + "/userdata.exe";
        Debug.Log (path);
        if (File.Exists (path)) {
            BinaryFormatter formater = new BinaryFormatter ();
            FileStream stream = new FileStream (path, FileMode.Open);
            UserData info = formater.Deserialize (stream) as UserData;
            stream.Close ();
            Debug.Log ("Load location: " + path);
            return info;
        } else {
            Debug.Log ("no file saved");
            return null;
        }
    }
    public void Save (UserData data) {
        string path = Application.persistentDataPath + "/userdata.exe";
        if (File.Exists (path)) {
            File.Delete (path);
        }
        BinaryFormatter fommater = new BinaryFormatter ();
        FileStream stream = new FileStream (path, FileMode.Create);
        fommater.Serialize (stream, data);
        stream.Close ();
        Debug.Log ("Save location: " + path);
    }
}