using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubEndRoom : MonoBehaviour {

	[Header("UI")]
	public TMPro.TextMeshProUGUI FinalText;

	private void OnTriggerEnter(Collider other) {
		//TODO Criar Panel com Score
		// Adiciona Texto
		SaveGameData svgd = DataManager.manager.savegame;
		var sb = new System.Text.StringBuilder();

		// Formata o tempo
		double sec = System.Math.Truncate(svgd.timeElapsed);

		int min = 0;
		int hour = 0;
		while (sec >= 60f) {
			sec -= 60f;
			min++;

			if (min == 60) {
				min = 0;
				hour++;
			}
		}

		double total = countScore();
		double perScore = 0f;
		if (total != 0) {
			//Debug.Log("Erros Totais: "+svgd.wrongAnswers+" Acertos Totais: "+svgd.rightAnswers);
			perScore = total;
			perScore = System.Math.Truncate(perScore);
		}

		sb.AppendLine(System.String.Format("Tempo Total: {0}h {1}m {2}s", hour, min, sec));
		sb.AppendLine(System.String.Format("Pontuação: {0}%", perScore));
		sb.AppendLine(System.String.Format("Acertos: {0}", svgd.rightAnswers));
		sb.AppendLine(System.String.Format("Erros: {0}", svgd.wrongAnswers));

		svgd.rightAnswers = 0;
		svgd.wrongAnswers = 0;

		FinalText.text = sb.ToString();
	}

	public double countScore(){
		SaveGameData svgd = DataManager.manager.savegame;
		svgd.rightAnswers = svgd.rooms.Length;
		svgd.wrongAnswers = 0;
		for(int b = 0; b < svgd.rooms.Length; b++) svgd.wrongAnswers = svgd.wrongAnswers + svgd.rooms[b].wrongs;
		double total = svgd.rooms.Length;
		double totalScore = 0;
		double [] perScoreRoom = new double[svgd.rooms.Length];
		for (int i = 0; i < perScoreRoom.Length; i++){
			perScoreRoom[i] = (100 / total) / (svgd.rooms[i].wrongs + svgd.rooms[i].right);
			totalScore = totalScore + perScoreRoom[i];
			Debug.Log("Sala "+i+" Pontuação: "+perScoreRoom[i]);
		}
		return totalScore;
	}
}
