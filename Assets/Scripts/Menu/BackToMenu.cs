using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnableDisableVR))]
public class BackToMenu : MonoBehaviour {
	void Start() {
		Invoke("toggleNonVR", 0.2f);
	}

	void toggleNonVR() {
		this.GetComponent<EnableDisableVR>().changeState(false, this.vrInactive);
	}

	void vrInactive(int xt) {
		// ... 
	}
}
