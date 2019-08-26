using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseInitializer : MonoBehaviour {
    static FirebaseInitializer _instance;
    public static FirebaseInitializer instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject ();
                _instance = go.AddComponent<FirebaseInitializer> ();
            }
            return _instance;
        }
    }
    public static bool firebaseLoaded { get { return app != null; } }
    public static Firebase.FirebaseApp app = null;
    bool initializing = false;

    private void Awake () {
        if ((_instance != null) && (FirebaseInitializer._instance != this)) {
            Destroy (this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad (this.gameObject);
			Invoke("initialize", 0.1f);
        }
    }

    private void initialize () {
        if (!initializing && !firebaseLoaded) {
            Debug.Log ("Firebase Initialization starting.");
            this.initializing = true;
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync ().ContinueWith (task => {
                var dependencyStatus = task.Result;
                Debug.Log ("Firebase Initialization completed with status: " + dependencyStatus);
                this.initializing = false;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    app = Firebase.FirebaseApp.DefaultInstance;

                    FirebaseAnalytics
                        .LogEvent (FirebaseAnalytics.EventAppOpen);
                    SceneManager.activeSceneChanged += OnSceneChanged;

                } else {
                    UnityEngine.Debug.LogError (System.String.Format (
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
    }

	void OnDestroy() {
		SceneManager.activeSceneChanged -= OnSceneChanged;
	}

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
		if (firebaseLoaded) {
			FirebaseAnalytics
				.LogEvent("navigation", FirebaseAnalytics.ParameterLevelName, newScene.name);
		}
    }
}
