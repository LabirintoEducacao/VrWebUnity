using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePrintscreen : MonoBehaviour {
	private int screenshotCount = 0;

	// Use this for initialization
	void Start () {
		screenshotCount = PlayerPrefs.GetInt("ScreenshotCount", 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Y)) {
			generate();
		}
	}

	public void generate() {
			ScreenCapture.CaptureScreenshot("Screenshot_" + screenshotCount+".png");
			screenshotCount++;
			PlayerPrefs.SetInt("ScreenshotCount", screenshotCount);
			PlayerPrefs.Save();
	}
}
