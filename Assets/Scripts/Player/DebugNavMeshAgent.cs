using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DebugNavMeshAgent : MonoBehaviour
{
    public bool debug;
    private NavMeshAgent agent;
    
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        
    }

    private void OnDrawGizmosSelected() {
        if (debug) {
            Gizmos.color = Color.magenta;
            Vector3 lastPos = this.transform.position;
            if (!agent.isStopped) {
                foreach (Vector3 corner in agent.path.corners)
                {
                    Gizmos.DrawLine(lastPos, corner);
                    lastPos = corner;
                }
                Gizmos.DrawLine(lastPos, agent.destination);
            }
        }
    }
}
