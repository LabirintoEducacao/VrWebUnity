using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : ItemBase {

	private void Start() {
		GetComponent<Animator>().SetBool("show", false);
	}

	public override void ActionItem() {
		
		// Assegura que a sala atual é a correta.
		GameManager.Instance.ChangeCurrentRoom(currentRoom.GetComponentInChildren<HubCheckpoint>());

		if (currentRoom.type == TypeRoom.hope_door)
			currentRoom.PositionNextRoomOfHopeDoor(transform.parent.name, properties.correct);
		else
			currentRoom.PositionNextRoom("DoorAnswer", properties.correct);
	}

	void OnEnable() {
		GetComponent<Animator>().SetBool("show", true);
	}


}
