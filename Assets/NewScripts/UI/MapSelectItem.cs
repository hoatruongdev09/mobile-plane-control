﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapSelectItem : MonoBehaviour {
    public OnItemInteract onChoose { get; set; }
    public OnItemInteract onQuickUnlock { get; set; }

    public int ID { get { return id; } set { id = value; Debug.Log ($"id: {id}"); } }
    public Image mapImage;
    public Image mapLock;
    public Text mapName;
    public Text textInfo;
    public GameObject[] starts;
    public Button button;
    public Button buttonButtonUnlockNow;
    private int id;
    public delegate void OnItemInteract (int id);
    private void Start () {
        button.onClick.AddListener (ButtonClick);
        buttonButtonUnlockNow.onClick.AddListener (ButtonUnlockNow);
    }

    private void ButtonUnlockNow () {
        onQuickUnlock?.Invoke (ID);
    }

    private void ButtonClick () {
        onChoose?.Invoke (ID);
    }
    public void SetInfo (MapSelectItemInfo info) {
        mapImage.sprite = info.mapImageSprite;
        mapName.text = info.mapName;
        mapLock.gameObject.SetActive (!info.unlocked);
        button.interactable = info.unlocked;
        textInfo.text = info.mapInfo;
        for (int i = 0; i < starts.Length; i++) {
            starts[i].gameObject.SetActive (i < info.difficult);
        }
    }
}

public class MapSelectItemInfo {
    public Sprite mapImageSprite;
    public string mapName;
    public string mapInfo;
    public int difficult;
    public bool unlocked;
}