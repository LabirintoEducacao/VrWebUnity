using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EnableDisableVR : MonoBehaviour {
    public bool vrMode;

    void Awake () {
        changeState (vrMode, null);
    }

    public void changeState (bool active, Action<int> callback) {
        Debug.Log ("Trying to activate VR mode? " + active);
        if (active) {
            StartCoroutine (SwitchToVR (callback));
        } else {
            StartCoroutine (SwitchOffVR (callback));
        }
    }

    IEnumerator SwitchOffVR (Action<int> callback) {
        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = ""; // Or "cardboard".
        Debug.Log ("Trying VR to set to no device.");

        // Some VR Devices do not support reloading when already active, see
        // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
        XRSettings.LoadDeviceByName (desiredDevice);

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return new WaitForSeconds (0.2f);

        // Now it's ok to enable VR mode.
        XRSettings.enabled = false;

        if (callback != null) {
            callback (0);
        }
    }

    IEnumerator SwitchToVR (Action<int> callback) {
        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "cardboard"; // Or "cardboard".
        Debug.Log ("Trying VR to set to device: " + desiredDevice);
        foreach (string device in XRSettings.supportedDevices) {
            Debug.Log ("device: " + device);
        }

        // Some VR Devices do not support reloading when already active, see
        // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
        XRSettings.LoadDeviceByName (desiredDevice);

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return new WaitForSeconds (0.2f);

        // Now it's ok to enable VR mode.
        XRSettings.enabled = true;

        if (callback != null) {
            callback (0);
        }
    }
}
