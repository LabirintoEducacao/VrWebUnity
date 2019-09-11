using Firebase.Analytics;
using larcom.support;
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
			var ld = SaveData.load("current_level");
			if (ld != null) {
				this.mazeLD = JsonUtility.FromJson<MazeLDWrapper>(ld);
				var save = SaveData.load("savegame");
				if (save != null) {
					svgd = JsonUtility.FromJson<SaveGameData>(save);
				}
			}
		}
	}
	#endregion

	void Start() {
		if ((mazeLD != null) && (svgd != null)) {
			FirebaseAnalytics.LogEvent("LoginWithData",
				new Parameter("MazeID", svgd.mazeID),
				new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
				new Parameter("ElapsedTime", svgd.timeElapsed)
				);
		}
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
	}
	private MazeLDWrapper _maze = null;
	public MazeLDWrapper mazeLD { get => _maze; set => _maze = value; }

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
			if (svgd != null) {
				svgd = new SaveGameData();
				svgd.mazeID = mazeLD.maze_id;
				svgd.currentRoomID = mazeLD.starting_question_id;
				svgd.playing = false;
			} else if ((svgd == null) || (svgd.mazeID != mazeLD.maze_id)) {
				svgd = new SaveGameData();
				svgd.mazeID = mazeLD.maze_id;
				svgd.currentRoomID = mazeLD.starting_question_id;
				svgd.playing = false;
			}
		} else {
			svgd = null;
		}
	}

	void cleanPlayerProgress() {
		SaveData.removeFile("savegame");
		checkAndCreateSave();
	}

	public void startRoom() {
		Parameter[] StartParameters = {
				new Parameter("MazeID", svgd.mazeID),
				new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
				new Parameter("ElapsedTime", svgd.timeElapsed)
			};
		FirebaseAnalytics.LogEvent(
			FirebaseAnalytics.EventLevelStart,
			StartParameters);
	}

	public void endRoom() {
		Parameter[] EndParameters = {
				new Parameter("MazeID", svgd.mazeID),
				new Parameter(FirebaseAnalytics.ParameterScore, svgd.score),
				new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
				new Parameter(FirebaseAnalytics.ParameterSuccess, svgd.finished.ToString()),
				new Parameter("RightAnswers", svgd.rightAnswers),
				new Parameter("WrongAnswers", svgd.wrongAnswers),
				new Parameter("ElapsedTime", svgd.timeElapsed)
			};
		FirebaseAnalytics.LogEvent(
			FirebaseAnalytics.EventLevelEnd,
			EndParameters);
	}

	void SceneChanged(Scene current, Scene next) {
		// cannot clean level data on main menu if we want the player to continue the next level while not finished
		//if (next.name.Equals("MainMenu")) {
		//	this.mazeLD = null;
		//}
		if (svgd != null) {
			svgd.playing = !next.name.Equals("MainMenu");
			SaveData.save("savegame", JsonUtility.ToJson(svgd));
			if (svgd.playing) {
				//acabou de entrar na sala, cria um save ou não
				//manda evento de LevelStart
				checkAndCreateSave();
				startRoom();
			} else {
				//saiu da fase
				//manda evento de level end
				endRoom();
			}
		}

	}

	public void answerStatus(int room_id, bool correct) {
		Parameter[] LevelUpParameters = {
				new Parameter("MazeID", svgd.mazeID),
				new Parameter(FirebaseAnalytics.ParameterLevel, svgd.currentRoomID),
				new Parameter("AnswerStatus", correct.ToString()),
				new Parameter("ElapsedTime", svgd.timeElapsed.ToString())
			};
		FirebaseAnalytics.LogEvent(
			FirebaseAnalytics.EventLevelUp,
			LevelUpParameters);
		if (correct) {
			svgd.timeElapsed = 0;
		}
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
		SceneManager.activeSceneChanged -= SceneChanged;
	}
}
