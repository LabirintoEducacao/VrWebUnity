using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

[RequireComponent(typeof(EnableDisableVR))]
public class LoadRealGameScene : MonoBehaviour
{
	public GameObject[] destroyables;

    void Start()
    {
		Invoke("toggleVR", 0.2f);
	}

	void toggleVR() {
		this.GetComponent<EnableDisableVR>().changeState(true, this.vrActive);
	}

	void vrActive(int outcome) {
		SceneManager.LoadScene("Gabriel_Level_Test", LoadSceneMode.Additive);
		InputTracking.Recenter();
		foreach (GameObject item in destroyables) {
			Destroy(item);
		}
	}

	//void Update() {
	//	if (Input.GetKeyDown(KeyCode.Escape)) {
	//		SceneManager.LoadScene("MainMenu");
	//	} else if (Input.touchCount > 0) {
	//		if (Input.GetTouch(0).phase == TouchPhase.Began) {
	//			this.toggleVR();
	//		}
	//	}
	//}
}
