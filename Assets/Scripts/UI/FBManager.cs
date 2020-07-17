using System;
using System.Collections;
using System.Collections.Generic;
// using Facebook.Unity;
using UnityEngine;

public class FBManager : MonoBehaviour {
    public string linkToShare;

    // Use this for initialization
    void Start () {
        // if (!FB.IsInitialized) {
        //     FB.Init (InitCallback, OnHideUnity);

        // } else {
        //     FB.ActivateApp ();
        // }
    }

    // Update is called once per frame
    void Update () {

    }
    private void InitCallback () {
        // if (FB.IsInitialized) {
        //     // Signal an app activation App Event
        //     FB.ActivateApp ();
        //     // Continue with Facebook SDK
        //     // ...
        // } else {
        //     Debug.Log ("Failed to Initialize the Facebook SDK");
        // }
    }
    private void OnHideUnity (bool isGameShown) {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    public void ShareFb () {
        // FB.ShareLink (
        //     new System.Uri (linkToShare),
        //     callback : ShareCallback
        // );

    }
    private void ShareCallback ( /*IShareResult result*/ ) {
        // if (result.Cancelled || !String.IsNullOrEmpty (result.Error)) {
        //     Debug.Log ("ShareLink Error: " + result.Error);
        // } else if (!String.IsNullOrEmpty (result.PostId)) {
        //     // Print post identifier of the shared content
        //     Debug.Log (result.PostId);
        // } else {
        //     // Share succeeded without postID
        //     Debug.Log ("ShareLink success!");
        // }
    }
}