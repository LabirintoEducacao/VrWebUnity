using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Transform target;
    public bool Y;

    private void LateUpdate() {
        if(Y){
            transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
        } else
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
    }
}
