using UnityEngine;

public class MazeLevelDesign : MonoBehaviour {
  public TextAsset levelDesign;
  MazeLDWrapper mazeLD;

  private void Start() {
    if (levelDesign == null) {
      Debug.LogError("Cannot play without a level.");
    }  
    mazeLD = readData(levelDesign.text);
  }

  public MazeLDWrapper readData(string json) {
    return JsonUtility.FromJson<MazeLDWrapper>(json);
  }  
}