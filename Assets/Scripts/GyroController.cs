using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class GyroController : MonoBehaviour
{
	// Problem message to gyroscope
	TextMeshProUGUI textProblem;
	const string strProblem = "Problema ao utilizar Giroscópio: Verifique se o celular possui o sensor e, em caso positivo, reinicie o jogo para tentar corrigir o problema.";

	// Optional, allows user to drag left/right to rotate the world.
	private const float DRAG_RATE = .2f;
	float dragYawDegrees;

	private Gyroscope gyro;
	private bool gyroEnabled;

	void Start() {
		textProblem = GameManager.Instance.startingRoom.GetComponent<RoomManager>().TextQuestion;
		gyroEnabled = EnableGyro();
	}

	private bool EnableGyro() {
		if (SystemInfo.supportsGyroscope) {
			gyro = Input.gyro;
			gyro.enabled = true;
			return true;
		}

		return false;
	}

	//private void OnGUI() {
	//		GUIStyle style = new GUIStyle();
	//		style.normal.textColor = Color.green;
	//		style.fontSize = 24;
	//		GUILayout.Label("Input.gyro.enabled: " + Input.gyro.enabled.ToString(), style);
	//		GUILayout.Label("XRSettings.enabled: " + XRSettings.enabled.ToString(), style);
	//		GUILayout.Label("Input.gyro.attitude: " + Input.gyro.attitude.ToString(), style);
	//		GUILayout.Label("transform.localRotation: " + transform.localRotation.ToString(), style);
	//		GUILayout.Label("dragYawDegrees: " + dragYawDegrees.ToString(), style);
	//		GUILayout.Label("isOkay: " + gyroEnabled.ToString(), style);
	//		GUILayout.Label("Input.gyro: " + Input.gyro.ToString(), style);
	//		GUILayout.Label("SystemInfo.supportsGyroscope: " + SystemInfo.supportsGyroscope.ToString(), style);
	//	}

	void Update() {
		if (XRSettings.enabled) {
			// Unity takes care of updating camera transform in VR.
			return;
		} else {
			// "Magic Window" 
			// renders a single (often full screen) monoscopic view of your 3D scene that is updated
			// based on the device's orientation sensor.
#if !UNITY_EDITOR && UNITY_ANDROID
			rotationCamera();
#endif
		}
	}

	void rotationCamera() {

		// android-developers.blogspot.com/2010/09/one-screen-turn-deserves-another.html
		// developer.android.com/guide/topics/sensors/sensors_overview.html#sensors-coords
		//
		//     y                                       x
		//     |  Gyro upright phone                   |  Gyro landscape left phone
		//     |                                       |
		//     |______ x                      y  ______|
		//     /                                       \
		//    /                                         \
		//   z                                           z
		//
		//
		//     y
		//     |  z   Unity
		//     | /
		//     |/_____ x
		//

		// Update `dragYawDegrees` based on user touch.
		CheckDrag();

		if (gyroEnabled) {

			// If the attitude not is working
			if (!CheckAttitude(Input.gyro.attitude)) {
				textProblem.text = strProblem;
			} else {
				transform.localRotation =
				// Allow user to drag left/right to adjust direction they're facing.
				Quaternion.Euler(0f, -dragYawDegrees, 0f) *

				// Neutral position is phone held upright, not flat on a table.
				Quaternion.Euler(90f, 0f, 0f) *

				// Sensor reading, assuming default `Input.compensateSensors == true`.
				Input.gyro.attitude *

				// So image is not upside down.
				Quaternion.Euler(0f, 0f, 180f);
			}
		} else {
			textProblem.text = strProblem;
		}
	}

	void CheckDrag() {
		if (Input.touchCount != 1) {
			return;
		}

		Touch touch = Input.GetTouch(0);
		if (touch.phase != TouchPhase.Moved) {
			return;
		}

		dragYawDegrees += touch.deltaPosition.x * DRAG_RATE;
	}

	bool CheckAttitude(Quaternion attitude) {
		return !(float.IsNaN(attitude.w) ||
						float.IsNaN(attitude.x) ||
						float.IsNaN(attitude.y) ||
						float.IsNaN(attitude.z)
					);
	}
}
