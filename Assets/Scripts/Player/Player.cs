using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using System;

public class Player : PlayerBase {
	[Header("Components")]
	public Image GUIReticleLoad;
	public ExitButton exit;
	public RoomManager currentRoom;

	[Header("Variables")]
	public float currentTimeUnlock;
	public float timeToUnlock;
	public float timeToLoadFillAmount;
	public float currentTimeLoadFillAmount;
	public float actionDistance = 4f;
	public bool IsSeted;
	public SetTarget _SetTarget;

	private Vector3 targetPosition; // Continue Path

	public static Player instance;

	[Header("Mouse")]
	private bool clicked = false;

	private void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}
	private void Start() {
		GUIReticleLoad.gameObject.SetActive(false);
	}

	private void Update() {
		DetectClickMouse();
		DetectInteraction();
	}

	private void DetectClickMouse() {
#if !UNITY_ANDROID
		if (Input.GetMouseButtonDown(0)) {
			ResetGUIReticleLoad();

			clicked = true;
		}
#endif
	}

	private void DetectInteraction() {
		Ray ray = new Ray(this.transform.position, transform.forward);
		RaycastHit hit;

		//Metodos para detecção
		if (Physics.Raycast(ray, out hit, this.actionDistance)) {
			Debug.Log(hit.collider.tag);
			if (hit.collider.CompareTag("Button")) {
				hitArrow(hit);
			} else if (hit.collider.CompareTag("Item")) {
				hitItem(hit);
			} else if (hit.collider.CompareTag("Door")) {
				hitCheckAnswer(hit);
			} else if (hit.collider.CompareTag("CheckShapes")) {
				hitCheckShapes(hit);
			} else if (hit.collider.CompareTag("BackToMenu")) {
				hitBackToMenu(hit);
			} else if (hit.collider.CompareTag("ExitGame")) {
				hitExitGame(hit);
			} else if (hit.collider.CompareTag("Lever")) {
				hitInteractObject(hit);
			} else if (hit.collider.CompareTag("LeversCheck")) {
				hitCheckLevers(hit);
			} else {

				ResetGUIReticleLoad();
			}
		} else {

			ResetGUIReticleLoad();
		}
		Debug.DrawRay(ray.origin, ray.direction, Color.red);
	}

	private void ResetGUIReticleLoad() {
		clicked = false;

		GUIReticleLoad.fillAmount = 0;
		GUIReticleLoad.gameObject.SetActive(false);

		currentTimeLoadFillAmount = 0;
		currentTimeUnlock = 0;
	}

	private bool ProcessReticleLoad() {

		currentTimeUnlock += Time.deltaTime;
		if (currentTimeUnlock >= timeToUnlock) {
			GUIReticleLoad.gameObject.SetActive(true);
			currentTimeLoadFillAmount += Time.deltaTime;
			GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

			if (currentTimeLoadFillAmount > timeToLoadFillAmount) {
				return true;
			}
		}

		return false;
	}

	#region Hit Detection
	void hitArrow(RaycastHit hit) {

		if (ProcessReticleLoad() || clicked) {
			_SetTarget = hit.collider.gameObject.GetComponent<SetTarget>();

			//foi acionado, mandando o agent se mexer e reiniciando as variáveis.
			targetPosition = _SetTarget.Target.position;
			SetTarget(targetPosition);
			_SetTarget.select();

			ResetGUIReticleLoad();
		}

	}

	/// <summary>
	/// Verifica e analisa o objeto com tag "Item" visto pelo o jogador.
	/// A verificação ocorre, quando o jogador fica olhando o objeto por um determinado tempo.
	/// Após o tempo, o objeto é colocado no inventário e caso já esteja com um outro objeto no inventário, eles são trocados.
	/// No final a resposta contida no objeto é salva.
	/// </summary>
	/// <param name="hit">Objeto "Item" visto pelo jogador</param>
	void hitItem(RaycastHit hit) {
		if (ProcessReticleLoad() || clicked) {
			//Caso já tenha algo no inventário, trocar
			if (Inventory.instance.item != null) {
				Inventory.instance.ItemObject.SetActive(true);
			}
			//Pegando o novo objeto
			Inventory.instance.item = null;
			Inventory.instance.item = hit.collider.gameObject.GetComponentInParent<ItemBase>();
			ItemBase item = hit.collider.gameObject.GetComponentInParent<ItemBase>();
			item.ActionItem();
			EventPool.sendAnswerInteractionEvent(item.properties.answer_id, item.properties.correct);
			Inventory.instance.ItemObject = Inventory.instance.item.gameObject;
			Inventory.instance.ItemObject.gameObject.SetActive(false);
			AudioList.instance.PlayPlayerChave();//É possivel detectar a diferença entre chave, objetos, alavanca etc?? Para usar sons diferentes
			ResetGUIReticleLoad();
		}
	}


	/// <summary>
	/// Verifica a porta com a resposta
	/// </summary>
	/// <param name="hit"></param>
	void hitCheckAnswer(RaycastHit hit) {
		if (ProcessReticleLoad() || clicked) {
			CheckBase checkDoor = hit.collider.GetComponent<CheckBase>();

			if (checkDoor != null) {

				if (Inventory.instance.item != null && checkDoor.answer.answer != null) {

					bool isCoorect = Inventory.instance.AnswerSelected.correct;
					checkDoor.checkAnswer(Inventory.instance.AnswerSelected); // Open Door
																																		

					if (currentRoom.type == TypeRoom.hope_door || currentRoom.type == TypeRoom.right_key) {
						DataManager.manager.answerStatus(this.currentRoom.id, isCoorect); //dispara evento para registrar a resposta no analytics

						if (isCoorect) {
							Debug.Log("Resposta Certa!");

						} else {
							Debug.Log("Resposta errada!");
						}
					}

					ResetGUIReticleLoad();
				}
			}
		}
	}

	/// <summary>
	/// Verifica a porta com a resposta
	/// </summary>
	/// <param name="hit"></param>
	void hitCheckShapes(RaycastHit hit) {
		if (ProcessReticleLoad() || clicked) {
			CheckBase checkShapes = hit.collider.GetComponent<CheckBase>();

			if (checkShapes != null) {

				if (Inventory.instance.item != null && checkShapes.answer.answer != null) {

					bool isCoorect = Inventory.instance.AnswerSelected.correct;
					checkShapes.checkAnswer(Inventory.instance.AnswerSelected); 
																																		

					DataManager.manager.answerStatus(this.currentRoom.id, isCoorect);

					if (isCoorect) {
						Debug.Log("Resposta Certa!");

					} else {
						Debug.Log("Resposta errada!");
					}
					

					ResetGUIReticleLoad();
				}
			}
		}
	}


	/// <summary>
	/// Método para interagir com um determinado objeto
	/// </summary>
	/// <param name="hit">Objeto "InteractObject" visto pelo jogador</param>
	void hitInteractObject(RaycastHit hit) {

		if (ProcessReticleLoad() || clicked) {
			ItemBase obj = hit.collider.gameObject.GetComponentInParent<ItemBase>();
			obj.ActionItem();

			ResetGUIReticleLoad();
		}
	}

	/// <summary>
	/// Verifica se as alavancas estão ativadas corretamente
	/// </summary>
	/// <param name="hit">Objeto "CheckLevers" visto pelo jogador</param>
	private void hitCheckLevers(RaycastHit hit) {

		if (ProcessReticleLoad() || clicked) {
			CheckBase checkLevers = hit.collider.GetComponent<CheckBase>();

			bool correct = checkLevers.checkAnswer(null);

			if (correct) {
				Debug.Log("Resposta Certa!");
			} else {
				Debug.Log("Resposta errada!");
			}
			//dispara evento para registrar a resposta no analytics
			DataManager.manager.answerStatus(this.currentRoom.id, correct);

			ResetGUIReticleLoad();
		}
	}
	void hitBackToMenu(RaycastHit hit) {
		if (ProcessReticleLoad() || clicked) {
			MenuInGame menu = hit.collider.GetComponent<MenuInGame>();
			if (menu != null)
				Debug.Log("Menu não está vazio!");

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
#endif

			menu.backToMenu("MainMenu_v2");
			ResetGUIReticleLoad();
		}
	}

	void hitExitGame(RaycastHit hit) {
		if (ProcessReticleLoad() || clicked) {
			MenuInGame menu = hit.collider.GetComponent<MenuInGame>();
			if (menu != null)
				Debug.Log("Menu não está vazio!");

			menu.quitGame();

			ResetGUIReticleLoad();
		}
	}
	#endregion
	internal override void SetTarget(Vector3 target) {
		IsSeted = true;
		agent.SetDestination(target);
	}

	public void ContinuePath() {
		if (_SetTarget) {
			agent.SetDestination(targetPosition);
		}
	}
}
