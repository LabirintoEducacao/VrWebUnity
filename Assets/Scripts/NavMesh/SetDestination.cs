using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDestination : MonoBehaviour
{
    [Header("Checkpoint")]
    [SerializeField] private Transform _checkpoint;
    private bool DestinyIsTrue = false;
    private Checkpoint check;

    [Header("Components")]
    private MeshRenderer myMesh;
    private BoxCollider myCollider;

    [Header("Scripts")]
    private Npc_Movement npc;
    [SerializeField] private string playerName;

    private void Awake() {
        npc = GameObject.Find(playerName).GetComponent<Npc_Movement>();

        myMesh = GetComponent<MeshRenderer>();
        myCollider = GetComponent<BoxCollider>();
    }

    private void OnMouseDown() {
        ActiveDestination();        
    }

    public void ActiveDestination(){
        npc.SetDestination(_checkpoint);
        myMesh.enabled = false;
        myCollider.enabled = false;
        DestinyIsTrue = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Checkpoint") && DestinyIsTrue){
            check = other.gameObject.GetComponent<Checkpoint>();
            check.desactiveButtons();
            DestinyIsTrue = false;
        }
    }
}
