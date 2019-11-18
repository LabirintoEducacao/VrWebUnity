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

		int total = svgd.rightAnswers + svgd.wrongAnswers;
		double perScore = 0f;
		if (total != 0) {

			perScore = (svgd.rightAnswers / total) * 100;
			perScore = System.Math.Truncate(perScore);
		}

		sb.AppendLine(System.String.Format("Tempo Total: {0}h {1}m {2}s", hour, min, sec));
		sb.AppendLine(System.String.Format("Pontuação: {0}%", perScore));
		sb.AppendLine(System.String.Format("Acertos: {0}", svgd.rightAnswers));
		sb.AppendLine(System.String.Format("Erros: {0}", svgd.wrongAnswers));

		FinalText.text = sb.ToString();
	}
}
