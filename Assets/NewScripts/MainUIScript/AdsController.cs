using System;
using System.Collections;
using System.Collections.Generic;
// using GoogleMobileAds;
// using GoogleMobileAds.Api;
using UnityEngine;
public class AdsController : MonoBehaviour
{
    //     public static AdsController Instance { get; set; }
    //     public IRewardAdDelegate RewardAdDelegate { get; set; }
    //     public IBannerAdDelegate BannerAdDelegate { get; set; }

    // #if UNITY_ANDROID
    //     private string rewardedAdUnit = "ca-app-pub-1857579585945039/7469462805";
    //     private string bannerAdUnit = "ca-app-pub-1857579585945039/2127127477";
    // #elif UNITY_IOS
    //     private string rewardedAdUnit = "ca-app-pub-1857579585945039/7187882465";
    //     private string bannerAdUnit = "ca-app-pub-1857579585945039/5274544305";
    // #else 
    //     private string rewardedAdUnit = "";
    //     private string bannerAdUnit = "";
    // #endif

    //     private RewardedAd rewardedAd;
    //     private BannerView bannerAd;
    //     private void Awake () {
    //         var adController = FindObjectOfType<AdsController> ();
    //         if (adController != this) {
    //             Destroy (gameObject);
    //         } else {
    //             DontDestroyOnLoad (gameObject);
    //         }
    //         if (Instance == null) {
    //             Instance = this;
    //         }
    //     }
    //     private void Start () {
    //         MobileAds.Initialize (initStatus => {
    //             Debug.Log ("status ok");
    //         });
    //         InitializeRewardAd ();
    //         InitializeBannerAd ();
    //     }
    //     #region BANNER AD
    //     private void InitializeBannerAd () {
    //         AdSize size = new AdSize (320, 50);
    //         bannerAd = new BannerView (bannerAdUnit, size, AdPosition.Top);
    //         bannerAd.OnAdLoaded += HandleBannerLoaded;
    //         bannerAd.OnAdFailedToLoad += HandleBannerFailedToLoad;
    //         bannerAd.OnAdOpening += HandleBannerOpen;
    //         bannerAd.OnAdClosed += HandleBannerClosed;
    //         bannerAd.OnAdLeavingApplication += HandleBannerLeaving;
    //     }
    //     public void ShowBannerAd () {
    //         this.bannerAd.Show ();
    //     }
    //     public void CloseBannerAd () {
    //         this.bannerAd.Hide ();
    //     }
    //     public void RequestBannerAd () {
    //         AdRequest request = new AdRequest.Builder ().Build ();
    //         this.bannerAd.LoadAd (request);
    //     }
    //     private void HandleBannerLeaving (object sender, EventArgs e) {
    //         BannerAdDelegate?.HandleBannerLeaving (sender, e);
    //     }

    //     private void HandleBannerClosed (object sender, EventArgs e) {
    //         BannerAdDelegate?.HandleBannerClosed (sender, e);
    //     }

    //     private void HandleBannerOpen (object sender, EventArgs e) {
    //         BannerAdDelegate?.HandleBannerOpen (sender, e);
    //     }

    //     private void HandleBannerFailedToLoad (object sender, AdFailedToLoadEventArgs e) {
    //         Debug.Log ($"banner failed to load: {e.Message}");
    //         BannerAdDelegate?.HandleBannerFailedToLoad (sender, e);
    //         RequestBannerAd ();
    //     }

    //     private void HandleBannerLoaded (object sender, EventArgs e) {
    //         BannerAdDelegate?.HandleBannerLoaded (sender, e);
    //     }
    //     #endregion

    //     #region REWARD AD
    //     private void InitializeRewardAd () {
    //         try {
    //             this.rewardedAd.OnAdLoaded -= HandleRewardedAdLoaded;
    //             this.rewardedAd.OnAdFailedToLoad -= HandleRewardedAdFailedToLoad;
    //             this.rewardedAd.OnAdOpening -= HandleRewardedAdOpening;
    //             this.rewardedAd.OnAdFailedToShow -= HandleRewardedAdFailedToShow;
    //             this.rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
    //             this.rewardedAd.OnAdClosed -= HandleRewardedAdClosed;
    //         } catch (Exception e) {

    //         }
    //         rewardedAd = new RewardedAd (rewardedAdUnit);
    //         this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
    //         this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
    //         this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
    //         this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
    //         this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    //         this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
    //     }
    //     public void HandleRewardedAdLoaded (object sender, EventArgs args) {
    //         RewardAdDelegate?.HandleRewardedAdLoaded (sender, args);
    //     }
    //     public void HandleRewardedAdFailedToLoad (object sender, AdErrorEventArgs args) {
    //         RewardAdDelegate?.HandleRewardedAdFailedToLoad (sender, args);
    //         RequestRewardAd ();
    //     }
    //     public void HandleRewardedAdOpening (object sender, EventArgs args) {
    //         RewardAdDelegate?.HandleRewardedAdOpening (sender, args);
    //     }
    //     public void HandleRewardedAdFailedToShow (object sender, AdErrorEventArgs args) {
    //         RewardAdDelegate?.HandleRewardedAdFailedToShow (sender, args);
    //         RequestRewardAd ();
    //     }
    //     public void HandleRewardedAdClosed (object sender, EventArgs args) {
    //         RewardAdDelegate?.HandleRewardedAdClosed (sender, args);
    //         RequestRewardAd ();
    //     }
    //     public void HandleUserEarnedReward (object sender, Reward args) {
    //         RewardAdDelegate?.HandleUserEarnedReward (sender, args);
    //     }
    //     public void RequestRewardAd () {
    //         InitializeRewardAd ();
    //         AdRequest request = new AdRequest.Builder ().Build ();
    //         rewardedAd.LoadAd (request);

    //     }
    //     public bool RewardAdLoaded () {
    //         return rewardedAd != null && rewardedAd.IsLoaded ();
    //     }
    //     public bool ShowRewardAd () {
    //         if (rewardedAd.IsLoaded ()) {
    //             this.rewardedAd.Show ();
    //             return true;
    //         } else {
    //             RequestRewardAd ();
    //             return false;
    //         }
    //     }
    //     #endregion
}
public interface IBannerAdDelegate
{
    // void HandleBannerClosed (object sender, EventArgs e);
    // void HandleBannerFailedToLoad (object sender, AdFailedToLoadEventArgs e);
    // void HandleBannerLeaving (object sender, EventArgs e);
    // void HandleBannerLoaded (object sender, EventArgs e);
    // void HandleBannerOpen (object sender, EventArgs e);
}
public interface IRewardAdDelegate
{
    // void HandleRewardedAdClosed (object sender, EventArgs args);
    // void HandleRewardedAdFailedToLoad (object sender, AdErrorEventArgs args);
    // void HandleRewardedAdFailedToShow (object sender, AdErrorEventArgs args);
    // void HandleRewardedAdLoaded (object sender, EventArgs args);
    // void HandleRewardedAdOpening (object sender, EventArgs args);
    // void HandleUserEarnedReward (object sender, Reward args);
}