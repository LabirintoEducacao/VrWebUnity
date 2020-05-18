using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnableDisableVR))]
public class BackToMenu : MonoBehaviour {
	void Start() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		Invoke("toggleNonVR", 0.2f);
	}

	void toggleNonVR() {
		//AudioList.instance.PlayButtonClick();
		this.GetComponent<EnableDisableVR>().changeState(false, this.vrInactive);
	}

	void vrInactive(int xt) {
		// ... 
	}
}
