using System;
using System.Collections;
using System.Collections.Generic;
// using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoBehaviour {
    public static AdsManager Instance { get; private set; }
    // private BannerView bannerView;
    // private InterstitialAd interstitial;
    // private RewardBasedVideoAd rewardBasedVideo;

    // Use this for initialization
    void Start () {
        // Instance = this;
        // MobileAds.Initialize (Ids.appId);
        // LoadAds ();
        // rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
    }

    //Load ads
    public void LoadAds () {

        // //Init and load banner
        // //
        // bannerView = new BannerView (Ids.bannerID, AdSize.Banner, AdPosition.Top);
        // // Create an empty ad request.
        // AdRequest requestBanner = new AdRequest.Builder ().Build ();

        // // Load the banner with the request.
        // bannerView.LoadAd (requestBanner);

        // //Init and load Interstitial
        // //
        // interstitial = new InterstitialAd (Ids.interstitialID);
        // // Create an empty ad request.
        // AdRequest requestInter = new AdRequest.Builder ().Build ();
        // // Load the interstitial with the request.
        // interstitial.LoadAd (requestInter);

        // RequestRewardBasedVideo ();

    }

    void RequestRewardBasedVideo () {
        // //Init and load Reward Video
        // //
        // rewardBasedVideo = RewardBasedVideoAd.Instance;
        // // Create an empty ad request.
        // AdRequest request = new AdRequest.Builder ().Build ();
        // // Load the rewarded video ad with the request.
        // this.rewardBasedVideo.LoadAd (request, Ids.rewardedVideoID);
    }

    public void ShowInterstitialAds () {
        // if (interstitial.IsLoaded ()) {
        //     interstitial.Show ();
        // }
    }

    public void ShowRewardVideo () {
        // if (rewardBasedVideo.IsLoaded ()) {
        //     rewardBasedVideo.Show ();
        // }
    }
    //event reward video
    // public void HandleRewardBasedVideoClosed (object sender, EventArgs args) {
    //     RequestRewardBasedVideo ();
    //     MonoBehaviour.print ("HandleRewardBasedVideoClosed event received");
    // }
    // public void HandleRewardBasedVideoRewarded (object sender, Reward args) {
    //     string type = args.Type;
    //     double amount = args.Amount;
    //     MonoBehaviour.print (
    //         "HandleRewardBasedVideoRewarded event received for " +
    //         amount.ToString () + " " + type);
    //     GameControl.Instance.OnContinue ();
    // }
    // /// <summary>
    // /// Open rate app
    // /// </summary>
    // /// <param name="packageName"> package name(android) or id(IOS)</param>

    // /// <summary>
    // /// This function is called when the behaviour becomes disabled or inactive.
    // /// </summary>
    // void OnDisable () {
    //     rewardBasedVideo.OnAdClosed -= HandleRewardBasedVideoClosed;
    // }

}