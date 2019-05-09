using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CorridorGenerator))]
public class CorridorGeneratorEditor: Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();

		CorridorGenerator gen = (CorridorGenerator) target;

		if (GUILayout.Button("Generate")) {
			gen.generateCorridor();
		}

		if (GUILayout.Button("Clean")) {
			gen.clean();
		}
	}
}
