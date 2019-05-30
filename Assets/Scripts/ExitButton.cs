using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject _desactivePanel;

    public void desactivePanel()
    {
        _desactivePanel.SetActive(false);
    }
}
