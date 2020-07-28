using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItems : MonoBehaviour {
    public static UIItems Instance { get; private set; }
    public Image img_fader;

    [Header ("Panel")]
    public GameObject panel_MainMenu;
    public GameObject panel_LevelMenu;
    public GameObject panel_tutor;
    public GameObject panel_Settings;
    public GameObject panel_Quit;
    [Header ("Scroll")]
    public GameObject level_ScrollView;
    [Header ("Holder")]
    public Transform holder_selectorItem;
    public Transform holder_dotItem;
    [Header ("Items")]
    public Selector_Item prefab_selectorItem;
    public GameObject prefab_dotItem;
    public SpecialScoreInfo[] specialScoreInfos;
    [Header ("Text")]
    public Text flipText;
    [Header ("Tutor")]
    public Text txt_Holder;
    public List<string> tutorText;
    public List<GameObject> tutorImage;
    [Header ("Prefabs")]
    public GameObject soundManager;
    public GameObject adsManager;

    private void Start () {
        Instance = this;
    }
    public SpecialScoreInfo FindScoreInfoByName (string name) {
        foreach (SpecialScoreInfo ssi in specialScoreInfos) {
            if (ssi.scoreName == name)
                return ssi;
        }
        return null;
    }

}