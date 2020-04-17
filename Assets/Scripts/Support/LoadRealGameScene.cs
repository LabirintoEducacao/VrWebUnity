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
		bool isEnabledVR = DataManager.manager.vrMode;
		this.GetComponent<EnableDisableVR>().changeState(isEnabledVR, this.vrActive);
	}
	
	void vrActive(int outcome) {

		//Receber o inteiro da fase escolhida atraves do json
		int level_id = DataManager.manager.mazeLD.theme;

		switch (level_id)
		{
			case 0:
				SceneManager.LoadScene("LevelGen", LoadSceneMode.Additive);
				break;
			case 1:
				SceneManager.LoadScene("CaveLevel", LoadSceneMode.Additive);
				break;
			case 2:
				SceneManager.LoadScene("DesertLevel", LoadSceneMode.Additive);
				break;
			case 3:
				SceneManager.LoadScene("ForestLevel", LoadSceneMode.Additive);
				break;
			case 4:
				SceneManager.LoadScene("HouseLevel", LoadSceneMode.Additive);
				break;
			case 5:
				SceneManager.LoadScene("UrbanLevel", LoadSceneMode.Additive);
				break;
			default:
				Debug.Log("Codigo do tema não existe");
				break;
		}

		//SceneManager.LoadScene("LevelGen", LoadSceneMode.Additive);
		InputTracking.Recenter();
#if UNITY_ANDROID
		Application.targetFrameRate = 60;
#endif
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
