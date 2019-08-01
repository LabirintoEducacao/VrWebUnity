using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseInitializer : MonoBehaviour {
    public static bool firebaseLoaded = false;
    public static Firebase.FirebaseApp app;

    private void Awake () {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync ().ContinueWith (task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                firebaseLoaded = true;

                // FirebaseAnalytics.SetCurrentScreen (SceneManager.GetActiveScene ().name);
                // googleAnalytics.LogScreen ("Main Menu");
            } else {
                UnityEngine.Debug.LogError (System.String.Format (
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
}
