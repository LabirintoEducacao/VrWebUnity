using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBase : MonoBehaviour {
	public RoomManager rmanager;
	public Answer answer;
	public Animator anim;

	public bool seted;
	public bool openDoor;

	public DoorStructure door;

	// Start is called before the first frame update
	public virtual bool checkAnswer(Answer ans) {
		return false;
	}
	public void SetNextDoor() {
		Player.instance.currentRoom.SetDoorAnswer(0);
	}

	// void Update() {
	//     if(ld != null && answer == null && !seted){
	//         answer = ld.answerLinked;
	//         seted = true;
	//     }
	// }

}
