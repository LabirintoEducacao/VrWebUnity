using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPreviousMenu : MonoBehaviour {
	public void OnClick() {
		AudioList.instance.PlayButtonClick();
		this.GetComponentInParent<Animator>().SetTrigger("Back");
	}
}
