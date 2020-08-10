using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class MainTitlePanel : UiView, IMapSelectViewDelegate, IShopViewDelegate, IStoreListener {
    public IMainTitlePanelDelegate Delegate { get; set; }
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
    public ShopView shopView;
    private UiView CurrentView;
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
        yield return null;
        Enter ();
        yield return new WaitUntil (() => PurchaseController.Instance != null);
        Debug.Log ("Purchaser instance ready");
        var purchaser = PurchaseController.Instance;
        purchaser.Initialize (this);
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
        view.Show ();
    }
    private void CloseView (UiView view) {
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
        PurchaseController.Instance.PurchaseRemoveAd ();
    }

    public void OnInitializeFailed (InitializationFailureReason error) {
        Debug.Log ("Initialize purchase failed");
    }

    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e) {
        if (e.purchasedProduct.definition.id == PurchaseController.Instance.RemoveAdID) {
            Debug.Log ("remove ad");
            shopView.DisableButtonRemoveAd ();
            shopView.ShowPurchaseResult ("Purchase Complete", "Ads are removed");
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed (Product i, PurchaseFailureReason p) {
        shopView.ShowPurchaseResult ("Purchase Failed", $"{p.ToString()}");
    }

    public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {
        Debug.Log ("Initialize purchase done");
    }
}
public interface IMainTitlePanelDelegate {
    void OnChooseLevel (int id);
    void OnChooseLevel (string levelId);
    void OnPlayClick ();
    void OnConfirmQuit ();
}