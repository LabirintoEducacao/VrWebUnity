using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationButton : MonoBehaviour {
	public string trigger;

	public void actionClick() {
		AudioList.instance.PlayButtonClick();
		this.GetComponentInParent<Animator>().SetTrigger(trigger);
	}
}
