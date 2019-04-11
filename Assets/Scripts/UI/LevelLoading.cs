using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelLoading : MonoBehaviour
{
    public static bool loadedData;
    [SerializeField] private UserData userData;
    [SerializeField] private SceneInfo[] listScenes;
    private List<LevelData> listUnlockedLevel;
    [SerializeField] private List<LevelInfo> listLevelInfo;

    private void Start()
    {
        userData = SaveLoadManager.Load();
        if (userData == null)
        {
            userData = new UserData();
        }
        StartCoroutine(WaitForDataLoaded());
    }
    public List<LevelInfo> GetLevelInfos()
    {
        return listLevelInfo;
    }

    private void LoadUnlockedLevel()
    {
        listUnlockedLevel = new List<LevelData>();
        foreach (SceneInfo si in listScenes)
        {
            if (si.scoreToUnlock <= userData.landedPlane)
            {
                listUnlockedLevel.Add(new LevelData(si.sceneName, 0, null));
            }
        }
    }
    private List<LevelInfo> ListAllLevel()
    {
        List<LevelInfo> lstLevelInfo = new List<LevelInfo>();
        LevelInfo lvInfo;
        foreach (SceneInfo sceneInfo in listScenes)
        {
            lvInfo = new LevelInfo(sceneInfo.sceneName, sceneInfo.scoreToUnlock, sceneInfo.scoreToUnlock - (int)userData.landedPlane, sceneInfo.difficult, GetLevelHighScore(sceneInfo.sceneName), GetLevelSpecialScore(sceneInfo.sceneName));
            lvInfo.SetUnlock((int)userData.landedPlane);
            lstLevelInfo.Add(lvInfo);
        }
        return lstLevelInfo;
    }
    private int GetLevelHighScore(string lvName)
    {
        LevelData lvData = userData.FindLevelByName(lvName);
        if (lvData != null)
        {
            return lvData.highScore;
        }
        else
        {
            return 0;
        }
    }
    public ScoreInfo[] GetLevelSpecialScore(string lvName)
    {
        LevelData lvData = userData.FindLevelByName(lvName);
        if (lvData != null)
        {
            return lvData.specialScore;
        }
        else
        {
            return null;
        }
    }
    private IEnumerator WaitForDataLoaded()
    {
        loadedData = false;
        yield return new WaitUntil(() => userData != null);
        LoadUnlockedLevel();
        listLevelInfo = ListAllLevel();
        loadedData = true;
    }
}

[System.Serializable]
public class LevelInfo
{
    public string levelName;
    public int scoreToUnlock;
    public ScoreInfo[] specialScore;
    public int remainToUnlock;
    public int difficult;
    private DifficultConst difficultClass;
    public int highScore;
    public bool isUnlocked;

    public LevelInfo(string levelName, int scoreToUnlock, int remainToUnlock, int difficult, int highScore, ScoreInfo[] specialScore)
    {
        this.levelName = levelName;
        this.scoreToUnlock = scoreToUnlock;
        this.difficult = difficult;
        this.highScore = highScore;
        this.remainToUnlock = remainToUnlock;
        this.specialScore = specialScore;
    }
    public void SetUnlock(int landedPlane)
    {
        isUnlocked = landedPlane >= scoreToUnlock;
    }
}

[System.Serializable]
public class SceneInfo
{
    public string sceneName;
    [Range(1, 10)]
    public int difficult;
    public int scoreToUnlock;

    public SceneInfo(string scene, int scoreToUnlock)
    {
        this.sceneName = scene;
        this.scoreToUnlock = scoreToUnlock;
    }
}