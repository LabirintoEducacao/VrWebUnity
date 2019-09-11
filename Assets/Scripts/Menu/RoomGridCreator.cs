using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
	public string gameScene;
	public int levelHasBeenLoaded = -1;

	private void FixedUpdate() {
		//data load
		if (privateData && (LoginHandler.handler.user.privateRooms != null)) {
			if (LoginHandler.handler.user.privateRooms.Length != grid.transform.childCount) {
				loaded = false;
			}

		} else if (!privateData && (LoginHandler.handler.publicRooms != null)) {
			if (LoginHandler.handler.publicRooms.Length != grid.transform.childCount) {
				this.loaded = false;
			}
		}
		if (!loaded) {
			if (privateData) {
				if (LoginHandler.handler.user.privateRooms != null) {
					this.clearGridChildren();
					createGrid(LoginHandler.handler.user.privateRooms);
				}
			} else {
				if (LoginHandler.handler.publicRooms != null) {
					this.clearGridChildren();
					createGrid(LoginHandler.handler.publicRooms);
				}
			}
		}

		//level load finished
		if (levelHasBeenLoaded > 0) {
			if (levelHasBeenLoaded == 1) {
				SceneManager.LoadScene(gameScene);
			} else {
				levelHasBeenLoaded = -1;
				foreach (Button btn in this.transform.GetComponentsInChildren<Button>()) {
					btn.enabled = false;
				}
			}
		}
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

	public void changeScope() {
		this.privateData = !this.privateData;
		this.clearGridChildren();
		this.requestData();
	}

	void OnGetData(WebRoomInfo[] rooms) {
		//data will arrive next frame
		Debug.Log("New data loaded! Room count: "+rooms.Length);
		this.loaded = false;

		LoginHandler.OnPrivateRoomRequestCompleted -= OnGetData;
		LoginHandler.OnPublicRoomRequestCompleted -= OnGetData;
	}

	private void OnEnable() {
		if (LoginHandler.handler.isValidUser) {
			this.privateData = true;
		} else {
			this.privateData = false;
		}
		this.clearGridChildren();
		this.requestData();
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

	void requestData() {
		LoginHandler.OnPrivateRoomRequestCompleted -= OnGetData;
		LoginHandler.OnPublicRoomRequestCompleted -= OnGetData;
		if (this.privateData) {
			LoginHandler.OnPrivateRoomRequestCompleted += OnGetData;
			LoginHandler.handler.privateRoomsAsync();
		} else {
			LoginHandler.OnPublicRoomRequestCompleted += OnGetData;
			LoginHandler.handler.publicRoomsAsync();
		}
	}

	void clearGridChildren() {
		this.loaded = false;
		this.loading.SetActive(true);
		for (int i = grid.transform.childCount - 1; i >= 0; i--) {
			Destroy(grid.transform.GetChild(i).gameObject);
		}
		this.grid.SetActive(false);
	}

	private void OnDisable() {
		this.clearGridChildren();
	}

	public void cancel() {
		srcButton.afterCanvasCancel();
	}

	public void loadGame(WebRoomInfo room, bool is_loaded = false) {
		foreach (Button btn in this.transform.GetComponentsInChildren<Button>()) {
			btn.enabled = false;
		}
		if (is_loaded) {
			SceneManager.LoadScene(this.gameScene);
		} else {

			this.loading.SetActive(true);
			this.grid.SetActive(false);
			this.toggleButton.SetActive(false);
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
		this.levelHasBeenLoaded = 1; //OK
		DataManager.manager.setNewLevel(ld);
	}
}
