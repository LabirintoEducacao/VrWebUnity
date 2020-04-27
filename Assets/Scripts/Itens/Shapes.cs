using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes : ItemBase
{
	public override void ActionItem() {
		// Assegura que a sala atual é a correta.
		GameManager.Instance.ChangeCurrentRoom(currentRoom.GetComponentInChildren<HubCheckpoint>());
	}
}
