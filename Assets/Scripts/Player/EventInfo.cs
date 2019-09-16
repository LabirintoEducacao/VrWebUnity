using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public long ansync_timestamp;
}

public class EventPool {
	public static List<EventInfo> pool;

	public static void sendMazeStart(EventInfo e) {
		//teste

	}
}