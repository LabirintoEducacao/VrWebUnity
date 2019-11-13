using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemBase : MonoBehaviour {
	public TextMeshProUGUI textPanel;
	public GameObject panel;
	public Answer properties;
	public RoomManager currentRoom;

	private void Start() {
		textPanel.text = properties.answer;
		DesactivePanel();
	}

	public void ActivePanel() {
		if (!panel.activeSelf) {
			EventPool.sendAnswerReadEvent(properties.answer_id);
		}
		panel.SetActive(true);
	}
	public void DesactivePanel() {
		panel.SetActive(false);
	}

	void OnBecameInvisible() {
		DesactivePanel();
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
