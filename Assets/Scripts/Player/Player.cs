using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayerBase
{
    [Header("Components")]
    public LayerMask layerButton;
    public GameObject button;
    public Image GUIReticleLoad;

    [Header("Variables")]
    public float currentTimeUnlock;
    public float timeToUnlock;
    public float timeToLoadFillAmount;
    public float currentTimeLoadFillAmount;
    public bool IsSeted;

    private void Start()
    {
        GUIReticleLoad.gameObject.SetActive(false);
    }

    public SetTarget st;

    private void Update()
    {
        Ray ray = new Ray(this.transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, layerButton))
        {
            if (hit.collider.CompareTag("Button"))
            {
                currentTimeUnlock += Time.deltaTime;
                if (currentTimeUnlock >= timeToUnlock)
                {
                    GUIReticleLoad.gameObject.SetActive(true);
                    currentTimeLoadFillAmount += Time.deltaTime;
                    GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);
                    button = hit.collider.gameObject;
                    st = button.GetComponent<SetTarget>();
                    if (st != null & currentTimeLoadFillAmount >= timeToLoadFillAmount)
                    {
                        SetTarget(st.Target.position);
                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);

                        currentTimeLoadFillAmount = 0;
                        currentTimeUnlock = 0;
                    }
                }
            }
            else
            {
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);

                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
            }
        }
        else
        {
            GUIReticleLoad.fillAmount = 0;
            GUIReticleLoad.gameObject.SetActive(false);

            currentTimeLoadFillAmount = 0;
            currentTimeUnlock = 0;
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }

    internal override void SetTarget(Vector3 target)
    {
        IsSeted = true;
        agent.SetDestination(target);
    }
}
