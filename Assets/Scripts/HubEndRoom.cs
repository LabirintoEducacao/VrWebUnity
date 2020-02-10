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
			Debug.Log("Erros Totais: "+svgd.rightAnswers+" Acertos Totais: "+svgd.wrongAnswers);
			perScore = total;
			perScore = System.Math.Truncate(perScore);
		}

		sb.AppendLine(System.String.Format("Tempo Total: {0}h {1}m {2}s", hour, min, sec));
		sb.AppendLine(System.String.Format("Pontuação: {0}%", perScore));
		sb.AppendLine(System.String.Format("Acertos: {0}", svgd.rightAnswers));
		sb.AppendLine(System.String.Format("Erros: {0}", svgd.wrongAnswers));

		FinalText.text = sb.ToString();
	}

	public double countScore(){
		SaveGameData svgd = DataManager.manager.savegame;
		double total = svgd.rightAnswers;
		double totalScore = 0;
		double [] perScoreRoom = new double[svgd.rightAnswers];
		for (int i = 0; i < perScoreRoom.Length; i++){
			perScoreRoom[i] = (100 / total) / (svgd.rooms[i].wrongs + svgd.rooms[i].right);
			totalScore = totalScore + perScoreRoom[i];
			Debug.Log("Sala "+i+" Pontuação: "+perScoreRoom[i]);
		}
		Debug.Log("Pontuação Total: "+totalScore);
		return totalScore;
	}
}
