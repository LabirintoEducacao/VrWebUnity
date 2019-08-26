using System.Collections;
using System.Collections.Generic;
using BarcodeScanner.Parser;
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
	public Texture2D testQRCode;

    private void OnEnable () {
        StartCoroutine (CameraStarter ());
    }

    IEnumerator CameraStarter () {
		bool noCamera = true;
#if PLATFORM_ANDROID && !UNITY_EDITOR
		if (!Permission.HasUserAuthorizedPermission (Permission.Camera)) {
            Permission.RequestUserPermission (Permission.Camera);
            while (this.enabled && !Permission.HasUserAuthorizedPermission (Permission.Camera)) {
                yield return new WaitForSeconds (0.2f);
            }
        }
		noCamera = false;
#endif
		// Create a basic scanner
		scanner = new Scanner();
		if (noCamera) {
			ParserResult result = scanner.Parser.Decode(testQRCode.GetPixels32(), testQRCode.width, testQRCode.height);
			string levelData = MazeTools.base64ToText (result.Value, true);
			Debug.Log("json... \n"+levelData);
			DataManager.manager.setNewLevel(levelData);
			this.gameObject.SetActive(false);

		} else {
			Debug.Log("Trying to start Cammera for QR Code Scanner.");

			// Start playing the camera
			scanner.Camera.Play();
			yield return null;
			scanner.OnReady += (sender, arg) => {
				// Bind the Camera texture to any RawImage in your scene and start scan
				target.texture = scanner.Camera.Texture;
				scanner.Scan((barCodeType, barCodeValue) => {
					//Set new level data
					string levelDataStr = MazeTools.base64ToText (barCodeValue, true);
					DataManager.manager.setNewLevel(levelDataStr);

					//stop camera
					scanner.Camera.Stop();
					scanner.Stop();
					srcButton.afterCanvasOK();
				});

			};
		}
    }

    private void OnDisable () {
        scanner.Camera.Stop ();
        scanner.Destroy ();
        scanner = null;
    }

    void Update () {
        // The barcode scanner has to be updated manually
        if ((scanner != null) && (scanner.Camera != null) && (scanner.Camera.IsPlaying()))
            scanner.Update ();
    }
}
