using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class MainTitlePanel : UiView, IMapSelectViewDelegate, IShopViewDelegate, IStoreListener {
    public IMainTitlePanelDelegate Delegate { get; set; }
    public IMainTitleDatasource Datasource { get; set; }
    public Button buttonStats;
    public Button buttonTutorials;
    public Button buttonSettings;
    public Button buttonMapSelect;
    public Button buttonPlay;
    public Button buttonShop;
    public Button buttonQuit;
    public Image bookImage;
    public StatView statView;
    public SettingsView settingsView;
    public MapSelectView mapSelectView;
    public TutorialView tutorialView;
    public ShopView shopView;
    private UiView CurrentView;
    private UiView currentLayerView;
    private void Start () {
        StartCoroutine (InitializeCoroutine ());
    }
    private IEnumerator InitializeCoroutine () {
        buttonStats.onClick.AddListener (ButtonStats);
        buttonTutorials.onClick.AddListener (ButtonTutorial);
        buttonSettings.onClick.AddListener (ButtonSetting);
        buttonMapSelect.onClick.AddListener (ButtonOpenMapSelect);
        buttonPlay.onClick.AddListener (ButtonPlay);
        buttonQuit.onClick.AddListener (ButtonQuit);
        buttonShop.onClick.AddListener (ButtonShop);
        mapSelectView.Delegate = this;
        shopView.Delegate = this;
        yield return null;
        Enter ();
        yield return new WaitUntil (() => PurchaseController.Instance != null);
        yield return new WaitUntil (() => DataManager.Instance != null);
        Debug.Log ("Purchaser instance ready");
        var purchaser = PurchaseController.Instance;
        var listLevelName = DataManager.Instance.LevelData.Select (LevelData => { return LevelData.info.name; }).ToArray ();
        purchaser.Initialize (this, listLevelName);
    }
    private void ChangeView (UiView nextView) {
        if (nextView == CurrentView) { return; }
        if (CurrentView == null) {
            CurrentView = nextView;
            CurrentView.Show ();
            return;
        }
        CurrentView?.Hide (() => {
            CurrentView = nextView;
            CurrentView.Show ();
        });
    }
    private void OpenView (UiView view) {
        currentLayerView = view;
        view.Show ();
    }
    private void CloseView (UiView view) {
        currentLayerView = null;
        view.Hide ();
    }
    private void Enter () {
        ButtonStats ();
    }
    public void SetViewData (LevelScoreInfo info) {
        statView.textAverageScore.text = $"{info.averageScore}";
        statView.textMostAircraftsOnScreen.text = $"{info.mostAircraftsOnScreen}";
        statView.textMostRecentScore.text = $"{info.mostRecentScore}";
        statView.textTotalAircraftLanded.text = $"{info.totalAircaftLanded}";
    }
    private void ButtonPlay () {
        Delegate?.OnPlayClick ();
    }
    private void ButtonQuit () {

    }
    private void ButtonStats () {
        buttonStats.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () + 1);
        buttonTutorials.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () - 1);
        buttonSettings.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () - 2);
        ChangeView (statView);
    }
    private void ButtonTutorial () {
        buttonStats.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () - 1);
        buttonTutorials.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () + 1);
        buttonSettings.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () - 1);
        ChangeView (tutorialView);
    }
    private void ButtonSetting () {
        buttonStats.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () - 1);
        buttonTutorials.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () - 2);
        buttonSettings.transform.SetSiblingIndex (bookImage.transform.GetSiblingIndex () + 1);
        ChangeView (settingsView);
    }
    private void ButtonShop () {
        OpenView (shopView);
    }
    private void ButtonOpenMapSelect () {
        OpenView (mapSelectView);
    }
    public void OnItemChoose (int id) {
        try {
            Delegate?.OnChooseLevel (id);
            LoadDataForItem (id);
            CloseView (mapSelectView);
        } catch (Exception e) {
            Debug.LogError ($"choose item error: {e}");
        }
    }
    public void OnItemQuickUnlock (int id) {
        if (Datasource == null) { return; }
        var levelInfo = Datasource.GetMapInfoByID (id);
        Debug.Log ($"prepare purchase {levelInfo.mapName}");
        PurchaseController.Instance?.PurchaseLevel (levelInfo.mapName);
        // PlayerSection.Instance.AddUnlockedLevel (levelInfo.mapName);
        // mapSelectView.ShowUnlockResult ("Unlock completed", "City is available to play !");
    }
    private void LoadDataForItem (int id) {
        Debug.Log ($"chose {id}");
        try {
            var levelData = DataManager.Instance.LevelData[id];
            var mapLevelImage = Resources.Load<Sprite> ($"Level_image/{levelData.info.levelImage}");
            var buttonSelect = buttonMapSelect.GetComponent<ButtonMapSelect> ();
            foreach (var image in buttonSelect.mapImages) {
                image.sprite = mapLevelImage;
            }
            buttonSelect.mapName.text = levelData.info.name;
            buttonSelect.SetStars (levelData.info.difficult);
            var levelScoredInfo = DataManager.Instance.LoadLevelScoreInfo (levelData.info.id);
            statView.SetStat (levelScoredInfo);
        } catch (Exception e) {
            throw e;
        }
    }
    public void LoadLevelInfoData (string level) {
        try {
            var levelData = DataManager.Instance.GetLevelDataByID (level);
            var mapLevelImage = Resources.Load<Sprite> ($"Level_image/{levelData.info.levelImage}");
            var buttonSelect = buttonMapSelect.GetComponent<ButtonMapSelect> ();
            foreach (var image in buttonSelect.mapImages) {
                image.sprite = mapLevelImage;
            }
            buttonSelect.mapName.text = levelData.info.name;
            buttonSelect.SetStars (levelData.info.difficult);
            var levelScoredInfo = DataManager.Instance.LoadLevelScoreInfo (levelData.info.id);
            statView.SetStat (levelScoredInfo);
        } catch (Exception e) {
            Debug.LogError (e);
        }
    }

    public void OnPurchaseRemoveAd () {
        Debug.Log ("purchase ad");
        PurchaseController.Instance.PurchaseRemoveAd ();
    }
    public void OnPurchaseUnlockAllLevel () {
        Debug.Log ("purchase all level");
        PurchaseController.Instance.PurchaseUnlockAllLevels ();
    }
    public void OnInitializeFailed (InitializationFailureReason error) {
        Debug.LogError ($"Initialize purchase failed: {error.ToString()}");
    }

    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e) {
        var purchaseProductId = e.purchasedProduct.definition.id;
        if (purchaseProductId == PurchaseController.Instance.RemoveAdID) {
            Debug.Log ("remove ad purchased");
            shopView.DisableButtonRemoveAd ();
            CrossSceneData.Instance.IsRemoveAd = true;
            shopView.ShowPurchaseResult ("Purchase Complete", "Ads are removed !");
        } else if (purchaseProductId == PurchaseController.Instance.UnlockAllLevelsdID) {
            Debug.Log ("unlock all world purchased");
            shopView.DisableButtonUnlockAllLevel ();
            shopView.ShowPurchaseResult ("Purchase Complete", "All level unlocked !");
        } else if (purchaseProductId.Contains (PurchaseController.Instance.UnlockLevelID)) {
            var levelName = purchaseProductId.Replace (PurchaseController.Instance.UnlockLevelID, "").Replace (".", " ").ToLower ();
            levelName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase (levelName.ToLower ());
            mapSelectView.ShowUnlockResult ("Unlock completed", $"{levelName} is available to play !");
            PlayerSection.Instance.AddUnlockedLevel (levelName);
        }
        Debug.Log ($"product id: ${purchaseProductId.Contains("unlock_level_")}");
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed (Product i, PurchaseFailureReason p) {
        if (i.definition.id.Contains (PurchaseController.Instance.UnlockAllLevelsdID)) {
            mapSelectView.ShowUnlockResult ("Unlock failed", $"{ParsePurchaseErrorToString(p)}");
            return;
        }
        shopView.ShowPurchaseResult ("Purchase Failed", $"{ParsePurchaseErrorToString(p)}");
    }
    private string ParsePurchaseErrorToString (PurchaseFailureReason p) {
        return Regex.Replace (p.ToString (), "([a-z])([A-Z])", "$1 $2");
    }
    public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {
        Debug.Log ("Initialize purchase done");
        PurchaseController.Instance.StoreController = controller;
        PurchaseController.Instance.StoreProvider = extensions;

        if (controller.products.WithID (PurchaseController.Instance.RemoveAdID).hasReceipt) {
            shopView.DisableButtonRemoveAd ();
            Debug.Log ("WTFF REMOVE AD ?");
            CrossSceneData.Instance.IsRemoveAd = true;
        } else if (controller.products.WithID (PurchaseController.Instance.RemoveAdID).hasReceipt) {
            Debug.Log ("unlocked all world");
            shopView.DisableButtonUnlockAllLevel ();
        }
    }

}
public interface IMainTitleDatasource {
    MapSelectItemInfo GetMapInfoByID (int id);
}
public interface IMainTitlePanelDelegate {
    void OnChooseLevel (int id);
    void OnChooseLevel (string levelId);
    void OnPlayClick ();
    void OnConfirmQuit ();
}