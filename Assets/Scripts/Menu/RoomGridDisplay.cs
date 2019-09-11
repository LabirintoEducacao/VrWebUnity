using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomGridDisplay : MonoBehaviour
{
	public TextMeshProUGUI textHolder;
	public WebRoomInfo roomInfo { get; set; }
	public bool isLoaded = false;


	private void LateUpdate() {
		if (roomInfo != null) {
			textHolder.text = roomLabel();
			this.GetComponent<Button>().interactable = true;
		} else {
			textHolder.text = "";
			this.GetComponent<Button>().interactable = false;
		}
	}

	private void OnDestroy() {
		roomInfo = null;
	}

	string roomLabel() {
		return string.Format("<b>{0}.{1}</b> - {2} a {3} perguntas", new object[] {
			roomInfo.id,
			roomInfo.name,
			roomInfo.Pergunta-roomInfo.Reforco,
			roomInfo.Pergunta
		});
	}

	public void click() {
		var crea = GetComponentInParent<RoomGridCreator>();
		if (crea == null) {
			Debug.LogError("No creator found!!!!");
			return;
		}
		crea.loadGame(this.roomInfo, isLoaded);
	}
}
