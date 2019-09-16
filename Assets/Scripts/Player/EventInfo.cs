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

		e.async_timestamp = System.DateTime.Now.ToBinary();
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
}