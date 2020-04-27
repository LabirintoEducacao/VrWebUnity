using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ItemBase
{
	public Animator anim;

	public bool isActivate = false;


	public override void ActionItem() {
		// Assegura que a sala atual é a correta.
		GameManager.Instance.ChangeCurrentRoom(currentRoom.GetComponentInChildren<HubCheckpoint>());

		// TODO Ativa a alavanca
		isActivate = !isActivate;
		anim.SetBool("isActivate", isActivate);
	}
}
