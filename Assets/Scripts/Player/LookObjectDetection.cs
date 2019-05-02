using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookObjectDetection : MonoBehaviour
{
    [Header("Names")]
    [SerializeField] private string TagButton;
    

    [Header("Variaveis")]
    [SerializeField] private SetDestination sd;

    Ray ray;
    RaycastHit hit;

    public float timeToActive = 0.0f;



    private void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit)){
            if(hit.collider.CompareTag(TagButton)){
                sd = hit.collider.GetComponent<SetDestination>();
                timeToActive += Time.deltaTime;
                if(timeToActive > 1f){
                    sd.ActiveDestination();
                }
            }
            else
            {
                timeToActive = 0f;
            }
        }
    }
}
