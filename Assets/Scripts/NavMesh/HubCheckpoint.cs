using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class HubCheckpoint : MonoBehaviour
{
    private ControlArrows arrows;
    public Transform[] goals;

    // Start is called before the first frame update
    void Start()
    {
        if (arrows == null)
            this.arrows = GameObject.FindObjectOfType<ControlArrows>();
        verifyIfPlayerInside();    
    }

    private void OnEnable() {
        this.arrows = GameObject.FindObjectOfType<ControlArrows>();
    }

    private void OnDisable() {
        this.arrows = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void verifyIfPlayerInside() {
        Player player = GameObject.FindObjectOfType<Player>();
        SphereCollider coll = this.GetComponent<SphereCollider>();
        float dist = Vector3.Distance(player.transform.position, coll.transform.position);
        if (dist < coll.radius) {
            //gotcha! Ele está aqui dentro
            for (int i = 0; i < goals.Length; i++) {
                arrows.setGoal(Constants.DIRECTIONS[i], goals[i]);
            }
            arrows.showArrows();
        }
    }
}
