using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMeshBaker))]
public class NavMeshBakerEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    
    NavMeshBaker baker = target as NavMeshBaker;
    if (GUILayout.Button("Regenerate NavMesh")) {
      baker.CreateBake();
    }
  }
}