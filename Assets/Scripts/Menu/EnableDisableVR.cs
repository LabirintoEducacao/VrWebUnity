using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EnableDisableVR : MonoBehaviour {
    public bool vrMode;

    void Start () {
        //changeState (vrMode, null);
    }

	public void updateState(bool active) {
		changeState(active, null);
	}

    public void changeState (bool active, Action<int> callback) {
        Debug.Log ("Trying to activate VR mode? " + active);

        if (active) {
            StartCoroutine ( changeDevice("cardboard",callback));
        } else {
						StartCoroutine ( changeDevice("", callback));
        }
    }

	IEnumerator changeDevice(string device, Action<int> callback) {
		if (String.Compare(XRSettings.loadedDeviceName, device, true) != 0) {
			try {
				XRSettings.LoadDeviceByName(device);
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
			yield return null;
			yield return null;
			try {
				XRSettings.enabled = true;
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
		}
		callback?.Invoke(0);
	}

    IEnumerator SwitchOffVR (Action<int> callback) {
        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "None"; // Or "cardboard".
        Debug.Log ("Trying VR to set to no device.");
		yield return null;

		if (desiredDevice.Equals(XRSettings.loadedDeviceName)) {
            Debug.Log("Trying to switch VR off on a non-VR status");
        } else {
            // Some VR Devices do not support reloading when already active, see
            // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
            XRSettings.LoadDeviceByName (desiredDevice);

            // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
            yield return new WaitForSeconds (0.2f);

			// Now it's ok to enable VR mode.
			XRSettings.enabled = true;
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		callback?.Invoke(0);
    }

    IEnumerator SwitchToVR (Action<int> callback) {
        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "cardboard"; // Or "cardboard".
        Debug.Log ("Trying VR to set to device: " + desiredDevice);
        foreach (string device in XRSettings.supportedDevices) {
            Debug.Log ("device: " + device);
        }

		if (desiredDevice.Equals(XRSettings.loadedDeviceName)) {
			Debug.Log("Trying to switch VR on but VR is already on.");
		} else {
			// Some VR Devices do not support reloading when already active, see
			// https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
			XRSettings.LoadDeviceByName(desiredDevice);

			// Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
			//yield return new WaitForSeconds (0.2f);
			yield return null;

			// Now it's ok to enable VR mode.
			XRSettings.enabled = true;
		}

		yield return null;
		callback?.Invoke(0);
	}
}
