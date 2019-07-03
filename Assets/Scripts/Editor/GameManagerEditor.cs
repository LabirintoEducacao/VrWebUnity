using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.IO.Compression;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Base64 json")) {
            GameManager obj = target as GameManager;
            if (obj.levelDesign != null) {
                string ld = obj.levelDesign.text;
                byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (ld);
                string encodedText = "";
                using (MemoryStream stream = new MemoryStream()) {
                    using (GZipStream zip = new GZipStream(stream, System.IO.Compression.CompressionLevel.Optimal) ) {
                        zip.Write(bytesToEncode, 0, bytesToEncode.Length);
                    }
                    encodedText =  System.Convert.ToBase64String (stream.ToArray());
                }
                Debug.Log(encodedText);
            }
        }
    }
}