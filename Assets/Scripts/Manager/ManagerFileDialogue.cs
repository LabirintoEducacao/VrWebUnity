using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif
using System.Security;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

public class ManagerFileDialogue : MonoBehaviour {

	public class Data {
		public string[] urls;
	}

#if UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void RequestOpenDialogWeb();
#endif

	private QRCode.LoadedQRCodes methodLoaded;

#if UNITY_STANDALONE_WIN
    OpenFileDialog openFileDialog1;

    // Start is called before the first frame update
    void Start()
    {
        openFileDialog1 = new OpenFileDialog();
        InitializeOpenFileDialog();
    }

    private void InitializeOpenFileDialog()
    {
        // Set the file dialog to filter for graphics files.
        this.openFileDialog1.Filter =
            "Images (*.PNG;*.JPG;)|*.PNG;*.JPG;";

        // Allow the user to select multiple images.
        this.openFileDialog1.Multiselect = true;
        this.openFileDialog1.Title = "Selecione os QRCodes";
    }

    public void OpenDialogWin(QRCode.LoadedQRCodes loadedQRCodes)
    {
        List<Texture2D> listTexture2D = new List<Texture2D>();

        DialogResult dr = this.openFileDialog1.ShowDialog();
        if (dr == System.Windows.Forms.DialogResult.OK)
        {
            // Read the files
            foreach (String file in openFileDialog1.FileNames)
            {
                byte[] fileData;
                //Load Texture2D
                try
                {
                    fileData = File.ReadAllBytes(file);
                    var texture = new Texture2D(2, 2);
                    
                    // Carrega a textura com seu tamanho de largura x altura corretos.
                    texture.LoadImage(fileData);

                    listTexture2D.Add(texture);

                }
                catch (SecurityException ex)
                {
                    // The user lacks appropriate permissions to read files, discover paths, etc.
                    MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                        "Error message: " + ex.Message + "\n\n" +
                        "Details (send to Support):\n\n" + ex.StackTrace
                    );
                }
                catch (Exception ex)
                {
                    // Could not load the image - probably related to Windows file system permissions.
                    MessageBox.Show("Cannot load the image: " + file.Substring(file.LastIndexOf('\\'))
                        + ". You may not have permission to read the file, or " +
                        "it may be corrupt.\n\nReported error: " + ex.Message);
                }
            }
        }

        loadedQRCodes(listTexture2D.ToArray());
    }
#endif

	public void OpenDialogWeb(QRCode.LoadedQRCodes loadedQRCodes) {
#if UNITY_WEBGL
		this.methodLoaded = loadedQRCodes;
		RequestOpenDialogWeb();
#endif
	}

	IEnumerator GetTexture(Data data) {

		List<Texture2D> listTexture2D = new List<Texture2D>();

		for (int i = 0; i < data.urls.Length; i++) {
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(data.urls[i]);
			yield return www.SendWebRequest();


			if (www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			} else {
				Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
				Debug.Log("Loaded QRCode size: " + myTexture.width + "x" + myTexture.height);

				listTexture2D.Add(myTexture);
			}
		}

		Debug.Log("Loaded QRCode Web");
		this.methodLoaded(listTexture2D.ToArray());
	}

	public void LoadedFilesWeb(string jsonData) {

		Data data = JsonUtility.FromJson<Data>("{\"urls\":" + jsonData + "}");

		Debug.Log("Recebido Data: " + data.urls);

		StartCoroutine(GetTexture(data));

	}
}
