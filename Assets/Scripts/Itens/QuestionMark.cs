using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionMark : MonoBehaviour {

	public Animator anim;

	public TextMeshProUGUI textPanel;
	public Question properties;
	public bool LookedQuestion = false;

	public void setPanelText() {

		if (!LookedQuestion) {
			StartCoroutine(WriteSentence());
			anim.Play("HideMark");
			LookedQuestion = true;
		}
	}

	private IEnumerator WriteSentence() {
		textPanel.text = string.Empty;
		foreach (char letter in properties.question.ToCharArray()) {
			while (Time.timeScale == 0) yield return null;
			textPanel.text += letter;
			yield return null;
		}
		Canvas.ForceUpdateCanvases();
	}
}
