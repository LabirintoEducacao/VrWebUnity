using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoginHandler))]
public class LoginHandlerEditor : Editor {
	public UserInfo user;
	public string email;
	public string pwd;
	public int maze_id;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		LoginHandler login = target as LoginHandler;
		GUILayout.Space(20f);
		GUILayout.Label("-------- LOGIN ----------");
		this.email = EditorGUILayout.TextField("E-mail", email);
		this.pwd = EditorGUILayout.TextField("Password", pwd);

		if (GUILayout.Button("Login")) {
			LoginHandler.OnLoginCompleted -= OnLoginComplete;
			LoginHandler.OnLoginCompleted += OnLoginComplete;

			login.loginAsync(email, pwd);
		}
		if (user != null) {
			GUILayout.Label(user.ToString());
		}
		GUILayout.Label("-------- SALAS ----------");

		if ((user != null) && (!user.uid.Equals("-1"))) {
			if (GUILayout.Button("Private Rooms")) {
				login.privateRoomsAsync(int.Parse(this.user.uid));
			}
			if (this.user.privateRooms != null) {
				if (this.user.privateRooms.Length == 0) {
					GUILayout.Label("No Rooms");
				} else {
					for (int i = 0; i < this.user.privateRooms.Length; i++) {
						WebRoomInfo r = this.user.privateRooms[i];
						GUILayout.Label(r.ToString());
					}
				}
			}
		}
		if (GUILayout.Button("Public Rooms")) {
			login.publicRoomsAsync(-1);
		}
		if (login.publicRooms != null) {
			if (login.publicRooms.Length == 0) {
				GUILayout.Label("No Rooms");
			} else {
				for (int i = 0; i < login.publicRooms.Length; i++) {
					WebRoomInfo r = login.publicRooms[i];
					GUILayout.Label(r.ToString());
				}
			}
		}
		GUILayout.Label("-------- JSON ----------");
		maze_id = EditorGUILayout.IntField("Maze ID", maze_id);
		if (GUILayout.Button("Get Maze Data")) {
			login.getMazeAsync(maze_id);
		}
		if (login.currentLevel != null) {
			GUILayout.Label(login.currentLevel.ToString());
		}

	}

	void OnLoginComplete(UserInfo user) {
		this.user = user;
		LoginHandler.OnLoginCompleted -= OnLoginComplete;
	}
}
