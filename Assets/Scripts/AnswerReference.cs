using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerReference : MonoBehaviour
{
    public TextMeshProUGUI textPanel;
    public GameObject panel;

    private void Start()
    {
       panel.SetActive(false);
    }

    public void ActivePanel()
    {
        panel.SetActive(true);
    }
    public void DesactivePanel()
    {
        panel.SetActive(false);
    }
}
