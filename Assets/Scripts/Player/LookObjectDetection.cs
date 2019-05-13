using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookObjectDetection : MonoBehaviour
{
    [Header("Variaveis")]
    [SerializeField] private SetDestination sd;
    public float timeToActive = 0.0f, timeToActiveMax = 2.0f, timeFillAmount;
    public float delayLook = 0.0f, delayLookMax = 1.0f;
    public bool HitButton;


    [Header("UI")]
    public Image GUIReticleLoad;

    Ray ray;
    RaycastHit hit;

    private void Start() {
        GUIReticleLoad.gameObject.SetActive(false);
    }

    private void Update() {
        timeFillAmount = timeToActive / timeToActiveMax;
        if (HitButton) SetMyDestiny();
        else
        {
            timeToActive = 0;
            delayLook = 0;
            GUIReticleLoad.fillAmount = 0;
            GUIReticleLoad.gameObject.SetActive(false);
        }
    }

    public void SetMyDestiny()
    {
        delayLook += Time.deltaTime;

        if (delayLook >= delayLookMax)
        {
            GUIReticleLoad.gameObject.SetActive(true);

            timeToActive += Time.deltaTime;
            GUIReticleLoad.fillAmount = timeFillAmount;
            if (timeToActive > timeToActiveMax)
            {
                sd.ActiveDestination();
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);
                HitButton = false;
            }
        }
    }

    public void SetHitButton(SetDestination set)
    {
        HitButton = true;
        if (HitButton) sd = set;
    }

    public void exitButton()
    {
        HitButton = false;
    }
}
