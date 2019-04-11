using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ids
{

    //App id

#if UNITY_ANDROID
    public static string appId = "ca-app-pub-2959493249771497~9209965093";
#elif UNITY_IOS
            public static string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            public static string appId = "unexpected_platform";
#endif
    //Admob ids

    public static string bannerID = "ca-app-pub-3940256099942544/6300978111";
    public static string interstitialID = "ca-app-pub-3940256099942544/1033173712";




    //reward video id
#if UNITY_ANDROID
    public static string rewardedVideoID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
            public static string rewardedVideoID = "ca-app-pub-3940256099942544/1712485313";
#else
            public static string rewardedVideoID = "unexpected_platform";
#endif

    //Show interstitial ad every 3 game played.
    public static int gamePlayed = 3;

    //Give award after rewarded video watched
    public static int videoAdRewardMoney = 250;
    //Start money
    public static int startMoney = 250;

    //1000 Money ID for In-App Purchase
    public static string purchaseMoney1 = "money1000";
    //5000 Money ID for In-App Purchase
    public static string purchaseMoney2 = "money5000";
    //10000 Money ID for In-App Purchase
    public static string purchaseMoney3 = "money10000";
    //25000 Money ID for In-App Purchase
    public static string purchaseMoney4 = "money25000";
}
