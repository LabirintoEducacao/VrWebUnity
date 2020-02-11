﻿using larcom.support;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour {
	#region singleton
	static DataManager instance;
	public static DataManager manager {
		get {
			if (instance == null) {
				GameObject go = new GameObject ();
				go.name = "DataManager";
				return go.AddComponent<DataManager>();
			} else {
				return instance;
			}
		}
	}
	SaveGameData svgd;
	public SaveGameData savegame {
		get {
			return svgd;
		}
	}

	void Awake() {
		if (instance != null) {
			if (instance != this) {
				Destroy(this.gameObject);
			}
		} else {

			instance = this;
			SceneManager.activeSceneChanged += SceneChanged;
			DontDestroyOnLoad(this.gameObject);
#if !UNITY_EDITOR
			if (this.mazeLD == null) {
				this.mazeLD = SaveData.load<MazeLDWrapper>("current_level");
			}
			this.checkAndCreateSave();
#else
			svgd = new SaveGameData();
            svgd.playing = !SceneManager.GetActiveScene().name.Equals("MainMenu_v2");
#endif
			//reload unsent event pool
			EventPoolWrapper ew = SaveData.load<EventPoolWrapper>("event_pool");
			if (ew == null) {
				EventPool.pool = new List<EventInfo>();
			} else {
				EventPool.pool = new List<EventInfo>(ew.pool);
			}
		}
	}
	#endregion

	public bool vrMode { get; private set; } = true;

	public MazeLDWrapper mazeLD { get; set; } = null;

	public void setNewLevel(string levelDesign) {
		MazeLDWrapper tempLD = JsonUtility.FromJson<MazeLDWrapper>(levelDesign);
		if ((mazeLD != null) && (tempLD.maze_id == mazeLD.maze_id)) {
			// do nothing - level already loaded
			return;
		}
		this.setNewLevel(tempLD);
	}
	public void setNewLevel(MazeLDWrapper maze) {
		mazeLD = maze;
		SaveData.save("current_level", JsonUtility.ToJson(mazeLD));

		checkAndCreateSave();
	}

	void checkAndCreateSave() {
		if (mazeLD != null) {
			// se não tem save, ou é outro labirinto, reseta os dados, caso contrário continua com o que tem.
			if (svgd != null) {
				createNewSave();
			} else if ((svgd == null) || (svgd.mazeID != mazeLD.maze_id)) {
				createNewSave();
			}
		} else {
			svgd = null;
		}
	}

	void createNewSave() {
		svgd = new SaveGameData();
		svgd.mazeID = mazeLD.maze_id;
		svgd.currentRoomID = mazeLD.starting_question_id;

		if (SceneManager.GetActiveScene().name != "MainMenu_v2") {
			svgd.playing = true;
		} else {
			svgd.playing = false;
		}

		RoomPlayerInfo[] rooms = new RoomPlayerInfo[mazeLD.questions.Length];
		for (int i = 0; i < rooms.Length; i++) {
			rooms[i] = new RoomPlayerInfo(mazeLD.questions[i].question_id);
		}

        svgd.rooms = rooms; // Salvar as salas de questões

    }

	/// <summary>
	/// Apenas save local.
	/// </summary>
	public void saveProgress() {
		SaveData.save("savegame", JsonUtility.ToJson(svgd));
	}

	public UserInfo loadUser() {
		return SaveData.load<UserInfo>("user_data");
	}

	public void saveUser() {
		if (LoginHandler.handler.isValidUser) {
			string user_data = JsonUtility.ToJson(LoginHandler.handler.user);
			SaveData.save("user_data", user_data);
		}
	}

	// Enable and Disable VrMode
	public void switchVrMode(bool isEnabled) {
		this.vrMode = isEnabled;
	}

	public void cleanPlayerProgress(bool logout = false) {
		SaveData.removeFile("savegame");
		SaveData.removeFile("user_data");
		if (logout)
			SaveData.removeFile("current_level");
		checkAndCreateSave();
	}

	public void startMaze() {
		// FIREBASE ANALYTICS
		//Parameter[] StartParameters = {
		//		new Parameter("MazeID", svgd.mazeID),
		//		new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
		//		new Parameter("ElapsedTime", svgd.timeElapsed)
		//	};
		//FirebaseAnalytics.LogEvent(
		//	FirebaseAnalytics.EventLevelStart,
		//	StartParameters);

		//Eh nois, Analytics - evento executado ao inicializar o jogo
		EventInfo e = new EventInfo();
		e.event_name = "maze_start";
		e.maze_id = svgd.mazeID;
		int uid = LoginHandler.handler.user == null ? -1 : int.Parse(LoginHandler.handler.user.uid);
		e.user_id = uid <= 0 ? 0 : uid;
		e.question_id = svgd.currentRoomID;
		e.elapsed_time = Mathf.RoundToInt(svgd.timeElapsed);
		_ = EventPool.sendEvent(e);
	}

	public void endMaze() {
		//Parameter[] EndParameters = {
		//		new Parameter("MazeID", svgd.mazeID),
		//		new Parameter(FirebaseAnalytics.ParameterScore, svgd.score),
		//		new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
		//		new Parameter(FirebaseAnalytics.ParameterSuccess, svgd.finished.ToString()),
		//		new Parameter("RightAnswers", svgd.rightAnswers),
		//		new Parameter("WrongAnswers", svgd.wrongAnswers),
		//		new Parameter("ElapsedTime", svgd.timeElapsed)
		//	};
		//FirebaseAnalytics.LogEvent(
		//	FirebaseAnalytics.EventLevelEnd,
		//	EndParameters);

		//http://192.168.240.222/api/data?event_name=maze_end&user_id=2&maze_id=1&elapsed_time=5&wrong_count=1&correct_count=1&async_timestamp=4000
		//Eh nois, Analytics - evento executado ao inicializar o jogo

		if (svgd.mazeID == -1)
			return;
		EventInfo e = new EventInfo();
		e.event_name = "maze_end";
		e.maze_id = svgd.mazeID;
		int uid = LoginHandler.handler.user == null ? -1 : int.Parse(LoginHandler.handler.user.uid);
		e.user_id = uid <= 0 ? 0 : uid;
		e.question_id = svgd.currentRoomID;
		e.wrong_count = svgd.wrongAnswers;
		e.correct_count = svgd.rightAnswers;
		e.elapsed_time = Mathf.RoundToInt(svgd.timeElapsed);
		_ = EventPool.sendEvent(e);
	}

	void SceneChanged(Scene current, Scene next) {
		// cannot clean level data on main menu if we want the player to continue the next level while not finished

		string[] nonMazeScenes = new string[] {"MainMenu_v2"};
		if (svgd != null) {
			bool playing = true;
			for (int i = 0; i < nonMazeScenes.Length; i++) {
				if (next.name.Equals(nonMazeScenes[i])) {
					//Cursor.visible = true;
					playing = false;
					break;
				}
			}
			SaveData.save("savegame", JsonUtility.ToJson(svgd));
			// TODO: implementar também save na nuvem aqui

			if (svgd.playing) {
				//acabou de entrar na sala, cria um save ou não
				//manda evento de LevelStart
				checkAndCreateSave();
				startMaze();
			} else {
				//saiu da fase
				//manda evento de level end
				endMaze();
			}
			svgd.playing = playing;
		}

	}

	/// <summary>
	/// Método para enviar evento sobre acertos e erros para o(s) analytics
	/// </summary>
	/// <param name="room_id">sala atual do jogo</param>
	/// <param name="correct">resposta certa ou errada?</param>
	public void answerStatus(int room_id, bool correct) {
		//Parameter[] LevelUpParameters = {
		//		new Parameter("MazeID", svgd.mazeID),
		//		new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
		//		new Parameter("AnswerStatus", correct.ToString()),
		//		new Parameter("ElapsedTime", svgd.timeElapsed.ToString())
		//	};
		//FirebaseAnalytics.LogEvent(
		//	FirebaseAnalytics.EventLevelUp,
		//	LevelUpParameters);

		EventPool.sendQuestionEndEvent(correct);
		//if (correct) {
		//	svgd.timeElapsed = 0;
		//}
	}

	public void setActiveRoom(int room_id, bool end = false) {
		svgd.currentRoomID = room_id;
	}

	void FixedUpdate() {
		if ((svgd != null) && (svgd.playing)) {
			svgd.timeElapsed += Time.fixedDeltaTime;
		}
	}

	private void OnDestroy() {
		EventPoolWrapper ew = new EventPoolWrapper();
		ew.pool = EventPool.pool.ToArray();
		SaveData.save("event_pool", JsonUtility.ToJson(ew));

		SceneManager.activeSceneChanged -= SceneChanged;
	}
}
