using larcom.MazeGenerator.Generators;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor: Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();

		MazeGenerator mazeGenerator = (MazeGenerator) target;

		if (GUILayout.Button("Generate Map")) {
			mazeGenerator.generate();
		}

		if (GUILayout.Button("Clean")) {
			mazeGenerator.clean();
		}
	}
}
