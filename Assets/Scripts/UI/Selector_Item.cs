using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Selector_Item : MonoBehaviour {
	public Text txt_levelName;
	public Text txt_bestScore;
	public Image img_levelImage;
	public Image img_hider;
	public Image img_locker;
	public Transform startDifficultHolder;
	public SpecialScore[] specialScores;
	public int orderIndex;
	public bool isUnlocked;
	// private void Start () {
	// 	Button button = GetComponent<Button> ();
	// 	button.onClick.AddListener (OnClick);
	// }
	public void SetItem (LevelInfo levelInfo, int ordIndex) {
		orderIndex = ordIndex;
		txt_levelName.text = levelInfo.levelName;
		Sprite lvImage = Resources.Load ("Level_Image/" + levelInfo.levelName, typeof (Sprite)) as Sprite;
		Debug.Log ("load image: " + lvImage);
		img_levelImage.sprite = lvImage;
		this.isUnlocked = levelInfo.isUnlocked;
		img_locker.gameObject.SetActive (!levelInfo.isUnlocked);
		if (!this.isUnlocked) {
			txt_bestScore.text = "<size=20>Land " + levelInfo.remainToUnlock.ToString () + " planes to unlock</size>";
		} else {
			txt_bestScore.text = "Best: " + levelInfo.highScore.ToString ();
		}
		if (levelInfo.specialScore != null) {
			for (int i = 0; i < levelInfo.specialScore.Length; i++) {
				specialScores[i].gameObject.SetActive (true);
				specialScores[i].image.sprite = UIItems.Instance.FindScoreInfoByName (levelInfo.specialScore[i].scorename).image;
				specialScores[i].text.text = ": " + levelInfo.specialScore[i].score.ToString ();
			}
		}
		for (int i = 0; i < startDifficultHolder.transform.childCount; i++) {
			if (i < levelInfo.difficult) {
				startDifficultHolder.transform.GetChild (i).gameObject.SetActive (true);
			} else {
				startDifficultHolder.transform.GetChild (i).gameObject.SetActive (false);
			}
		}
	}
	public void SetItem (string lvName, int bestScore, int lv_difficult, bool unlocked, int scoreToUnlock, int ordIndex) {
		orderIndex = ordIndex;
		txt_levelName.text = lvName;
		Sprite lvImage = Resources.Load ("Level_Image/" + lvName, typeof (Sprite)) as Sprite;
		Debug.Log ("load image: " + lvImage);
		img_levelImage.sprite = lvImage;
		this.isUnlocked = unlocked;
		img_locker.gameObject.SetActive (!unlocked);
		if (!this.isUnlocked) {
			txt_bestScore.text = "<size=20>Land " + scoreToUnlock.ToString () + " planes to unlock</size>";
		} else {
			txt_bestScore.text = "Best: " + bestScore.ToString ();
		}
		for (int i = 0; i < startDifficultHolder.transform.childCount; i++) {
			if (i < lv_difficult) {
				startDifficultHolder.transform.GetChild (i).gameObject.SetActive (true);
			} else {
				startDifficultHolder.transform.GetChild (i).gameObject.SetActive (false);
			}
		}
	}
	public void OnClick () {
		bool isCenter = CheckIsCenter ();
		if (isUnlocked && isCenter) {
			Debug.Log ("ON CLICKKKK");
			SceneManager.LoadScene (txt_levelName.text, LoadSceneMode.Single);
		}
		if (!isCenter) {
			FindObjectOfType<ScrollViewSnap> ().SnapAnItemToCenter (this.orderIndex);
		}
	}
	private bool CheckIsCenter () {
		Transform parent = transform.parent;
		Selector_Item[] listItem = parent.GetComponentsInChildren<Selector_Item> ();
		int thisIndex = IndexOf (listItem);
		return thisIndex == listItem.Length - 1;
	}
	private int IndexOf (Selector_Item[] listItem) {
		for (int i = 0; i < listItem.Length; i++) {
			if (listItem[i].orderIndex == this.orderIndex)
				return i;
		}
		return -1;
	}
}