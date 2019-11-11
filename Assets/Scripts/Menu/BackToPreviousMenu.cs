using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPreviousMenu : MonoBehaviour {
	public void OnClick() {
		this.GetComponentInParent<Animator>().SetTrigger("Back");
	}
}
