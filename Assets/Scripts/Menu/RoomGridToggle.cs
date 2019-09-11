using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomGridToggle : MonoBehaviour
{
	public RoomGridCreator gridCreator;
	public TextMeshProUGUI label;
	public Color publicRoomColor;
	public Color privateRoomColor;

	private void Update() {
		if (gridCreator.privateData) {
			label.text = "Salas Públicas";
			this.GetComponent<Image>().color = this.publicRoomColor;
		} else {
			label.text = "Salas Privadas";
			this.GetComponent<Image>().color = this.privateRoomColor;
			if (!LoginHandler.handler.isValidUser) {
				this.GetComponent<Button>().interactable = false;
				return;
			}
		}
		this.GetComponent<Button>().interactable = true;
	}
}
