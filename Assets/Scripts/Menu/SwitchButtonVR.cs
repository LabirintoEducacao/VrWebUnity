using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SwitchButtonVR : MonoBehaviour
{

	public Toggle toggle;

	public void Start() {
		
		toggle.isOn = DataManager.manager.vrMode;
	}
	public void actionClick() {
		AudioList.instance.PlayButtonVR();
		DataManager.manager.switchVrMode(toggle.isOn);
	}
}
