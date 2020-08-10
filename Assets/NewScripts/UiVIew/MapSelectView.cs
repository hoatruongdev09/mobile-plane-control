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
        ClearHolder ();
        CreateItems ();
        base.Show ();
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
            var item = Instantiate (recycleItem, itemHolder);
            if (info.unlocked) {
                item.transform.SetSiblingIndex (0);
            }
            item.SetInfo (info);
            int ID = i;
            item.ID = ID;
            item.onChoose += OnItemChoose;
            listSelectItem.Add (item);
        }
    }
    private void OnItemChoose (int id) {
        Delegate?.OnItemChoose (id);
    }
}
public interface IMapSelectViewDelegate {
    void OnItemChoose (int id);
}

public interface IMapSelectViewDatasource {
    int MapCount ();
    MapSelectItemInfo GetMapInfoByID (int id);
}