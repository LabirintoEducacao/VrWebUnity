using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class EventInfo {
	// nome do evento enviado
	public string event_name;
	// id do usuário ou -1 no caso de não logado.
	public int user_id;
	// id da sala(maze)
	public int maze_id;
	// id da pergunta(question)
	public int question_id;
	// id da alternativa
	public int answer_id;
	// número de vezes que interagiu com resposta errada
	public int wrong_count;
	// número de vezes que interagiu com resposta correta
	public int correct_count;
	// a resposta está certa ou errada?
	public bool correct;
	// tempo decorrido em segundos do inicio do jogo pela primeira vez
	public int elapsed_time;
	// Quantas vezes cada alternativa foi acessada naquela pergunta
	public int aswers_read_count;
	// momento do evento localmente, só não será nulo quando o evento não 
	// conseguiu ser enviado na hora.
	public long async_timestamp;
}

public class EventPool {
	public static List<EventInfo> pool = new List<EventInfo>();

	public static string createMazeStartURL(EventInfo e) {
		//curl -X POST "http://192.168.240.222/api/data?event_name=maze_start&user_id=1&maze_id=2&question_id=3&elapsed_time=1&async_timestamp=0"
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}

	public static string createQuestionStartURL(EventInfo e) {
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}&wrong_count={5}&correct_count={6}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time, e.wrong_count, e.correct_count });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}

	public static string createQuestionReadURL(EventInfo e) {
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}

	public static string createAnswerReadURL(EventInfo e) {
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}&answer_id={5}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time, e.answer_id });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}

	public static string createAnswerInteractionURL(EventInfo e) {
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}&answer_id={5}&correct={6}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time, e.answer_id, e.correct ? "1" : "-1" });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}

	public static string createQuestionEndURL(EventInfo e) {
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}&correct={5}&correct_count={6}&wrong_count={7}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time, e.correct ? "1" : "-1", e.correct_count, e.wrong_count });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}
	public static string createMazeEndURL(EventInfo e) {
		WebServiceData webAPI = Resources.FindObjectsOfTypeAll<WebServiceData>()[0];
		string url = string.Format("{0}?event_name={1}&maze_id={2}&question_id={3}&elapsed_time={4}&correct_count={5}&wrong_count={6}", new object[] { webAPI.eventURL, e.event_name, e.maze_id, e.question_id, e.elapsed_time, e.correct_count, e.wrong_count });
		if (e.user_id > 0) {
			url += "&user_id=" + e.user_id.ToString();
		} else {
			url += "&user_id=0";
		}

		if (e.async_timestamp > 0) {
			url += "&async_timestamp=" + e.async_timestamp;
		}
		return url;
	}

	public static async Task<int> sendEvent(EventInfo e) {
		string url = "";
		switch (e.event_name) {
			case "maze_start":
				url = createMazeStartURL(e);
				break;
			case "question_start":
				url = createQuestionStartURL(e);
				break;
			case "answer_read":
				url = createAnswerReadURL(e);
				break;
			case "answer_interaction":
				url = createAnswerInteractionURL(e);
				break;
			case "question_end":
				url = createQuestionEndURL(e);
				break;
			case "maze_end":
				url = createMazeEndURL(e);
				break;
			default:
				Debug.LogError("Event - " + e.event_name + ". Not yet implemented.");
				return -1;
		}

		if (Application.internetReachability != NetworkReachability.NotReachable) {
			using (UnityWebRequest uwr = UnityWebRequest.Post(url, "")) {
				uwr.downloadHandler = new DownloadHandlerBuffer();
				await uwr.SendWebRequest();
				if (uwr.isNetworkError || uwr.isHttpError) {
					Debug.LogWarning(string.Format("Could not send {1} event. Due to - {0}", new object[] { uwr.error, e.event_name }));
				} else {
					string data = Encoding.UTF8.GetString(uwr.downloadHandler.data);
					dynamic info = JsonUtility.FromJson<dynamic>(data);
					if (info.success == -1) {
						Debug.LogWarning(string.Format("Could not send {1} event. Due to - {0}", new object[] { info.error, e.event_name }));
					} else {
						Debug.Log(string.Format("Event {0} sent successfully. Due to - {1}", new object[] { e.event_name, data }));
					}
					return 1;
				}
			}
		} else {
			Debug.LogWarning(string.Format("Could not send {0} event. Due to - No WIFI/LAN connection.", new object[] { e.event_name }));
		}

		List<string> importants = new List<string>(new string[] {"maze_start", "question_start", "question_end", "maze_end"});
		if (importants.IndexOf(e.event_name) != -1) {
			if (e.async_timestamp < 0) {
				e.async_timestamp = System.DateTime.Now.ToBinary();
			}
			pool.Add(e);
		}
		return 0;
	}

#region event creation
	public static void sendMazeStartEvent() {
		EventInfo e = baseEventInfo("maze_start");
		EventPool.sendEvent(e);
	}

	public static void sendQuestionStartEvent() {
		SaveGameData svgd = DataManager.manager.savegame;
		EventInfo e = new EventInfo();
		e.event_name = "question_start";
		e.maze_id = svgd.mazeID;
		e.question_id = svgd.currentRoomID;
		if (LoginHandler.handler.user != null) {
			e.user_id = int.Parse(LoginHandler.handler.user.uid);
		} else {
			e.user_id = -1;
		}
		e.elapsed_time = Mathf.RoundToInt( svgd.timeElapsed );
		e.wrong_count = svgd.wrongAnswers;
		e.correct_count = svgd.rightAnswers;
		DataManager.manager.savegame.setRoomStart(e.question_id);
		EventPool.sendEvent(e);
	}

	public static void sendQuestionReadEvent() {
		EventInfo e = baseEventInfo("question_read");

		SaveGameData svgd = DataManager.manager.savegame;
		e.question_id = svgd.currentRoomID;
		EventPool.sendEvent(e);
	}

	public static void sendAnswerReadEvent(int answer_id) {
		EventInfo e = baseEventInfo("answer_read");

		SaveGameData svgd = DataManager.manager.savegame;
		e.question_id = svgd.currentRoomID;
		e.answer_id = answer_id;
		EventPool.sendEvent(e);
	}

	public static void sendAnswerInteractionEvent(int answer_id, bool correct) {
		EventInfo e = baseEventInfo("answer_interaction");

		SaveGameData svgd = DataManager.manager.savegame;
		e.question_id = svgd.currentRoomID;
		e.answer_id = answer_id;
		e.correct = correct;
		DataManager.manager.savegame.setAnswer(e.question_id, answer_id);
		EventPool.sendEvent(e);
	}

	public static void sendQuestionEndEvent(bool correct) {
		EventInfo e = baseEventInfo("question_end");

		SaveGameData svgd = DataManager.manager.savegame;
		e.question_id = svgd.currentRoomID;
		e.wrong_count = svgd.wrongAnswers;
		e.correct_count = svgd.rightAnswers;
		e.correct = correct;
		DataManager.manager.savegame.setRoomEnd(e.question_id, correct);
		EventPool.sendEvent(e);
	}

	public static void sendMazeEndEvent() {
		EventInfo e = baseEventInfo("maze_end");

		SaveGameData svgd = DataManager.manager.savegame;
		e.wrong_count = svgd.wrongAnswers;
		e.correct_count = svgd.rightAnswers;
		EventPool.sendEvent(e);
	}

	/// <summary>
	/// Evento base, com os campos: user_id, maze_id e elapsed_time; que estão em todos os eventos.
	/// </summary>
	/// <param name="event_name">Nome do evento a ser criado.</param>
	/// <returns></returns>
	static EventInfo baseEventInfo(string event_name) {
		SaveGameData svgd = DataManager.manager.savegame;
		EventInfo e = new EventInfo();
		e.event_name = event_name;
		e.maze_id = svgd.mazeID;
		int uid = LoginHandler.handler.user == null ? -1 : int.Parse(LoginHandler.handler.user.uid);
		e.user_id = uid <= 0 ? 0 : uid;
		e.elapsed_time = Mathf.RoundToInt(svgd.timeElapsed);
		return e;
	}
#endregion
}

public class EventPoolWrapper {
	public EventInfo[] pool;
}