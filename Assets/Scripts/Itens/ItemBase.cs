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

	//public void setPanelText(){
	//			currentRoom.AnswerOpen = this;
	//			StopAllCoroutines();
	//   		StartCoroutine(WriteSentence());
	//			LookedAnswer = true;
	//}

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

	//private IEnumerator WriteSentence(){
	//	textPanel.text = string.Empty;
 //   foreach (char letter in properties.answer.ToCharArray())
 //   {
 //   	while (Time.timeScale == 0) yield return null;
 //     textPanel.text += letter;
 //     yield return null;
 //   }
	//	Canvas.ForceUpdateCanvases();
	//}
}