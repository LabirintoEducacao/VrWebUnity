using System;
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Animations;

public enum TypeRoom {
	start_room,
	right_key,
	hope_door,
	multiple_forms,
	true_or_false,
	end_room
}

public struct DoorStructure {
	public string name;
	public linkDoor doorProperties;
	public int doorPosition;
	public Animator anim;
	public Transform doorTransform;
}
public class RoomManager : MonoBehaviour {
	[Header("Answer")]
	public int entranceDirection = Constants.DIRECTION_NONE;
	public Transform[] spawnAnswer;
	public List<ItemBase> answerReference;
	[Tooltip("Prefabs das respostas que serão instanciados no mapa.\n 0 - Key\n 1 - Cube\n 2 - Prism\n 3 - Circle")]
	public GameObject[] answerPrefab;
	public BlackBoardManager textPanel;
	public ItemBase AnswerOpen;

	/// <summary>
	/// PortDatas armazena as portas da sala, sendo assim: 0 - Gateway e as demais portas de saída.
	/// </summary>
	[Header("Door")]
	[Tooltip("PortDatas armazena as portas da sala, sendo assim: 0 - Gateway e as demais portas de saída.")]
	public GameObject[] Doors;
	public Transform[] PositionNextCorridor;
	public DoorStructure[] portDatas;
	public int currentPositionCorridor;

	[Header("Question")]
	public TextMeshProUGUI TextQuestion;
	public QuestionMark mark;

	[Header("room data")]
	public Question question;
	public int id { get => question.question_id; }
	public TypeRoom type;
	public GameManager manager;
	public string availbility = "";

	[Header("Extras")]
	public checkShapes CheckFormAnswer;
	public CheckLevers CheckLeverAnswer;

	//Metodos
	public void generateAnswers() {
		SetTypeRoom();

		Answer[] answers = question.answers;
		GameObject[] answerTemp = new GameObject[answers.Length];
		answerReference = new List<ItemBase>();

		List<Transform> molds = new List<Transform>(spawnAnswer);
		Tools.Shuffle(molds);

		if (type == TypeRoom.right_key) {
			for (int i = 0; i < answers.Length; i++) {
				GameObject go = Instantiate(answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
				ItemBase ansRef = go.GetComponent<ItemBase>();
				ansRef.properties = answers[i];
				ansRef.textPanel = textPanel;
				ansRef.currentRoom = this;
				answerReference.Add(ansRef);
			}
		} else if (type == TypeRoom.hope_door) {
			for (int i = 0; i < answers.Length; i++) {
				GameObject go = Instantiate(answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
				ItemBase ansRef = go.GetComponent<ItemBase>();
				ansRef.properties = answers[i];
				ansRef.textPanel = textPanel;
				ansRef.currentRoom = this;
				answerReference.Add(ansRef);
			}
		} else if (type == TypeRoom.multiple_forms) {
			List<int> count = new List<int>();
			List<int> shapes = new List<int>();
			int i = 0;
			while (count.Count < 3) {
				i = UnityEngine.Random.Range(0, 3);
				if (!count.Contains(i)) {
					int j = UnityEngine.Random.Range(1, 4);
					if (!shapes.Contains(j)) {
						GameObject go = Instantiate(answerPrefab[j], molds[i].position, molds[i].rotation, molds[i]);
						ItemBase ansRef = go.GetComponent<ItemBase>();
						ansRef.properties = answers[i];
						ansRef.textPanel = textPanel;
						ansRef.currentRoom = this;
            answerReference.Add(ansRef);
						count.Add(i);
						shapes.Add(j);
					}
				}
			}
		} else if (type == TypeRoom.true_or_false) {
			for (int i = 0; i < answers.Length; i++) {
				Vector3 newPosition = molds[i].position;
				newPosition.y += 0.5f;
				GameObject go = Instantiate(answerPrefab[4], newPosition, molds[i].rotation, molds[i]);
				ItemBase ansRef = go.GetComponent<ItemBase>();
				ansRef.properties = answers[i];
				ansRef.textPanel = textPanel;
				ansRef.currentRoom = this;
				answerReference.Add(ansRef);
			}

			foreach (ItemBase item in answerReference) {
				Lever lever = item.gameObject.GetComponent<Lever>();
				CheckLeverAnswer.listLevers.Add(lever);
			}

			CheckLeverAnswer.rmanager = this;

			CheckLeverAnswer.door = portDatas[1];
		}

		mark.properties = question;
	}

	/// <summary>
	/// Identificação e Criação da sala
	/// </summary>
	public void SetTypeRoom() {
		if (question.room_type == "start_room") {
			type = TypeRoom.start_room;
			Start_Room();
		} else if (question.room_type == "right_key") {
			type = TypeRoom.right_key;
			Room_right_key();
		} else if (question.room_type == "hope_door") {
			type = TypeRoom.hope_door;
			Room_hope_door();
		} else if (question.room_type == "multiple_forms") {
			type = TypeRoom.multiple_forms;
			RoomTypeObjectsAndShapes();
		} else if (question.room_type == "true_or_false") {
			type = TypeRoom.true_or_false;
			Room_true_or_false();
		} else if (question.room_type == "end_room") {
			type = TypeRoom.end_room;
			End_Room();
		} else if ((int)type == 3)
			Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);
	}
	#region Types_Of_Room

	/// <summary>
	/// Criação da sala Start Room
	/// </summary>
	void Start_Room() {
		portDatas = new DoorStructure[2];
		DoorStructure ds = new DoorStructure();
		int count = 0;
		//Salvando as propriedades da porta de entrada
		foreach (GameObject door in Doors) {
			StructureDoor structure = door.GetComponent<StructureDoor>();
			ds.name = structure.name;
			ds.doorProperties = structure.doorProperties;
			ds.doorPosition = structure.doorPosition;
			ds.anim = structure.anim;
			portDatas[count] = ds;
			count++;
		}
	}

	/// <summary>
	/// Criação da sala Right Key Room
	/// </summary>
	void Room_right_key() {
		portDatas = new DoorStructure[2];
		DoorStructure ds = new DoorStructure();
		int count = 0;
		//Salvando as propriedades da porta de entrada
		foreach (GameObject door in Doors) {
			StructureDoor structure = door.GetComponent<StructureDoor>();
			ds.name = structure.name;
			ds.doorProperties = structure.doorProperties;
			ds.doorPosition = structure.doorPosition;
			ds.anim = structure.anim;
			portDatas[count] = ds;
			count++;
		}
		/*foreach (DoorStructure door in portDatas) {
			Debug.Log(door.name + "\n" + door.doorProperties);
		}*/
	}


	/// <summary>
	/// Criação da Hope Door Room
	/// </summary>
	void Room_hope_door() {
		portDatas = new DoorStructure[4];
		DoorStructure ds = new DoorStructure();
		int count = 0;
		//Salvando as propriedades da porta de entrada
		foreach (GameObject door in Doors) {
			StructureDoor structure = door.GetComponent<StructureDoor>();
			ds.name = structure.name;
			ds.doorProperties = structure.doorProperties;
			ds.doorPosition = structure.doorPosition;
			ds.anim = structure.anim;
			portDatas[count] = ds;
			count++;
		}
	}
	/// <summary>
	/// Criação da Multiple Forms Room
	/// </summary>
	void RoomTypeObjectsAndShapes() {
		portDatas = new DoorStructure[2];
		DoorStructure ds = new DoorStructure();
		int count = 0;
		//Salvando as propriedades da porta de entrada
		foreach (GameObject door in Doors) {
			StructureDoor structure = door.GetComponent<StructureDoor>();
			ds.name = structure.name;
			ds.doorProperties = structure.doorProperties;
			ds.doorPosition = structure.doorPosition;
			ds.anim = structure.anim;
			portDatas[count] = ds;
			count++;
		}

		CheckFormAnswer.rmanager = this;

		foreach (Answer item in question.answers) {
			if (item.correct)
				CheckFormAnswer.answer = item;
		}
	}


	/// <summary>
	/// Criação da sala verdadeiro e falso
	/// </summary>
	void Room_true_or_false() {
		portDatas = new DoorStructure[2];
		DoorStructure ds = new DoorStructure();
		int count = 0;
		//Salvando as propriedades da porta de entrada
		foreach (GameObject door in Doors) {
			StructureDoor structure = door.GetComponent<StructureDoor>();
			ds.name = structure.name;
			ds.doorProperties = structure.doorProperties;
			ds.doorPosition = structure.doorPosition;
			ds.anim = structure.anim;
			portDatas[count] = ds;
			count++;
		}
	}

	/// <summary>
	/// Criação da End Room
	/// </summary>
	void End_Room() {
		portDatas = new DoorStructure[1];
		DoorStructure ds = new DoorStructure();
		int count = 0;
		//Salvando as propriedades da porta de entrada
		foreach (GameObject door in Doors) {
			StructureDoor structure = door.GetComponent<StructureDoor>();
			ds.name = structure.name;
			ds.doorProperties = structure.doorProperties;
			ds.doorPosition = structure.doorPosition;
			ds.anim = structure.anim;
			portDatas[count] = ds;
			count++;
		}
	}

	#endregion

	/// <summary>
	/// Posiciona a próxima sala junto com o corredor, onde a sala possui apenas uma porta de saída.
	/// </summary>
	/// <param name="nameDoor"></param>
	/// <param name="checkAnswer"></param>
	public void PositionNextRoom(string nameDoor, bool checkAnswer) {
		if (portDatas[1].anim.GetBool("open")) {
			portDatas[1].anim.SetTrigger("closing");
		}
		this.GetComponentInChildren<HubCheckpoint>().clearGoals();
		int doorPosition = portDatas[1].doorPosition;
		int dir = Constants.DIRECTIONS[doorPosition];
		CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);
		StartCoroutine(GameManager.Instance.placeNextCorridor(this.PositionNextCorridor[0].position, this.transform.rotation, dir, corridors[0]));
		portDatas[1].anim.SetTrigger("openning");
		if (this.gameObject.name != "room_0")
			SetDoorAnswer(0);
	}

	/// <summary>
	/// Posiciona a próxima sala Hope door junto com o corredor, onde a sala possui 3 portas de saída.
	/// </summary>
	/// <param name="nameDoor"></param>
	/// <param name="checkAnswer"></param>
	public void PositionNextRoomOfHopeDoor(string nameDoor, bool checkAnswer) {
		if (portDatas[1].anim.GetBool("open")) {
			portDatas[1].anim.SetTrigger("closing");
		}
		this.GetComponentInChildren<HubCheckpoint>().clearGoals();

		int door = 0;
		foreach (DoorStructure item in portDatas) {
			if (item.name == nameDoor) {
				door = item.doorPosition;
			}
		}

		int pathLength = question.paths.Length;

		int dir = Constants.DIRECTIONS[door];
		int path = 0;
		if (pathLength > 1) {

			while ((door == currentPositionCorridor) || (door == 2)) {
				door = UnityEngine.Random.Range(1, 4);
			}
			currentPositionCorridor = door;
			dir = Constants.DIRECTIONS[door];

			path = setNextRoom(checkAnswer);

			Debug.Log("Avaibility chamado é: " + question.paths[path].availability);
			CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);
			StartCoroutine(GameManager.Instance.placeNextCorridor(this.PositionNextCorridor[door].position, this.transform.rotation, dir, corridors[path]));

			PositionNextCorridor[door].gameObject.GetComponentInChildren<Animator>().SetTrigger("openning");

			SetDoorAnswer(path);
		} else {
			CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);

			StartCoroutine(GameManager.Instance.placeNextCorridor(this.PositionNextCorridor[door].position, this.transform.rotation, dir, corridors[path]));
			SetDoorAnswer(path);
			PositionNextCorridor[door].gameObject.GetComponentInChildren<Animator>().SetTrigger("openning");
		}
	}

	int setNextRoom(bool check) {
		for (int i = 0; i < question.paths.Length; i++) {
			if (check && question.paths[i].availability == "right") {
				return i;
			} else if (!check && question.paths[i].availability == "wrong") {
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// Coloca a resposta da pergunta na porta.
	/// </summary>
	/// <param name="path">index do caminho</param>
	public void SetDoorAnswer(int path) {
		int pathLength = question.paths.Length;
		if (question.paths[0].end_game == false) {
			if (pathLength > 1) {
				foreach (RoomManager room in GameManager.Instance.roomsObjects) {
					if (room.id == question.paths[path].connected_question) {
						StartCoroutine(setGateway(room));
						return;
					}
				}
			} else if (question.room_type == "true_or_false") {
				foreach (RoomManager room in GameManager.Instance.roomsObjects) {
					if (room.id == question.paths[path].connected_question) {
						linkDoor linked = room.portDatas[0].doorProperties;

						linked.answerLinked = question.answers[0];
					}
				}
			} else {
				int currentId = question.paths[path].connected_question;
				foreach (RoomManager room in GameManager.Instance.rooms) {
					int id = room.id;
					if (id == currentId) {
						linkDoor linked = room.portDatas[0].doorProperties;

						foreach (Answer ans in question.answers) {
							if (ans.correct) {
								linked.answerLinked = ans;
								return;
							}
						}

					}
				}
			}
		} else {
			if (question.room_type == "true_or_false") {
				RoomManager room = GameManager.Instance.roomsObjects[GameManager.Instance.roomsObjects.Length - 1]; // End Room

				linkDoor linked = room.portDatas[0].doorProperties;

				linked.answerLinked = question.answers[0];
			} else {
				RoomManager room = GameManager.Instance.roomsObjects[GameManager.Instance.roomsObjects.Length - 1]; // End Room

				linkDoor linked = room.portDatas[0].doorProperties;

				foreach (Answer ans in question.answers) {
					if (ans.correct) {
						linked.answerLinked = ans;
					}
				}

			}

		}

		IEnumerator setGateway(RoomManager room) {
			yield return new WaitForSeconds(1f);
			linkDoor linked = room.portDatas[0].doorProperties;
			Debug.Log("A sala que foi pego é a " + room.portDatas[0].name);
			Debug.Log(Inventory.instance.AnswerSelected.answer);
			linked.answerLinked = Inventory.instance.AnswerSelected;
		}
	}
}

/*public interface IRoomSet {
	//void setNextDoor();
	void PositionNextRoom(String nameDoor, bool checkAnswer);
	void generateAnswers();
	void SetTypeRoom();
}*/
