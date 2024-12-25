using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour, IMapSelectViewDatasource, IMainTitleDatasource
{
    public static MainGameManager Instance { get; set; }
    public bool isTest = true;
    public MainUiManager mainUiManager;
    [SerializeField] private DataManager dataManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        Application.targetFrameRate = 300;
        dataManager = DataManager.Instance;
        mainUiManager.mainTitlePanel.mapSelectView.Datasource = this;
        mainUiManager.mainTitlePanel.Datasource = this;
        StartCoroutine(InitializationCoroutine());
    }
    public MapSelectItemInfo GetMapInfoByID(int id)
    {
        try
        {
            var data = dataManager.LevelData[id];
            var info = new MapSelectItemInfo();
            info.mapName = data.info.name;
            info.difficult = data.info.difficult;
            info.mapImageSprite = Resources.Load<Sprite>($"Level_Image/{data.info.levelImage}");
            info.unlocked = isTest || PlayerSection.Instance.PlayerData.unlockedLevel.Contains(data.info.id) || PurchaseController.Instance.CheckIfUnlockAllLevelPurchased();
            if (!info.unlocked && (LevelDataInfo.UnlockType)data.info.unlockType == LevelDataInfo.UnlockType.landed)
            {
                info.mapInfo = $"Land {data.info.unlock - PlayerSection.Instance.PlayerData.totalPlaneLanded} planes to unlock!";
            }
            return info;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public void StartGame()
    {
        var levelData = dataManager.GetLevelDataByID(PlayerSection.Instance.LastChooseLevelID);
        string loadOptionScene = levelData.info.loadOption.sceneLoading;
        SceneManager.LoadScene(loadOptionScene);
    }
    public void Quit()
    {
        StartCoroutine(DelayToQuit());
    }
    public int MapCount()
    {
        return dataManager.LevelData.Count;
    }
    private IEnumerator DelayToQuit()
    {
        PlayerSection.Instance.SaveSection();
        yield return null;
        Application.Quit();
    }
    private IEnumerator InitializationCoroutine()
    {
        Time.timeScale = 1;
        dataManager.LoadLevelData();
        yield return new WaitUntil(() => PlayerSection.Instance != null);
        PlayerSection.Instance.PlayerData = dataManager.LoadPlayerData();
        yield return new WaitUntil(() => SoundController.Instance != null);
        SoundController.Instance.UseAudio = PlayerSection.Instance.PlayerData.settingData.useMusic;
        SoundController.Instance.UseSoundFX = PlayerSection.Instance.PlayerData.settingData.useSoundFX;
        SoundController.Instance.PlayMusic(0, true);
        SoundController.Instance.AssignButtonSound();
        mainUiManager.mainTitlePanel.LoadLevelInfoData(PlayerSection.Instance.PlayerData.lastPlayedLevelID);
        mainUiManager.HideFader(() =>
        {
            mainUiManager.mainTitlePanel.Show();
        });
        yield return new WaitForEndOfFrame();
        // yield return new WaitUntil (() => AdsController.Instance != null);
        // AdsController.Instance?.CloseBannerAd ();
        yield return new WaitUntil(() => GameServiceController.Instance != null);
        GameServiceController.Instance.Authenticate();

        yield return null;
    }
}