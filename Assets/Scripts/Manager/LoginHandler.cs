using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class LoginHandler : MonoBehaviour {
	public UserInfo user;
	public MazeLDWrapper currentLevel;
	public WebServiceData webAPI;
	WebRoomInfo[] pRooms;
	public WebRoomInfo[] publicRooms { get => pRooms; }
	public bool isValidUser { get {
			if (user == null)
				return false;
			if (user.uid.Equals("-1"))
				return false;
			return true;
		}
	}

#region singleton
	public static LoginHandler handler {
		get {
			if (_instance == null) {
				GameObject go = new GameObject();
				_instance = go.AddComponent<LoginHandler>();
			}
			return _instance;
		}
	}
	static LoginHandler _instance;

	private void Awake() {
		if ((_instance != null) && (_instance != this)) {
			Destroy(this.gameObject);
		} else {
			_instance = this;
			//publicRoomsAsync(-1);
			DontDestroyOnLoad(this.gameObject);
			//this.webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		}
	}
#endregion

#region event pool retrial
	private void loadPool() {
		//load events from disk

		InvokeRepeating("checkAndSendEventInPool", 2f, 5f);
	}

	void checkAndSendEventInPool() {
		if (EventPool.pool.Count > 0) {
			//Events works like LIFO
			EventInfo e = EventPool.pool[EventPool.pool.Count-1];
			EventPool.pool.RemoveAt(EventPool.pool.Count - 1);
			EventPool.sendEvent(e);
		}
	}
	#endregion

	private void OnDestroy() {
		CancelInvoke("checkAndSendEventInPool");
	}

	#region login
	public delegate void LoginCompleted(UserInfo user);
	public static LoginCompleted OnLoginCompleted;

	public async Task<UserInfo> loginAsync(string username, string pwd) {
		Debug.Log(string.Format("Login requested for {0} - {1}", new object[] { username, pwd }));
		UserInfo u = new UserInfo(null, "-1");
		this.user = null;
		string url = webAPI.loginURL + "?email="+username+"&password="+pwd;

		using (UnityWebRequest uwr = UnityWebRequest.Post(url, "")) {
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.downloadHandler = new DownloadHandlerBuffer();
			await uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError) {
				Debug.LogWarning(string.Format("Could not login {0} - due to: {1}", new object[] { username, uwr.error }));
				u.username = "Could not connect.";
			} else {
				string data = Encoding.UTF8.GetString(uwr.downloadHandler.data);
				LoginReturn lr = JsonUtility.FromJson<LoginReturn>(data);
				if (lr.id.Equals("-1")) {
					u.username = "Not a valid user.";
				} else {
					u = new UserInfo(lr.name, lr.id);
					this.user = u;
				}

			}
			if (OnLoginCompleted != null) {
				OnLoginCompleted(u);
			}
			return u;
		}
	}

	struct LoginReturn {
		public string id;
		public string name;
	}
#endregion

#region rooms
	public delegate void RoomRequestCompleted(WebRoomInfo[] rooms);
	public static RoomRequestCompleted OnPublicRoomRequestCompleted;
	public static RoomRequestCompleted OnPrivateRoomRequestCompleted;

	public async Task<WebRoomInfo[]> publicRoomsAsync(int uid = -2) {
		if (uid == -2) {
			if (this.user != null) {
				uid = int.Parse(this.user.uid);
			}
		}
		if (uid == -1) {
			uid = -2;
		}
		//this.pRooms = null;
		this.pRooms = await getRoomsAsync(uid, 0);
		OnPublicRoomRequestCompleted?.Invoke(this.pRooms);
		return this.pRooms;
	}

	public async Task<WebRoomInfo[]> privateRoomsAsync(int uid = -2) {
		if (uid == -2) {
			if (this.user != null) {
				uid = int.Parse(this.user.uid);
			}
		}
		if (uid == -1) {
			uid = -2;
		}
		this.user.privateRooms = null;
		this.user.privateRooms = await getRoomsAsync(uid, 1);
		OnPrivateRoomRequestCompleted?.Invoke(this.user.privateRooms);
		return this.user.privateRooms;
	}

	async Task<WebRoomInfo[]> getRoomsAsync(int uid, int type) {
		Debug.Log(string.Format("{1} Rooms requested for {0}", new object[] { uid, typeToNameType(type) }));
		WebRoomInfo[] rooms = null;
		string url = webAPI.getRoomURL+"?type=" + type.ToString()+"&id=" + uid.ToString();

		Debug.Log(url);
		using (UnityWebRequest uwr = UnityWebRequest.Post(url, "")) {
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.downloadHandler = new DownloadHandlerBuffer();
			await uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError) {
				Debug.LogWarning(string.Format("Could not get {1} rooms from {0} - due to: {2}", new object[] { uid, typeToNameType(type), uwr.error }));
				user.username = "Could not connect.";
			} else {
				string data = Encoding.UTF8.GetString(uwr.downloadHandler.data);
				RoomWrapper dr = JsonUtility.FromJson<RoomWrapper>(data);
				if (dr.success == -1) {
					Debug.LogWarning(string.Format("Could not get {1} rooms from {0} - due to: 'Invalid user'", new object[] { uid, typeToNameType(type) }));
					rooms = null;
				} else {
					//Debug.Log(string.Format("Got {2} {1} rooms from {0}", new object[] { uid, typeToNameType(type), dr.salas.Length }));
					rooms = dr.salas;
				}

			}
			return rooms;
		}
	}

	string typeToNameType(int type) {
		return (type == 1 ? "Private" : "Public");
	}

	class RoomWrapper {
		public WebRoomInfo[] salas;
		public int success;
	}

	#endregion

	#region maze Json
	public delegate void MazeJsonCompleted(MazeLDWrapper ld);
	public static MazeJsonCompleted OnMazeJsonCompleted;

	public async Task<MazeLDWrapper> getMazeAsync(int maze_id) {
		Debug.Log(string.Format("Requesting maze: {0}", new object[] { maze_id }));
		string url = webAPI.getMazeURL+"?id=" + maze_id;

		MazeLDWrapper ld;
		using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.downloadHandler = new DownloadHandlerBuffer();
			await uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError) {
				Debug.LogWarning(string.Format("Could not get json for {0} - due to: {1}", new object[] { maze_id, uwr.error }));
				user.username = "Could not connect.";
				ld = null;
			} else {
				string data = Encoding.UTF8.GetString(uwr.downloadHandler.data);
				bool error = (data.IndexOf("\"success\":-1") != -1);
				
				if (error) {
					Debug.LogWarning(string.Format("Could not get json for {0} - due to: {1}", new object[] { maze_id, data }));
					ld = null;
				} else {
					ld = JsonUtility.FromJson<MazeLDWrapper>(data);
				}

			}
			this.currentLevel = ld;
			return ld;
		}
	}

	#endregion

}


public class UserInfo {
	public string username;
	public string uid;
	public WebRoomInfo[] privateRooms = null;

	public UserInfo(string username, string uid) {
		this.username = username;
		this.uid = uid;
	}

	public override string ToString() {
		return string.Format("user: {0}.{1}", new object[] { uid, username });
	}
}

[System.Serializable]
public class WebRoomInfo {
	public int id;
	public string name;
	public int Pergunta;
	public int Reforco;
	public string photoURL;
	public Texture2D photo;

	public void setPhoto(string photoURL) {
		this.photoURL = photoURL;
		loadImage();
	}

	async Task<Texture2D> loadImage() {

		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(this.photoURL)) {
			await uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError) {
				Debug.LogError("Could not download room file "+this.photoURL+" due to: " + uwr.error);
			} else {
				this.photo = DownloadHandlerTexture.GetContent(uwr).ToTexture2D();
				return this.photo;
			}
		}
		return null;
			
	}
	public override string ToString() {
		return string.Format("{0}.{1} - {2}, {3}", new object[] { id, name, Pergunta-Reforco, Reforco });
	}
}
