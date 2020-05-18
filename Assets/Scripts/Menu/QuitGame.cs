using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
	public void quitClick() {
		AudioList.instance.PlayButtonClick();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();	
#endif
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			quitClick();
		}
	}
}
