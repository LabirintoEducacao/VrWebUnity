using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVRinWeb : MonoBehaviour
{
    public GameObject vrButton;
    void Start()
    {
        AudioList.instance.PlayMenuBGM();
        #if UNITY_WEBGL
            vrButton.SetActive(false);
        #endif
    }
}
