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
    public ControlArrows arrows;
    public Transform[ ] goals; // UP, RIGHT, DOWN, LEFT
    public GameObject playerAgent;

    bool canShow = false;

    public delegate void OnPlayerEnter(HubCheckpoint hub);
    public event OnPlayerEnter onPlayerEnter;

    public delegate void OnPlayerExit(HubCheckpoint hub);
    public event OnPlayerExit onPlayerExit;

    // Start is called before the first frame update
    void Start ( ) {
        if (arrows == null)
            this.arrows = GameObject.FindObjectOfType<ControlArrows> ( );
        // verifyIfPlayerInside();
        if (startingPoint) {
            playerAgent = GameObject.FindGameObjectWithTag("PlayerAgent");
            canShow = true;
        }

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
            canShow = true;
            Player player = GameObject.Find("PlayerVr").GetComponentInChildren<Player>();
            player.currentRoom = this.gameObject.GetComponentInParent<RoomManager>();

            if (onPlayerEnter != null) {
                this.onPlayerEnter(this);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("PlayerAgent")) {
            playerAgent = null;
            // arrows.changeState(false);

            if (this.onPlayerExit != null){
                this.onPlayerExit(this);
            }
        }
    }

    private void FixedUpdate() {
        if (isPlayerInside && canShow) {
            NavMeshAgent agent = playerAgent.GetComponent<NavMeshAgent>();
            if (agent.velocity.magnitude == 0f)
                enableArrows();
        }
    }

    void enableArrows ( ) {
        if (arrows == null)
            this.arrows = GameObject.FindObjectOfType<ControlArrows> ( );
        //gotcha! Ele está aqui dentro
        canShow = false;
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
        }
    }

    public void activate() {
        enableArrows();
    }

    /** define um destino para uma direção */
    public void setGoal(int direction, Transform goal) {
        switch (direction) {
            case Constants.DIRECTION_UP:
                goals[0] = goal;
                break;
            case Constants.DIRECTION_RIGHT:
                goals[1] = goal;
                break;
            case Constants.DIRECTION_DOWN:
                goals[2] = goal;
                break;
            case Constants.DIRECTION_LEFT:
                goals[3] = goal;
                break;
            default:
                break;
        }
    }
}
