using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomGridDisplay : MonoBehaviour
{
	public TextMeshProUGUI roomNameText;
	public TextMeshProUGUI roomIDText;
	public TextMeshProUGUI roomQuestionsText;
	public TextMeshProUGUI roomStatusText;
	public bool oneText = true;

	public Sprite lockedImage;
	public Sprite unlockedImage;
	public Image permissionImage;

	public WebRoomInfo roomInfo { get; set; }
	public bool isLoaded = false;


	private void LateUpdate() {
		if (roomInfo != null) {
			if (this.oneText) {
				roomNameText.text = roomLabel();
			} else {
				this.roomNameText.text = this.roomInfo.name;
				this.roomIDText.text = this.roomInfo.id.ToString();
				this.roomQuestionsText.text = string.Format("{0}/{1}", new object[] {
						roomInfo.Pergunta-roomInfo.Reforco,
						roomInfo.Pergunta 
				});
				this.roomStatusText.text = roomInfo.progressName;
				this.permissionImage.sprite = roomInfo.isPublic ? this.unlockedImage : this.lockedImage;
			}
			this.GetComponent<Button>().interactable = true;
		} else {
			roomNameText.text = "";
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
