using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc_Movement : MonoBehaviour
{
    [SerializeField] Transform _SetDestiny;

    [SerializeField] private float distanceDestiny;

    private NavMeshAgent agent;

    [SerializeField] private bool _ArrivedDestiny;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();

        if(agent == null)
            Debug.Log("Objeto Sem NavMeshAgent!");
    }

    private void Update() {
        ArrivedAtDestinationCheck();
    }

    private void ArrivedAtDestinationCheck(){
        float distanceToTarget = Vector3.Distance (transform.position, _SetDestiny.position);

        if(distanceToTarget < distanceDestiny){
            _ArrivedDestiny = true;
            
            // agent.Stop();
        } else{
            _ArrivedDestiny = false;
        }
            
    }

    public void SetDestination(Transform target){
        Vector3 targetVector = target.position;
        _SetDestiny = target;
        agent.SetDestination(targetVector);
    }
}
