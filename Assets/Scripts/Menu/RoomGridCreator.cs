using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RoomGridCreator : MonoBehaviour
{
	public CubeButton srcButton;
	public GameObject loading;
	public GameObject grid;
	public GameObject gridElementPrefab;
	public GameObject toggleButton;
	public GameObject loadedRoom;
	public bool privateData = true;
	public bool loaded = false;
	int levelHasBeenLoaded = -1;
	string gameScene = "VR_Level_Base";
	public List<WebRoomInfo> salas;
	int pageSize = 5;
	int pageID = 0;
	int filter = 0;
	public int page {
		get => pageID;
		set {
			this.pageID = value;
			this.refreshPage();
		}
	}
	int pages = 1;

	public Button pageUpBtn;
	public Button pageDownBtn;

	private void FixedUpdate() {
		//data load
		//if (privateData && (LoginHandler.handler.user.privateRooms != null)) {
		//	if (LoginHandler.handler.user.privateRooms.Length != grid.transform.childCount) {
		//		loaded = false;
		//	}

		//} else if (!privateData && (LoginHandler.handler.publicRooms != null)) {
		//	if (LoginHandler.handler.publicRooms.Length != grid.transform.childCount) {
		//		this.loaded = false;
		//	}
		//}
		if (!this.loaded) {
			this.updateRoomList();
			this.refreshPage();
		}

		//level load finished
		if (levelHasBeenLoaded > 0) {
			if (levelHasBeenLoaded == 1) {
				if (srcButton != null) {
					srcButton.afterCanvasOK();
				} else {
					if (DataManager.manager.savegame.timeElapsed > 0) {
						// perguntar se carrega o progresso
						this.GetComponentInParent<Animator>().SetTrigger("LoadProgress");
					} else {
						SceneManager.LoadScene(gameScene);
					}
				}
				levelHasBeenLoaded = -1;
			} else {
				levelHasBeenLoaded = -1;
				foreach (Button btn in this.transform.GetComponentsInChildren<Button>()) {
					btn.enabled = false;
				}
			}
		}
	}

	public void loadWithProgress() {
		SceneManager.LoadScene(gameScene);
	}

	public void loadWithoutProgress() {
		DataManager.manager.cleanPlayerProgress();
		SceneManager.LoadScene(gameScene);
	}

	void refreshPage() {
		this.clearGridChildren();
		int roomCount = 0;
		if (this.salas.Count == 0) {
			this.loading.SetActive(true);
		} else {
			this.loading.SetActive(false);

			IOrderedEnumerable<WebRoomInfo> orderedRooms;
			switch (this.filter) {
				case 1: //salas públicas 
					orderedRooms = (from room in this.salas
									where room.isPublic
									orderby room.progressOrder ascending
									select room);
					break;
				case 2: //salas privadas 
					orderedRooms = (from room in this.salas
									where !room.isPublic
									orderby room.progressOrder ascending
									select room);
					break;
				case 3: //salas novas 
					orderedRooms = (from room in this.salas
									where room.progress == 0
									orderby room.progressOrder ascending
									select room);
					break;
				case 4: //salas iniciadas
					orderedRooms = (from room in this.salas
									where room.progress == 1
									orderby room.progressOrder ascending
									select room);
					break;
				case 5: //salas concluídas 
					orderedRooms = (from room in this.salas
									where room.progress == 2
									orderby room.progressOrder ascending
									select room);
					break;
				case 0: //sem filtro
				default:
					orderedRooms = (from room in this.salas
									orderby room.isPublic, room.progressOrder ascending
									select room);
					break;
			}
			roomCount = orderedRooms.Count();
			createGrid(orderedRooms
									.Skip(this.pageID * this.pageSize)
									.Take(this.pageSize)
									.ToArray());
		}
		this.pages = Mathf.FloorToInt((roomCount-1) / this.pageSize);
		Debug.LogFormat("RoomGridCreator::updateRoomList - salas: {0}, páginas: {1} com {2} itens.", new object[] { roomCount, this.pages, this.pageSize });

		this.pageUpBtn.interactable = (this.pageID < this.pages);
		this.pageDownBtn.interactable = !(pageID <= 0);
	}

	public void setFilter(GameObject trigger) {
		this.filter = trigger.GetComponent<TMP_Dropdown>().value;
		this.refreshPage();
	}

	void createGrid(WebRoomInfo[] rooms) {
		this.loaded = true;
		this.loading.SetActive(false);
		foreach (WebRoomInfo r in rooms) {
			RoomGridDisplay rgd = Instantiate(gridElementPrefab, this.grid.transform).GetComponent<RoomGridDisplay>();
			rgd.roomInfo = r;
		}
		this.grid.SetActive(true);
	}

	void updateRoomList() {
		this.salas = new List<WebRoomInfo>();
		if (LoginHandler.handler.publicRooms != null) {
			this.salas.AddRange(LoginHandler.handler.publicRooms);
		}
		if (LoginHandler.handler.isValidUser && (LoginHandler.handler.user.privateRooms != null)) {
			this.salas.AddRange(LoginHandler.handler.user.privateRooms);
		}
	}

	public void changeScope() {
		this.privateData = !this.privateData;
		this.clearGridChildren();
		this.requestData();
	}

	void OnGetData(WebRoomInfo[] rooms) {
		//data will arrive next frame
		Debug.Log("New data loaded! Room count: "+rooms.Length);
		this.loaded = false;
		this.updateRoomList();
		this.refreshPage();
	}

	private void OnEnable() {
		LoginHandler.OnPrivateRoomRequestCompleted += OnGetData;
		LoginHandler.OnPublicRoomRequestCompleted += OnGetData;

		this.privateData = LoginHandler.handler.isValidUser;
		this.pageID = 0;
		//this.clearGridChildren();
		this.updateRoomList();
		this.requestData();

		if (this.loadedRoom != null) {
			if (DataManager.manager.mazeLD != null) {
				WebRoomInfo room = new WebRoomInfo();
				room.id = DataManager.manager.mazeLD.maze_id;
				room.name = DataManager.manager.mazeLD.maze_name;
				room.Pergunta = 999;
				room.Reforco = 333;
				this.loadedRoom.GetComponent<RoomGridDisplay>().roomInfo = room;
			} else {
				this.loadedRoom.GetComponent<RoomGridDisplay>().roomInfo = null;
			}
		}
		this.refreshPage();
	}

	public void pageUp() {
		this.pageID++;
		this.refreshPage();
	}

	public void pageDown() {
		this.pageID--;
		this.refreshPage();
	}

	void requestData() {
		_ = LoginHandler.handler.publicRoomsAsync();
		if (LoginHandler.handler.isValidUser) {
			_ = LoginHandler.handler.privateRoomsAsync();
		}
	}

	void clearGridChildren() {
		this.loading.SetActive(true);
		for (int i = grid.transform.childCount - 1; i >= 0; i--) {
			Destroy(grid.transform.GetChild(i).gameObject);
		}
		this.grid.SetActive(false);
	}

	private void OnDisable() {
		LoginHandler.OnPrivateRoomRequestCompleted -= OnGetData;
		LoginHandler.OnPublicRoomRequestCompleted -= OnGetData;
		this.clearGridChildren();
	}

	public void cancel() {
		if (srcButton != null) {
			srcButton.afterCanvasCancel();
		} else {
			this.GetComponentInParent<Animator>().SetTrigger("Back");
		}
	}

	/// <summary>
	/// Sala a jogar foi selecionada e está carregando o jogo.
	/// </summary>
	/// <param name="room">sala a ser jogada</param>
	/// <param name="is_loaded">dados já está na memória? (continuando ou qr code)</param>
	public void loadGame(WebRoomInfo room, bool is_loaded = false) {
		//desabilita os botões enquanto não resolve
		foreach (Button btn in this.transform.GetComponentsInChildren<Button>()) {
			btn.enabled = false;
		}

		if (is_loaded) {
			//SceneManager.LoadScene(this.gameScene);
			this.levelHasBeenLoaded = 1;
		} else {

			this.loading.SetActive(true);
			this.grid.SetActive(false);
			if (this.toggleButton != null) {
				this.toggleButton.SetActive(false);
			}
			loadRoom(room);
		}
	}

	async void loadRoom(WebRoomInfo room) {
		//get json
		string level = "";
		MazeLDWrapper ld = await LoginHandler.handler.getMazeAsync(room.id);

		if (ld == null) {
			this.levelHasBeenLoaded = 2; //error
			return;
		}
		//set level
		DataManager.manager.setNewLevel(ld);

		//level set, loading progress.
		bool progress = await LoginHandler.handler.getMazeProgress(ld.maze_id);

		this.levelHasBeenLoaded = 1; //OK
	}
}
