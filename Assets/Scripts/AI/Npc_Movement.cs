using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc_Movement : MonoBehaviour
{
    [SerializeField] Transform _SetDestiny;

    private NavMeshAgent agent;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();

        if(agent == null)
            Debug.Log("Objeto Sem NavMeshAgent!");
    }

    public void SetDestination(){
        Vector3 targetVector = _SetDestiny.transform.position;
        agent.SetDestination(targetVector);
    }
}
