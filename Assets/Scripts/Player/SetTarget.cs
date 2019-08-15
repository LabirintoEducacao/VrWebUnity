using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTarget : MonoBehaviour
{
    public delegate void OnTargetSelected(SetTarget actioneer, Transform target);
    public event OnTargetSelected onTargetSelected;

    public Transform Target;

    public void select() {
        if (onTargetSelected != null) {
            //Debug.Log("Entrou aqui;");
            onTargetSelected(this, Target);
        }
    }
}
