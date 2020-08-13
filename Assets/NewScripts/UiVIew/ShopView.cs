using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopView : UiView {
    public IShopViewDelegate Delegate { get; set; }
    public Button buttonClose;
    public Button buttonRemoveAd;
    public Button buttonUnlockAllLevel;
    private void Start () {
        buttonClose.onClick.AddListener (ButtonClose);
        buttonRemoveAd.onClick.AddListener (ButtonPurchaseRemoveAd);
        buttonUnlockAllLevel.onClick.AddListener (ButtonUnlockAllLevel);
    }

    private void ButtonUnlockAllLevel () {
        Delegate?.OnPurchaseUnlockAllLevel ();
    }

    private void ButtonClose () {
        Hide ();
    }
    private void ButtonPurchaseRemoveAd () {
        Delegate?.OnPurchaseRemoveAd ();
    }
    public void DisableButtonRemoveAd () {
        buttonRemoveAd.interactable = false;
        var text = buttonRemoveAd.GetComponentInChildren<Text> ();
        text.text += $"<size={text.fontSize*.8f}>(Purchased)</size>";
    }
    public void DisableButtonUnlockAllLevel () {
        buttonUnlockAllLevel.interactable = false;
        var text = buttonUnlockAllLevel.GetComponentInChildren<Text> ();
        text.text += $"<size={text.fontSize*.8f}>(Purchased)</size>";
    }
    public void ShowPurchaseResult (string title, string content) {
        NotificationAnnouncerView announcerPrefab = Resources.Load<NotificationAnnouncerView> ("UI/PanelPurchaseResult");
        var announcer = Instantiate (announcerPrefab, transform);
        announcer.transform.localScale = Vector2.one;
        announcer.textTitle.text = title;
        announcer.textTextContent.text = content;
        announcer.Show ();
    }
}
public interface IShopViewDelegate {
    void OnPurchaseRemoveAd ();
    void OnPurchaseUnlockAllLevel ();
}