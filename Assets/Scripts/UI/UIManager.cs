using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;

public class UIManager : MonoBehaviour
{
    public UIItems uiItems;

    private void Start()
    {
        uiItems = GetComponent<UIItems>();
        StartCoroutine(WaitForInitializedSelectorItem());
        Time.timeScale = 1;
        if (PlayerPrefs.GetInt("open_level") == 1)
        {
            uiItems.panel_LevelMenu.SetActive(true);
            PlayerPrefs.SetInt("open_level", 0);
        }
        uiItems.img_fader.GetComponent<Animator>().SetTrigger("close");
        uiItems.panel_MainMenu.GetComponent<Animator>().SetTrigger("open");
        uiItems.panel_LevelMenu.SetActive(false);
        //Init Sound Manager
        if (GameObject.FindObjectOfType(typeof(InGameSoundManager)) == null)
        {
            DontDestroyOnLoad(Instantiate(uiItems.soundManager));
        }
        //Init Ads Manager
        if (GameObject.FindObjectOfType(typeof(AdsManager)) == null)
        {
            DontDestroyOnLoad(Instantiate(uiItems.adsManager));
        }


    }
    public void Button_Play()
    {
        StartCoroutine(DelayToOpenLevelMenu());

    }
    public void Button_Back(GameObject from)
    {
        StartCoroutine(DelayToCloseMenu());
        StartCoroutine(DelayToClosePanel(from, uiItems.panel_MainMenu, 1f));
    }
    public void Button_Tutor()
    {
        StartCoroutine(DelayToOpenTutor());
    }
    public void Button_Settings()
    {
        StartCoroutine(DelayToOpenPanel(uiItems.panel_MainMenu, uiItems.panel_Settings, 1f));
    }
    public void Button_Achievement()
    {
        Social.ShowAchievementsUI();
    }
    public void Button_Leaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    private IEnumerator DelayToOpenLevelMenu()
    {
        yield return DelayToOpenPanel(uiItems.panel_MainMenu, uiItems.panel_LevelMenu, 1f);
        StartCoroutine(ChooseDiffcult());
    }
    private IEnumerator DelayToOpenPanel(GameObject from, GameObject to, float duration)
    {
        Animator animatorRef;
        animatorRef = to.GetComponent<Animator>();
        from.GetComponent<Animator>().SetTrigger("close");
        yield return new WaitForSeconds(duration);
        to.SetActive(true);
        animatorRef.ResetTrigger("close");
        animatorRef.SetTrigger("open");
    }
    private IEnumerator DelayToOpenTutor()
    {
        yield return DelayToOpenPanel(uiItems.panel_MainMenu, uiItems.panel_tutor, 1f);
    }
    private IEnumerator DelayToCloseMenu()
    {
        yield return DelayToClosePanel(uiItems.panel_LevelMenu, uiItems.panel_MainMenu, 1f);
    }
    private IEnumerator DelayToClosePanel(GameObject from, GameObject to, float duration)
    {
        Animator fromAnimator = from.GetComponent<Animator>();
        fromAnimator.ResetTrigger("open");
        fromAnimator.SetTrigger("close");

        yield return new WaitForSeconds(duration);
        from.SetActive(false);
        to.GetComponent<Animator>().SetTrigger("open");
    }
    private IEnumerator DelayOpenPlayAnimation(GameObject GO, float duration)
    {
        yield return new WaitForSeconds(duration);
        GO.GetComponent<Animator>().SetTrigger("open");
    }
    private IEnumerator WaitForInitializedSelectorItem()
    {
        yield return new WaitUntil(() => LevelLoading.loadedData);
        InitializeSelectorItem();
    }
    private void InitializeSelectorItem()
    {
        List<LevelInfo> lstLevelInfo = GetComponent<LevelLoading>().GetLevelInfos();
        for (int i = 0; i < lstLevelInfo.Count; i++)
        {
            CreateSelectorItem(lstLevelInfo[i], i);
        }
    }
    private void CreateSelectorItem(LevelInfo lvInfo, int orderIndex)
    {
        GameObject go = Instantiate(uiItems.prefab_dotItem, uiItems.holder_dotItem);
        Selector_Item si = Instantiate(uiItems.prefab_selectorItem, uiItems.holder_selectorItem);
        si.SetItem(lvInfo, orderIndex);
    }
    private IEnumerator ChooseDiffcult()
    {
        yield return new WaitForEndOfFrame();
        DifficultButton[] dButtons = FindObjectsOfType<DifficultButton>();
        string savedDifficult = PlayerPrefs.GetString("difficult", "easy");
        foreach (DifficultButton dButton in dButtons)
        {
            if (dButton.content.Equals(savedDifficult))
            {
                dButton.Active();
                break;
            }
        }
    }
    int currentTutor;

    public void btn_nextTutor()
    {

        if (currentTutor == uiItems.tutorImage.Count - 1)
        {
            uiItems.panel_tutor.GetComponent<Animator>().ResetTrigger("open");
            uiItems.panel_tutor.GetComponent<Animator>().SetTrigger("close");
            StartCoroutine(DelayOpenPlayAnimation(uiItems.panel_MainMenu, 1f));
            uiItems.tutorImage[currentTutor].SetActive(false);
            currentTutor = 0;
            uiItems.tutorImage[currentTutor].SetActive(true);
        }
        else
        {
            uiItems.tutorImage[currentTutor].SetActive(false);
            currentTutor++;
            uiItems.tutorImage[currentTutor].SetActive(true);
            uiItems.txt_Holder.text = uiItems.tutorText[currentTutor].Replace("\\n", "\n");
        }
    }

}