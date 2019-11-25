using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackBoardManager : MonoBehaviour
{
	public TextMeshProUGUI textPanel;

	public void setPanelText(char[] letters) {
		StopAllCoroutines();
		StartCoroutine(WriteSentence(letters));
	}

	private IEnumerator WriteSentence(char[] letters) {
		textPanel.text = string.Empty;
		foreach (char letter in letters) {
			while (Time.timeScale == 0) yield return null;
			textPanel.text += letter;
			yield return null;
		}
		Canvas.ForceUpdateCanvases();
	}
}
