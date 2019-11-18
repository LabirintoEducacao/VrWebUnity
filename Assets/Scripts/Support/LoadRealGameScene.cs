using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

[RequireComponent(typeof(EnableDisableVR))]
public class LoadRealGameScene : MonoBehaviour
{
	public GameObject[] destroyables;
	bool canQuit = false;

    void Start()
    {
		Invoke("toggleVR", 0.2f);
	}

	void toggleVR() {
		this.GetComponent<EnableDisableVR>().changeState(true, this.vrActive);
	}

	void vrActive(int outcome) {
		SceneManager.LoadScene("LevelGen", LoadSceneMode.Additive);
		InputTracking.Recenter();
		Application.targetFrameRate = 60;
		foreach (GameObject item in destroyables) {
			Destroy(item);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (this.canQuit) {
				CancelInvoke("resetQuitStatus");
				SceneManager.LoadScene("MainMenu_v2");
			} else {
				this.canQuit = true;
				Invoke("resetQuitStatus", 1.5f);
			}
		}
	}

	void resetQuitStatus() => this.canQuit = false;
}
