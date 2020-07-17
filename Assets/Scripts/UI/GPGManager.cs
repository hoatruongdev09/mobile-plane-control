using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
#endif
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class GPGManager : MonoBehaviour {

    //	public Text login;
    string mStatus;

    List<string> nameList = new List<string> ();
    //public List<UILabel> uiLabel = new List<UILabel>();
    string leaderBoardID = "CgkIrfSRlN0XEAIQAA";

    IScore[] lbScore;

    // Use this for initialization
    void Start () {
        LoadGoogleLeaderBoard ();
        LoadGameCenter ();
    }

    // Update is called once per frame
    void Update () {

    }

    private void LoginGooglePlayGames (bool loadLeaderboard) {
        Social.localUser.Authenticate ((bool success) => {
            if (success) {
                Debug.Log ("Login Sucess");
                Debug.Log (Social.localUser.userName + "co cl");
                if (loadLeaderboard) {
                    DoLoadLeaderboard (leaderBoardID);
                }
            } else {
                Debug.Log ("Login failed");

            }
        });
    }

    public void LoadGoogleLeaderBoard () {
#if UNITY_ANDROID
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        //PlayGamesPlatform.InitializeInstance(config);
        // PlayGamesPlatform.Activate ();
        // LoginGooglePlayGames (false);

#endif

    }

    public void OnLB (string id) {
        DoLoadLeaderboard (leaderBoardID);
    }
    private void DoLoadLeaderboard (string lbID) {
        Debug.Log ("DoLoadLeaderboard");
#if UNITY_ANDROID
        // PlayGamesPlatform.Instance.LoadScores (
        //     lbID,
        //     LeaderboardStart.TopScores,
        //     4,
        //     LeaderboardCollection.Public,
        //     LeaderboardTimeSpan.AllTime,
        //     (data) => {
        //         mStatus = "Leaderboard data valid: " + data.Valid;
        //         mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;

        //         lbScore = data.Scores;
        //         Debug.Log (data.Scores.Length + " lb fucking shit, ID: " + lbID);
        //         Debug.Log ("Status: " + mStatus);

        //         //Load score to UI
        //         //LoadUsersAndDisplay(lbScore);

        //         Debug.Log (lbScore[0].userID + " User ID 1st");
        //         //Load Leaderboard buil-in UI
        //         Social.ShowLeaderboardUI ();
        //     });
#endif
    }

    private void LoadUsersAndDisplay (IScore[] scores) {
        if (scores.Length == 0) return;
        // get the use ids
        List<string> userIds = new List<string> ();
        nameList.Clear ();
        foreach (IScore score in scores) {
            userIds.Add (score.userID);
        }
        Social.LoadUsers (userIds.ToArray (), (users) => {
            //login.text = "Leaderboard loading: " + lb.title + " count == " +users.Length;

            foreach (IScore score in scores) {
                IUserProfile user = FindUser (users, score.userID);
                Debug.Log (score.userID + " User ID");
                nameList.Add ((string) (
                    (user != null) ? user.userName : "**unk_" + score.userID + "**"));
                mStatus += " " + score.formattedValue + " by " +
                    (string) (
                        (user != null) ? user.userName : "**unk_" + score.userID + "**");
            }
        });
    }

    private IUserProfile FindUser (IUserProfile[] users, string userid) {
        foreach (IUserProfile user in users) {
            if (user.id == userid) {
                return user;
            }
        }
        return null;
    }

    public void OnAddScoreToLeaderBorad () {
#if UNITY_ANDROID
        // if (Social.localUser.authenticated) {
        //     Social.ReportScore (5200, leaderBoardID, (bool success) => {
        //         if (success) {
        //             ((PlayGamesPlatform) Social.Active).ShowLeaderboardUI (leaderBoardID);
        //         } else {
        //             Debug.Log ("Add Score Fail");
        //         }
        //     });
        // }
#endif
    }

    private void LoadGameCenter () {
#if !UNITY_ANDROID
        Social.localUser.Authenticate (ProcessAuthentication);
#endif
    }

    private void ProcessAuthentication (bool success) {
        if (success) {
            Debug.Log ("Loggin Game Center success");
            Social.ShowLeaderboardUI ();
        } else {
            Debug.Log ("Game Center logging failed");
        }
    }

    public void LoadLeaderboard (string leaderboardID) {
        if (Social.localUser.authenticated)
            DoLoadLeaderboard (leaderboardID);
        else
            LoginGooglePlayGames (true);
    }
}