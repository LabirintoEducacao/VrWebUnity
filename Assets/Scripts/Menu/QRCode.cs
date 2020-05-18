using System.Collections;
using System.Collections.Generic;
using BarcodeScanner.Parser;
using BarcodeScanner.Scanner;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QRCode : MonoBehaviour
{
    Scanner scanner;
    public RawImage target;
    public string sceneAfterRead;
    public CubeButton srcButton;
	public GameObject buttonDialogQRCode;
    bool noCamera = true;
    bool finished = false;
    public List<string> base64bits;

	[Header("WEBGL")]
	public GameObject obsObject;

    public delegate void LoadedQRCodes(Texture2D[] testQRCode);
    public LoadedQRCodes loadedQRCodes;

    void Start()
    {
		loadedQRCodes = LoadedQRCodeFiles;

#if PLATFORM_ANDROID
		buttonDialogQRCode.SetActive(false);
    Destroy(obsObject);
#elif UNITY_STANDALONE
		buttonDialogQRCode.SetActive(true);
	  Destroy(obsObject);
#elif UNITY_WEBGL
		buttonDialogQRCode.SetActive(true);
#endif
	}

	private void OnEnable()
    {
        StartCoroutine(CameraStarter());
    }

    public int base64bitsSize
    {
        get
        {
            if (base64bits == null)
            {
                return 0;
            }
            return base64bits.Count;
        }
    }
    public bool base64BitRead(int bit)
    {
        if ((base64bits == null) || (base64bits.Count <= bit))
        {
            return false;
        }
        return (!string.IsNullOrEmpty(base64bits[bit]));
    }

    void proccessQRCode(string base64bit)
    {
        //append|i|len|<base64data>
        string[] slicedBits = base64bit.Split('|');
        //validations
        if (slicedBits.Length != 4)
        {
            Debug.LogError("Cannot parse QRCode with slices length != 4.  Starts with: " + base64bit.Substring(0, 20));
            return;
        }
        else if (slicedBits[0] != "append")
        {
            Debug.LogError("Invalid QR Code \"metadata\". Starts with... " + base64bit.Substring(0, 20));
            return;
        }
        //real proccessing
        int index = int.Parse(slicedBits[1]) - 1; // their numbering starts at 1 and we start at 0
        int len = int.Parse(slicedBits[2]);
        if (base64bits == null)
        {
            base64bits = new List<string>();
            for (int i = 0; i < len; i++)
            {
                base64bits.Add("");
            }
        }
        base64bits[index] = slicedBits[3];

        //verify if all QR Codes have been read
        bool fin = true;
        for (int i = 0; i < base64bits.Count; i++)
        {
            if ((base64bits[i] == null) || (base64bits[i].Length <= 0))
            {
                fin = false;
                break;
            }
        }
        this.finished = fin;
    }

    IEnumerator CameraStarter()
    {
        noCamera = true;
        finished = false;
        base64bits = null;
#if UNITY_ANDROID && !UNITY_EDITOR
		if (!Permission.HasUserAuthorizedPermission (Permission.Camera)) {
            Permission.RequestUserPermission (Permission.Camera);
            while (this.enabled && !Permission.HasUserAuthorizedPermission (Permission.Camera)) {
                yield return new WaitForSeconds (0.2f);
            }
        }
		noCamera = false;
#else
		// Aask for the authorization to use the webcam
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

		if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {

			foreach (var device in WebCamTexture.devices) {
				Debug.Log("Device Camera: " + device.name);
				noCamera = false;
			}
		} 
		yield return null;
#endif
        // Create a basic scanner
        scanner = new Scanner();
        if (!noCamera)
        {
            Debug.Log("Trying to start Cammera for QR Code Scanner.");

            // Start playing the camera
            scanner.Camera.Play();

            scanner.OnReady += (sender, arg) =>
            {
                // Bind the Camera texture to any RawImage in your scene and start scan
                target.texture = scanner.Camera.Texture;
                scanner.Scan((barCodeType, barCodeValue) =>
                {
                    //Set new level data
                    //string levelDataStr = MazeTools.base64ToText (barCodeValue, true);
                    proccessQRCode(barCodeValue);
                    //DataManager.manager.setNewLevel(levelDataStr);

                });
            };
        }
    }

    public void LoadQRCodeFiles()
    {
#if UNITY_STANDALONE_WIN
		FindObjectOfType<ManagerFileDialogue>().OpenDialogWin(loadedQRCodes);
#elif !UNITY_EDITOR && UNITY_WEBGL
		FindObjectOfType<ManagerFileDialogue>().OpenDialogWeb(loadedQRCodes);
#else
		Debug.Log("não foi possível abrir o Dialog, pois não foi encontrada a respectiva plataforma");
#endif

	}

    private void LoadedQRCodeFiles(Texture2D[] testQRCode)
    {

        foreach (Texture2D t in testQRCode)
        {
            ParserResult result = scanner.Parser.Decode(t.GetPixels32(), t.width, t.height);
            proccessQRCode(result.Value);
            //string levelData = MazeTools.base64ToText (result.Value, true);
        }
        if (!finished)
        {
            Debug.LogError("Should have ended QR Code reading");
        }
    }

    void OnReady(object sender, System.EventArgs args)
    {

    }

    private void OnDisable()
    {
        scanner.Camera.Stop();
        scanner.Destroy();
        scanner = null;
    }

    void FixedUpdate()
    {
        // The barcode scanner has to be updated manually
        if ((scanner != null) && (!noCamera))
            scanner.Update();

        if (this.finished)
        {
            if (!noCamera)
            {
                //stop camera
                scanner.Camera.Stop();
            }
            scanner.Stop();
            string data = "";
            foreach (string bit in base64bits)
            {
                data += bit;
            }
            string levelData = MazeTools.base64ToText(data, true);
            DataManager.manager.setNewLevel(levelData);

            if (srcButton != null)
            {
                srcButton.afterCanvasOK();
            }
            else
            {
                //nova estrutura
                SceneManager.LoadScene(this.sceneAfterRead);
            }
        }
    }
}
