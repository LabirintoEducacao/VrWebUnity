using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : ItemBase {

	public override void ActionItem() {
		currentRoom.PositionNextRoom("DoorAnswer", properties.correct);
	}
}
