using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerBase
{
    public LayerMask layerButton;
    public GameObject button;

    public SetTarget st;

    private void Update()
    {
        Ray ray = new Ray(this.transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, layerButton))
        {
            Debug.Log("Button atigindo");
            button = hit.collider.gameObject;
            st = button.GetComponent<SetTarget>();
            if (st != null)
            {
                SetTarget(st.Target.position);
            }
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }

    internal override void SetTarget(Vector3 target)
    {
        agent.SetDestination(target);
    }
}
