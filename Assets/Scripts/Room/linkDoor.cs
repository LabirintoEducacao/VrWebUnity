using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linkDoor : MonoBehaviour {
	public Answer answerLinked;
	public Door thisDoor;

	private void Start() {
		thisDoor.answer = answerLinked;
	}
	public void SetSeta() {

	}

	/// <summary>
	/// Quando termina animação de fechar a porta.
	/// </summary>
	public void ClosedDoor() {
		GameManager.Instance.ClosedGatewayDoor();
	}
}
