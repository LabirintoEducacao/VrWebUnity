using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (GameManager))]
public class GameManagerEditor : Editor {
    public Texture2D qrCode;
    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();

        GameManager obj = target as GameManager;
        if (GUILayout.Button ("Base64 json")) {
            if (obj.levelDesign != null) {
                string encodedText = MazeTools.textToBase64 (obj.levelDesign.text, false);
                // Debug.Log (encodedText);
            }
        }

        if (GUILayout.Button ("Generate QRCode")) {
            if (obj.levelDesign != null) {
                qrCode = MazeTools.GenerateQRCode (obj.levelDesign.text, 256, 256);
            }
        }
        if (qrCode) {
            GUILayout.Box (qrCode);
            // EditorGUI.PrefixLabel (new Rect (25, 45, 100, 15), 0, new GUIContent ("QR Code:"));
            // EditorGUI.DrawPreviewTexture (new Rect (25, 60, 100, 100), qrCode);
            if (GUILayout.Button ("Save")) {
                string path = EditorUtility.SaveFilePanel ("QR Code", Application.persistentDataPath, "level_qrcode.png", "png");

                if (path.Length > 0) {
                    File.WriteAllBytes (path, qrCode.EncodeToPNG ());
                }
            }
        }
        if (GUILayout.Button ("Teste")) {
            if (obj.levelDesign != null) {
                Debug.Log ("Test with real level design.");
                MazeTools.testConvertion (obj.levelDesign.text);

            }
            Debug.Log ("Test with abacate. Abacate é bom! Come abacate, come.");
            string t = "abacate é bom pra pele. Come abacate, come.";
            MazeTools.testConvertion (t);
        }
    }
}
