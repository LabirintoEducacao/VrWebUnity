using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GyroController : MonoBehaviour {

	// Optional, allows user to drag left/right to rotate the world.
	private const float DRAG_RATE = .2f;
	float dragYawDegrees;

	void Start() {
		// Make sure orientation sensor is enabled.
		Input.gyro.enabled = true;
	}

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
}
