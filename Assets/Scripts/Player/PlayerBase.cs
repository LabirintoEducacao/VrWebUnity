using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBase : MonoBehaviour
{
    //Components
    public NavMeshAgent agent;

    internal Vector3 _target;
    public Rigidbody rb;

    private void Update()
    {
        if (transform.position == _target)
        {
            agent.SetDestination(Vector3.zero);
        }
    }

    internal virtual void SetTarget(Vector3 target)
    {

    }

}
