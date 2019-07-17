using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour {
#region singleton
    static DataManager instance;
    public static DataManager manager {
        get {
            if (instance == null) {
                GameObject go = new GameObject ();
                go.name = "DataManager";
                return go.AddComponent<DataManager> ();
            } else {
                return instance;
            }
        }
    }

    void Awake () {
        if (instance != null) {
            if (instance != this) {
                Destroy (this.gameObject);
            }
        } else {
            instance = this;
            SceneManager.activeSceneChanged += SceneChanged;
            DontDestroyOnLoad (this.gameObject);
        }
    }
#endregion

    public MazeLDWrapper mazeLD = null;

    public void setNewLevel (string levelDesign) {
        mazeLD = JsonUtility.FromJson<MazeLDWrapper> (levelDesign);
    }

    void SceneChanged (Scene current, Scene next) {
        if (next.name.Equals("MainMenu")) {
            this.mazeLD = null;
        }
    }

    private void OnDestroy () {
        SceneManager.activeSceneChanged -= SceneChanged;
    }
}
