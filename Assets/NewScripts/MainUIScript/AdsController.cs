using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;
public class AdsController : MonoBehaviour {
    public static AdsController Instance { get; set; }
    public IRewardAdDelegate RewardAdDelegate { get; set; }
    public IBannerAdDelegate BannerAdDelegate { get; set; }

#if UNITY_ANDROID
    public string rewardedAdUnit = "ca-app-pub-3940256099942544/5224354917";
    public string bannerAdUnit = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
    public string rewardedAdUnit = "ca-app-pub-3940256099942544/1712485313";
    public string bannerAdUnit = "ca-app-pub-3940256099942544/2934735716";
#else 
    public string rewardedAdUnit = "";
    public string bannerAdUnit = "";
#endif

    private RewardedAd rewardedAd;
    private BannerView bannerAd;
    private void Awake () {
        var adController = FindObjectOfType<AdsController> ();
        if (adController != this) {
            Destroy (gameObject);
        } else {
            DontDestroyOnLoad (gameObject);
        }
        if (Instance == null) {
            Instance = this;
        }
    }
    private void Start () {
        MobileAds.Initialize (initStatus => {
            Debug.Log ("status ok");
        });
        InitializeRewardAd ();
        InitializeBannerAd ();
    }
    private void InitializeBannerAd () {
        AdSize size = new AdSize (320, 50);
        bannerAd = new BannerView (bannerAdUnit, size, AdPosition.Top);
        bannerAd.OnAdLoaded += HandleBannerLoaded;
        bannerAd.OnAdFailedToLoad += HandleBannerFailedToLoad;
        bannerAd.OnAdOpening += HandleBannerOpen;
        bannerAd.OnAdClosed += HandleBannerClosed;
        bannerAd.OnAdLeavingApplication += HandleBannerLeaving;
    }
    public void RequestBannerAd () {
        AdRequest request = new AdRequest.Builder ().Build ();
        this.bannerAd.LoadAd (request);
    }
    private void HandleBannerLeaving (object sender, EventArgs e) {
        BannerAdDelegate?.HandleBannerLeaving (sender, e);
    }

    private void HandleBannerClosed (object sender, EventArgs e) {
        BannerAdDelegate?.HandleBannerClosed (sender, e);
    }

    private void HandleBannerOpen (object sender, EventArgs e) {
        BannerAdDelegate?.HandleBannerOpen (sender, e);
    }

    private void HandleBannerFailedToLoad (object sender, AdFailedToLoadEventArgs e) {
        BannerAdDelegate?.HandleBannerFailedToLoad (sender, e);
    }

    private void HandleBannerLoaded (object sender, EventArgs e) {
        BannerAdDelegate?.HandleBannerLoaded (sender, e);
    }

    private void InitializeRewardAd () {
        rewardedAd = new RewardedAd (rewardedAdUnit);
        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
    }
    public void HandleRewardedAdLoaded (object sender, EventArgs args) {
        RewardAdDelegate?.HandleRewardedAdLoaded (sender, args);
    }
    public void HandleRewardedAdFailedToLoad (object sender, AdErrorEventArgs args) {
        RewardAdDelegate?.HandleRewardedAdFailedToLoad (sender, args);
    }
    public void HandleRewardedAdOpening (object sender, EventArgs args) {
        RewardAdDelegate?.HandleRewardedAdOpening (sender, args);
    }
    public void HandleRewardedAdFailedToShow (object sender, AdErrorEventArgs args) {
        RewardAdDelegate?.HandleRewardedAdFailedToShow (sender, args);
    }
    public void HandleRewardedAdClosed (object sender, EventArgs args) {
        RewardAdDelegate?.HandleRewardedAdClosed (sender, args);
    }
    public void HandleUserEarnedReward (object sender, Reward args) {
        RewardAdDelegate?.HandleUserEarnedReward (sender, args);
    }
    public void RequestRewardAd () {
        AdRequest request = new AdRequest.Builder ().Build ();
        rewardedAd.LoadAd (request);
    }
    public bool ShowRewardAd () {
        if (rewardedAd.IsLoaded ()) {
            this.rewardedAd.Show ();
            return true;
        } else {
            return false;
        }
    }
}
public interface IBannerAdDelegate {
    void HandleBannerClosed (object sender, EventArgs e);
    void HandleBannerFailedToLoad (object sender, AdFailedToLoadEventArgs e);
    void HandleBannerLeaving (object sender, EventArgs e);
    void HandleBannerLoaded (object sender, EventArgs e);
    void HandleBannerOpen (object sender, EventArgs e);
}
public interface IRewardAdDelegate {
    void HandleRewardedAdClosed (object sender, EventArgs args);
    void HandleRewardedAdFailedToLoad (object sender, AdErrorEventArgs args);
    void HandleRewardedAdFailedToShow (object sender, AdErrorEventArgs args);
    void HandleRewardedAdLoaded (object sender, EventArgs args);
    void HandleRewardedAdOpening (object sender, EventArgs args);
    void HandleUserEarnedReward (object sender, Reward args);
}