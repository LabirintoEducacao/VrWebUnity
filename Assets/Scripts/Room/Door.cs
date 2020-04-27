using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Door : CheckBase {
	/* Quando acontecer uma interação com a porta, o script deve verificar se a reposta(item)
	 * que o jogador carrega com ele é o que deve ser utilizado para abrir a porta em que este
	 * script se encontra.
	 * 
	 * Caso seja a resposta errada, o jogador perderá uma estrela.
	 */

	public linkDoor ld;

	private void Update() {
		if (answer != ld.answerLinked)
			answer = ld.answerLinked;
	}

	/// <summary>
	/// Se a resposta for igual ao da porta, a porta abre, se não, a porta permanece fechada.
	/// </summary>
	/// <param name="ans"></param>
	/// <returns></returns>
	public override bool checkAnswer(Answer ans) {
		if (ans == answer) {
			anim.SetTrigger("openning");

			// Limpar respostas ao acertar a pergunta
			foreach (ItemBase item in GameManager.Instance.currentRoom.answerReference) {
				item.gameObject.SetActive(false);
			}

			Inventory.instance.item = null;
			GameManager.Instance.setExitMotionInHub();

			return true;
		} else {
			anim.SetTrigger("wrongAnswer");
		}
		return false;
	}

	

	/*
	Animator anim = door.thisAnimator;
											anim.SetTrigger("openning");
*/
}
