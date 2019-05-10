using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookObjectDetection : MonoBehaviour
{
    [Header("Names")]
    [SerializeField] private string TagButton;
    

    [Header("Variaveis")]
    [SerializeField] private SetDestination sd;
    public float timeToActive = 0.0f, timeToActiveMax = 2.0f, timeFillAmount;
    public float delayLook = 0.0f, delayLookMax = 1.0f;


    [Header("UI")]
    public Image GUIReticleLoad;

    Ray ray;
    RaycastHit hit;

    private void Start() {
        GUIReticleLoad.gameObject.SetActive(false);
    }

    private void Update() {
        timeFillAmount = timeToActive / timeToActiveMax;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit)){
            if(hit.collider.tag == TagButton){
                sd = hit.collider.GetComponent<SetDestination>();
                delayLook += Time.deltaTime;
                
                if(delayLook >= delayLookMax){
                    GUIReticleLoad.gameObject.SetActive(true);
                    
                    timeToActive += Time.deltaTime;
                    GUIReticleLoad.fillAmount = timeFillAmount;
                    if(timeToActive > timeToActiveMax){
                        sd.ActiveDestination();
                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);
                    }
                }
            }
            else if (hit.collider == null){
                timeToActive = 0f;
                delayLook = 0f;
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);
            }
            else {
                timeToActive = 0f;
                delayLook = 0f;
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);
            }
        }
    }
}
