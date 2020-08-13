using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapSelectView : UiView {
    public IMapSelectViewDelegate Delegate { get; set; }
    public IMapSelectViewDatasource Datasource { get; set; }
    public Button buttonClose;
    public MapSelectItem recycleItem;
    public RectTransform itemHolder;

    [SerializeField] public List<MapSelectItem> listSelectItem = new List<MapSelectItem> ();
    private void Start () {
        buttonClose.onClick.AddListener (ButtonClose);
    }

    private void ButtonClose () {
        Hide ();
    }

    public override void Show () {
        Refresh ();
        base.Show ();
    }
    public void Refresh () {
        ClearHolder ();
        CreateItems ();
    }
    public void ShowUnlockResult (string title, string content) {
        NotificationAnnouncerView announcerPrefab = Resources.Load<NotificationAnnouncerView> ("UI/PanelPurchaseResult");
        var announcer = Instantiate (announcerPrefab, transform);
        announcer.transform.localScale = Vector2.one;
        announcer.textTitle.text = title;
        announcer.textTextContent.text = content;
        announcer.ConfirmEvents.AddListener (Refresh);
        announcer.Show ();
    }
    private void ClearHolder () {
        foreach (var selectItem in listSelectItem) {
            Destroy (selectItem.gameObject);
        }
        listSelectItem.Clear ();
    }
    private void CreateItems () {
        int itemCount = 0;
        if (Datasource != null) { itemCount = Datasource.MapCount (); } else { return; }
        for (int i = 0; i < itemCount; i++) {
            var info = Datasource.GetMapInfoByID (i);
            var item = CreateItem (i, info);
            listSelectItem.Add (item);
        }
    }
    private MapSelectItem CreateItem (int id, MapSelectItemInfo info) {
        var item = Instantiate (recycleItem, itemHolder);
        if (info.unlocked) {
            item.transform.SetSiblingIndex (0);
            item.buttonButtonUnlockNow.gameObject.SetActive (false);
        }

        item.SetInfo (info);
        int ID = id;
        item.ID = ID;
        item.onChoose += OnItemChoose;
        item.onQuickUnlock += OnItemQuickUnlock;
        return item;
    }
    private void OnItemChoose (int id) {
        Delegate?.OnItemChoose (id);
    }
    private void OnItemQuickUnlock (int id) {
        Delegate?.OnItemQuickUnlock (id);
    }
}
public interface IMapSelectViewDelegate {
    void OnItemChoose (int id);
    void OnItemQuickUnlock (int id);
}

public interface IMapSelectViewDatasource {
    int MapCount ();
    MapSelectItemInfo GetMapInfoByID (int id);
}