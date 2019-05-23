using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;
using UnityEngine.AI;

public class HubCheckpoint : MonoBehaviour {
    public MapCoord coord;
    public bool startingPoint = false;
    public bool isPlayerInside {
        get { return playerAgent != null; }
    }
    private ControlArrows arrows;
    public Transform[ ] goals;
    GameObject playerAgent;

    // Start is called before the first frame update
    void Start ( ) {
        if (arrows == null)
            this.arrows = GameObject.FindObjectOfType<ControlArrows> ( );
        playerAgent = GameObject.FindGameObjectWithTag("PlayerAgent");
        // verifyIfPlayerInside();
    }

    private void OnEnable ( ) {
        this.arrows = GameObject.FindObjectOfType<ControlArrows> ( );
    }

    private void OnDisable ( ) {
        this.arrows = null;
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.CompareTag ("PlayerAgent")) {
            playerAgent = other.gameObject;
            enableArrows();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("PlayerAgent")) {
            playerAgent = null;
            // arrows.changeState(false);
        }
    }

    private void FixedUpdate() {
        if (isPlayerInside) {
            if (playerAgent.GetComponent<NavMeshAgent>().isStopped)
                enableArrows();
        }
    }

    void enableArrows ( ) {
        //gotcha! Ele está aqui dentro
        for (int i = 0; i < goals.Length; i++) {
            arrows.setGoal (Constants.DIRECTIONS[i], goals[i]);
        }
        arrows.showArrows ( );
    }

    void verifyIfPlayerInside ( ) {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerAgent");
        SphereCollider coll = this.GetComponent<SphereCollider> ( );
        float dist = Vector3.Distance (player.transform.position, coll.transform.position);
        if (dist < coll.radius) {
            playerAgent = player;
            enableArrows ( );
        }
    }
}
