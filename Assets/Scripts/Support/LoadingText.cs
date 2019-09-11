using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    public float time = 0.2f;
    public string fullText = "CARREGANDO...";
    TextMeshProUGUI textHolder;


    private void OnEnable() {
        InvokeRepeating("refreshText", time, time);
    }
    private void OnDisable() {
        CancelInvoke("refreshText");
    }

    void refreshText() {
        this.textHolder = GetComponent<TextMeshProUGUI>();
        int status = (textHolder.text.Length + 1) % (fullText.Length+1);
        textHolder.text = fullText.Substring(0,status);
    }
}
