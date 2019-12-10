using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemBase : MonoBehaviour {
	public BlackBoardManager textPanel;
	public Answer properties;
	public RoomManager currentRoom;
	public AudioSource AudioSource;
	public bool LookedAnswer = false;



	public void ActivePanel() {
		if (!LookedAnswer) {
			EventPool.sendAnswerReadEvent(properties.answer_id);
		}
		if (currentRoom.AnswerOpen) {
			if (currentRoom.AnswerOpen == this) {
				return;
			} else {
				currentRoom.AnswerOpen.LookedAnswer = false;
				startAnswerText();
			}
		} else
			startAnswerText();
	}

	private void startAnswerText() {
		currentRoom.AnswerOpen = this;
		textPanel.setPanelText(properties.answer.ToCharArray());
		LookedAnswer = true;
	}

	/// <summary>
	/// Onde ocorre a ação do item quando selecionado.
	/// </summary>
	public virtual void ActionItem() {
	}

	/// <summary>
	/// Onde ocorre a ação do item quando selecionado.
	/// Nome da porta desejada.
	/// </summary>
	public virtual void ActionItem(string NameDoor) {
	}

}