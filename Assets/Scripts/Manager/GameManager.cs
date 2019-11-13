using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

[RequireComponent(typeof(NavMeshBaker))]
public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	[Header("Rooms")]
	public TextAsset levelDesign;
	MazeLDWrapper mazeLD;

	public string tagRoom;
	public Transform mapRoot;
	public GameObject startingRoom;
	//public RoomDescriptor startingRoom;
	public GameObject endRoom; // Finish Game

	/// <summary>
	/// Todos os Tipos da Sala 0 - right_key, 1 - hope_door, 2 - multiple_forms, 3 - true_or_false
	/// </summary>
	[Tooltip("Todos os Tipos da Sala 0 - right_key, 1 - hope_door, 2 - multiple_forms, 3 - true_or_false.")]
	public GameObject[] roomPrefabs;

	public RoomManager[] roomsObjects;
	public int cellSize = 1;

	public List<RoomManager> rooms;
	public List<CorridorManager> corridors;
	public GameObject basicTilePrefab;
	public GameObject hubPrefab;

	public RoomManager currentRoom;
	public RoomManager nextRoom;
	CorridorManager currentCorridor;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	private void Start() {
		if (DataManager.manager.mazeLD != null) {
			Debug.Log("Loaded QR Code level.");
			mazeLD = DataManager.manager.mazeLD;
		} else {
			if (levelDesign == null) {
				Debug.LogError("Cannot play without a level.");
			}
			mazeLD = readData(levelDesign.text);
			DataManager.manager.setNewLevel(mazeLD);
		}

		CreateRooms();
		CreateCorridors();
		positionCorrectRoomAndPlayer();
	}

	void positionCorrectRoomAndPlayer() {
		int croom = 0; // Sala Inicial

		int currentRoomID = DataManager.manager.savegame.currentRoomID;

		if (currentRoomID >= 0 && currentRoomID != mazeLD.starting_question_id) {
			croom = DataManager.manager.savegame.currentRoomID;
		} else { //Start Room
			startingRoom.GetComponent<RoomManager>().PositionNextRoom("DoorAnswer", true);
			foreach (RoomManager room in roomsObjects) {
				if (room.id == currentRoomID) {
					room.portDatas[0].anim.SetTrigger("openning");
				}
			}
		}

		RoomManager r = getRoom(croom);
		if (this.currentRoom != null) {
			if (croom != r.id) {
				this.currentRoom.gameObject.SetActive(false);
			}
		}
		this.currentRoom = r;
		this.currentRoom.gameObject.SetActive(true);

		GameObject player = GameObject.FindGameObjectWithTag("PlayerAgent");
		player.transform.position = r.GetComponentInChildren<HubCheckpoint>().transform.position + Vector3.up;

	}

	public RoomManager getRoom(int id) {
		return this.rooms.Find(x => (x.id == id));
	}

	public CorridorManager[] getCorridorsByRoom(int question_id) {
		return this.corridors.FindAll(x => (x.question_id == question_id)).ToArray();
	}

	public CorridorManager[] getCorridorsByRoomAndType(int question_id, string room_type) {
		return this.corridors.FindAll(x => ((x.question_id == question_id) && (x.pathInfo.type.Equals(room_type)))).ToArray();
	}

	public CorridorManager[] getCorridorsByRoomAndAvailbility(int question_id, string availbility) {
		return this.corridors.FindAll(x => ((x.question_id == question_id) && (x.pathInfo.availability.Equals(availbility)))).ToArray();
	}

	public IEnumerator placeNextCorridor(Vector3 position, Quaternion baseRot, int direction, CorridorManager corridor) {
		Vector3 nextCorrPivot = position;
		int delta = (Mathf.RoundToInt(baseRot.eulerAngles.y / 90) + 4) % 4;
		int true_dir = (Tools.directionToIndex(direction) + delta) % 4;

		//se já tem corredor, desabilita
		// if (this.currentCorridor != null) {
		//     this.currentCorridor.gameObject.SetActive(false);
		// }
		this.currentCorridor = corridor;

		Debug.Log(currentCorridor.pathInfo.connected_question);
		//TODO: condicao de endgame.

		if (this.currentCorridor == null) {
			Debug.LogError(string.Format("Cannot allocate inexistent corridor. Room-{0}", new object[] { currentRoom.id }));
			yield break;
		}
		// Debug.Log("generating corridor in direction: "+direction+" index: "+Tools.directionToIndex(direction));
		// coloca o novo corredor em posição e rotação e espera até o final do frame pra continuar devido a problemas de render/update
		float rot = Constants.ROTATIONS[true_dir];
		MapCoord d = Constants.DELTA[true_dir];
		Vector3 fwd = new Vector3(d.x, 0f, d.y);
		d = Constants.DELTA[(true_dir + 1) % 4];
		Vector3 right = new Vector3(d.x, 0f, d.y);
		float corrEntx = (corridor.GetComponent<CorridorGenerator>().entrance.x + 0.5f) * corridor.cellSize;
		this.currentCorridor.transform.rotation = Quaternion.Euler(0f, rot + baseRot.y, 0f);
		this.currentCorridor.transform.position = nextCorrPivot - right * corrEntx;
		this.currentCorridor.gameObject.SetActive(true);
		yield return new WaitForEndOfFrame();

		if (!currentCorridor.pathInfo.end_game) {
			nextRoom = getRoom(currentCorridor.pathInfo.connected_question);
			if (nextRoom == null) {
				Debug.LogError(string.Format("Room {0} does not exist on path of type {1} from room {2}.", new object[] { currentCorridor.pathInfo.connected_question, currentCorridor.pathInfo.type, currentRoom.id }));
				yield break;
			}
			//add delta to finishing position in corridor
			Transform exitHub = corridor.GetComponent<CorridorGenerator>().getExitTranform();

			float corrEndx = (corridor.GetComponent<CorridorGenerator>().exit.x + 0.5f) * corridor.cellSize;
			nextCorrPivot += (fwd * (corridor.pathInfo.height * corridor.cellSize) + right * corrEndx);

			nextCorrPivot = exitHub.transform.position + exitHub.forward * cellSize / 2f;
			nextCorrPivot.y -= 1.5f;

			nextRoom.transform.rotation = Quaternion.Euler(0f, rot + baseRot.y, 0f);
			//nextRoom.transform.position = nextCorrPivot + Vector3.forward * currentCorridor.pathInfo.height + Vector3.forward * nextRoom.GetComponent<RoomDescriptor>().size.y;
			nextRoom.transform.position = nextCorrPivot + fwd * nextRoom.GetComponent<RoomDescriptor>().size.y - right * nextRoom.GetComponent<RoomDescriptor>().size.x / 2f; // - nextRoom.spawnDoor[0].transform.localPosition;

			//change listener
			currentRoom.GetComponentInChildren<HubCheckpoint>().onPlayerEnter -= onEnteredNextRoom;
			nextRoom.GetComponentInChildren<HubCheckpoint>().onPlayerEnter += onEnteredNextRoom;

			yield return new WaitForEndOfFrame();
			CorridorGenerator ccGen = currentCorridor.GetComponent<CorridorGenerator>();
			currentRoom.GetComponentInChildren<HubCheckpoint>().setGoal(direction, ccGen.getEntranceTranform());
			ccGen.setEntranceMotion(currentRoom.GetComponentInChildren<HubCheckpoint>().transform);
			//ccGen.setExitMotion (nextRoom.GetComponentInChildren<HubCheckpoint> ().transform);
			this.nextRoom.gameObject.SetActive(true);
		} else // END GAME
			{
			nextRoom = endRoom.GetComponent<RoomManager>();
			if (nextRoom == null) {
				Debug.LogError(string.Format("Sala final não existe."));
				yield break;
			}
			//add delta to finishing position in corridor
			Transform exitHub = corridor.GetComponent<CorridorGenerator>().getExitTranform();

			float corrEndx = (corridor.GetComponent<CorridorGenerator>().exit.x + 0.5f) * corridor.cellSize;
			nextCorrPivot += (fwd * (corridor.pathInfo.height * corridor.cellSize) + right * corrEndx);

			nextCorrPivot = exitHub.transform.position + exitHub.forward * cellSize / 2f;
			nextCorrPivot.y -= 1.5f;

			nextRoom.transform.rotation = Quaternion.Euler(0f, rot + baseRot.y, 0f);
			//nextRoom.transform.position = nextCorrPivot + Vector3.forward * currentCorridor.pathInfo.height + Vector3.forward * nextRoom.GetComponent<RoomDescriptor>().size.y;
			nextRoom.transform.position = nextCorrPivot + fwd * nextRoom.GetComponent<RoomDescriptor>().size.y - right * nextRoom.GetComponent<RoomDescriptor>().size.x / 2f; // - nextRoom.spawnDoor[0].transform.localPosition;

			//change listener
			currentRoom.GetComponentInChildren<HubCheckpoint>().onPlayerEnter -= onEnteredNextRoom;
			nextRoom.GetComponentInChildren<HubCheckpoint>().onPlayerEnter += onEnteredNextRoom;

			yield return new WaitForEndOfFrame();
			CorridorGenerator ccGen = currentCorridor.GetComponent<CorridorGenerator>();
			currentRoom.GetComponentInChildren<HubCheckpoint>().setGoal(direction, ccGen.getEntranceTranform());
			ccGen.setEntranceMotion(currentRoom.GetComponentInChildren<HubCheckpoint>().transform);
			//ccGen.setExitMotion (nextRoom.GetComponentInChildren<HubCheckpoint> ().transform);
			this.nextRoom.gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(0.1f);
		this.GetComponent<NavMeshBaker>().CreateBake();
		this.currentRoom.GetComponentInChildren<HubCheckpoint>().activate();
	}

	IEnumerator placeContinuation() {
		Vector3 nextCorrPivot = currentRoom.GetComponent<RoomDescriptor>().topLeft;
		Vector2 roomSize = currentRoom.GetComponent<RoomDescriptor>().size;

		CorridorManager[] corridors = getCorridorsByRoom(currentRoom.id);
		CorridorManager corr = null;
		if (corridors.Length > 0) {
			corr = corridors[0];
		} else {
			Debug.Log("Must have reached the endgame.");
			yield break;
		}
		yield return placeNextCorridor(nextCorrPivot + Vector3.right * roomSize.x / 2f, this.transform.rotation, Constants.DIRECTION_UP, corr);
	}


	/// <summary>
	/// Salva ao entrar na sala
	/// </summary>
	/// <param name="hub">Ponto de referência do caminho.</param>
	void onEnteredNextRoom(HubCheckpoint hub) {
		if (this.currentRoom.GetComponentInChildren<HubCheckpoint>() == hub)
			return;
		this.currentRoom.gameObject.SetActive(false);
		this.currentCorridor.gameObject.SetActive(false);
		this.currentRoom = this.nextRoom;
		DataManager.manager.savegame.currentRoomID = this.currentRoom.id;
		//limpa as setas da sala, pro caso de já ter passado aqui (volta do reforço)


		// this.currentRoom.gameObject.SetActive (true);
		if (this.currentRoom.id == -42) {
			// id fixo para o endgame
			EventPool.sendMazeEndEvent();
			DataManager.manager.saveProgress(); // Salvar no arquivo local

			// uhuuuu!!!
			// parabéns!!!
		} else {
			EventPool.sendQuestionStartEvent();
			DataManager.manager.saveProgress();
		}
	}

	/// <summary>
	/// Cria todas as salas que serão utilizadas dentro do jogo.
	/// </summary>
	public void CreateRooms() {
		this.rooms = new List<RoomManager>();
		this.corridors = new List<CorridorManager>();
		this.roomsObjects = new RoomManager[mazeLD.questions.Length + 2]; // + Add Starting Room e End Room

		//Start Room
		startingRoom.SetActive(true);
		startingRoom.name = "room_0";
		startingRoom.GetComponent<RoomManager>().SetTypeRoom();
		startingRoom.GetComponent<RoomManager>().manager = this;
		roomsObjects[0] = startingRoom.GetComponent<RoomManager>();
		startingRoom.GetComponent<RoomManager>().question.paths[0].connected_question = mazeLD.starting_question_id; // Put connection
		this.rooms.Add(startingRoom.GetComponent<RoomManager>());

		int i = 1;
		// Other rooms
		foreach (Question quest in mazeLD.questions) {
			GameObject go = null;
			if (quest.room_type == "right_key")
				go = Instantiate(roomPrefabs[0], new Vector3(0f, -100f - (this.rooms.Count * 2f), 0f), Quaternion.identity, mapRoot);
			else if (quest.room_type == "hope_door")
				go = Instantiate(roomPrefabs[1], new Vector3(0f, -100f - (this.rooms.Count * 2f), 0f), Quaternion.identity, mapRoot);
			else if (quest.room_type == "multiple_forms")
				go = Instantiate(roomPrefabs[2], new Vector3(0f, -100f - (this.rooms.Count * 2f), 0f), Quaternion.identity, mapRoot);
			else if (quest.room_type == "true_or_false")
				go = Instantiate(roomPrefabs[3], new Vector3(0f, -100f - (this.rooms.Count * 2f), 0f), Quaternion.identity, mapRoot);

			if (go != null) {

				go.name = "room_" + quest.question_id;
				RoomManager rm = go.GetComponent<RoomManager>();
				if (quest.question_id == mazeLD.starting_question_id) {
					currentRoom = rm;
				} else {
					go.SetActive(false);
				}
				rm.question = quest;
				rm.generateAnswers();
				rm.manager = this;
				roomsObjects[i] = go.GetComponent<RoomManager>();
				i++;

				this.rooms.Add(rm);
			}
		}
		// yield break;

		//End Room
		endRoom.SetActive(false);
		endRoom.name = "room_-42";
		endRoom.GetComponent<RoomManager>().SetTypeRoom(); // Criação da sala End Room
		endRoom.GetComponent<RoomManager>().manager = this;
		roomsObjects[roomsObjects.Length - 1] = endRoom.GetComponent<RoomManager>();
		this.rooms.Add(endRoom.GetComponent<RoomManager>());
	}

	public void CreateCorridors() {
		foreach (RoomManager room in this.rooms) {
			float sameRoomXPad = 1.5f;

			foreach (MazePath path in room.question.paths) {
				GameObject go = new GameObject();
				go.transform.parent = mapRoot;
				go.transform.position = room.transform.position + Vector3.right * sameRoomXPad; //+ Vector3.forward * 1.5f
				sameRoomXPad += path.width * 1.5f;
				go.name = "corridor_" + room.id + "_" + path.type;

				CorridorManager man = go.AddComponent<CorridorManager>();
				man.cellSize = this.cellSize;
				man.question_id = room.id;
				man.roomEntrance = room.GetComponentInChildren<HubCheckpoint>().transform;
				man.pathInfo = path;
				if (path.connected_question != -1) {
					man.roomExit = getRoom(path.connected_question).GetComponentInChildren<HubCheckpoint>().transform;
				}

				man.generateCorridor(this.basicTilePrefab, this.hubPrefab);
				// yield return new WaitForSeconds (0.1f);
				man.gameObject.SetActive(false);
				this.corridors.Add(man);
			}
		}
	}

	public void setExitMotionInHub() {
		currentCorridor.generator.setExitMotion(nextRoom.gameObject.GetComponentInChildren<HubCheckpoint>().transform);
	}

	public MazeLDWrapper readData(string json) {
		return JsonUtility.FromJson<MazeLDWrapper>(json);
	}
}
