using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDestination : MonoBehaviour
{
    [Header("Checkpoint")]
    [SerializeField] private Transform _checkpoint;
    public Checkpoint check;

    [Header("Components")]
    private MeshRenderer myMesh;
    private BoxCollider myCollider;

    [Header("Scripts")]
    private PlayerMovement npc;
    [SerializeField] private string TagPlayer;

    private void Awake() {
        npc = GameObject.FindGameObjectWithTag(TagPlayer).GetComponent<PlayerMovement>();

        myMesh = GetComponent<MeshRenderer>();
        myCollider = GetComponent<BoxCollider>();
    }

    private void OnMouseDown() {
        ActiveDestination();        
    }

    public void ActiveDestination(){
        npc.SetDestination(_checkpoint);
        check.desactiveButtons();
    }
}
