using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopView : UiView {
    public IShopViewDelegate Delegate { get; set; }
    public Button buttonClose;
    public Button buttonRemoveAd;
    private void Start () {
        buttonClose.onClick.AddListener (ButtonClose);
        buttonRemoveAd.onClick.AddListener (ButtonPurchaseRemoveAd);
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
        text.text = "Purchased";
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
}