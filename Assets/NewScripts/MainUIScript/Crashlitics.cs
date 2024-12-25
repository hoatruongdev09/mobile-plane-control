using System.Collections;
using System.Collections.Generic;
// using Firebase;
using UnityEngine;
public class Crashlitics : MonoBehaviour
{
    public static Crashlitics Instance { get; private set; }
    private void Awake()
    {
        var crasher = FindObjectOfType<Crashlitics>();
        if (crasher.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        // Initialize Firebase
        // Firebase.FirebaseApp.CheckAndFixDependenciesAsync ().ContinueWith (task => {
        //     var dependencyStatus = task.Result;
        //     if (dependencyStatus == Firebase.DependencyStatus.Available) {
        //         Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
        //     } else {
        //         UnityEngine.Debug.LogError (System.String.Format (
        //             "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //     }
        // });
    }
}