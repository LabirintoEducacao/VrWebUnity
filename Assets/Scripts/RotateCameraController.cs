using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraController : MonoBehaviour {

	public float speedH = 2.0f;
	public float speedV = 2.0f;

	private float yaw = 0.0f;
	private float pitch = 0.0f;

#if !UNITY_EDITOR && (UNITY_STANDALONE || UNITY_WEBGL)
	private void Start() {
		 Cursor.lockState = CursorLockMode.Locked;
		 Cursor.visible = false;
	}

	private void Update() {

		yaw += speedH * Input.GetAxis("Mouse X");
		pitch -= speedV * Input.GetAxis("Mouse Y");

		pitch = Mathf.Clamp(pitch, -90, 90);

		transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

}
#endif
}
