using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float y;

    // Update is called once per frame
    void LateUpdate()
    {

        transform.position = new Vector3(target.position.x, (target.position.y + y), target.position.z);
    }
}
