using System.Collections;
using System.Collections.Generic;
using BarcodeScanner.Scanner;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.UI;

public class QRCode : MonoBehaviour {
    Scanner scanner;
    public RawImage target;
    public CubeButton srcButton;

    private void OnEnable () {
        StartCoroutine (CameraStarter ());
    }

    IEnumerator CameraStarter () {

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission (Permission.Camera)) {
            Permission.RequestUserPermission (Permission.Camera);
            while (this.enabled && !Permission.HasUserAuthorizedPermission (Permission.Camera)) {
                yield return new WaitForSeconds (0.2f);
            }
        }
#endif
        Debug.Log ("Trying to start Cammera for QR Code Scanner.");
        // Create a basic scanner
        scanner = new Scanner ();

        // Start playing the camera
        scanner.Camera.Play ();
        scanner.OnReady += (sender, arg) => {
            // Bind the Camera texture to any RawImage in your scene and start scan
            target.texture = scanner.Camera.Texture;
            scanner.Scan ((barCodeType, barCodeValue) => {
                //Set new level data
                string levelDataStr = MazeTools.base64ToText (barCodeValue, true);
                DataManager.manager.setNewLevel (levelDataStr);

                //stop camera
                scanner.Camera.Stop ();
                scanner.Stop ();
                srcButton.afterCanvasOK();
            });

        };
    }

    private void OnDisable () {
        scanner.Camera.Stop ();
        scanner.Destroy ();
        scanner = null;
    }

    void Update () {
        // The barcode scanner has to be updated manually
        if (scanner != null)
            scanner.Update ();
    }
}
