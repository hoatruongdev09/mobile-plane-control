using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapSelectItem : MonoBehaviour {
    public OnChoose onChoose { get; set; }
    public int ID { get { return id; } set { id = value; Debug.Log ($"id: {id}"); } }
    public Image mapImage;
    public Image mapLock;
    public Text mapName;
    public Text textInfo;
    public GameObject[] starts;
    public Button button;
    private int id;
    public delegate void OnChoose (int id);
    private void Start () {
        button.onClick.AddListener (ButtonClick);
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