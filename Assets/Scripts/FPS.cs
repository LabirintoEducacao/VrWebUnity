using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {
	public Text FPSText;
	public float deltaTime;

	// Start is called before the first frame update
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float FPS = 1.0f / deltaTime;

		FPSText.text = Mathf.Ceil(FPS).ToString() + " fps";
	}
}
