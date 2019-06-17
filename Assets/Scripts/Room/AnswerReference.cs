using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerReference : MonoBehaviour
{
    public TextMeshProUGUI textPanel;
    public GameObject panel;
    public Answer properties;

    private void Start() {
        textPanel.text = properties.answer;
        DesactivePanel();
    }

    public void ActivePanel()
    {
        panel.SetActive(true);
    }
    public void DesactivePanel()
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// OnBecameInvisible is called when the renderer is no longer visible by any camera.
    /// </summary>
    void OnBecameInvisible()
    {
        panel.SetActive(false);
        Debug.Log("Entrou aqui!");
    }
}
