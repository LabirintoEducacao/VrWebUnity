using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class CorridorManager : MonoBehaviour {
  public Transform roomEntrance;
  public Transform roomExit;
  public MazePath pathInfo;
  public int cellSize = 1;
  public int question_id = -1;
  public int id;

  public void generateCorridor(GameObject tilePrefab, GameObject hubPrefab) {
    CorridorGenerator generator =  this.GetComponent<CorridorGenerator>();
    if (generator == null) {
      generator = this.gameObject.AddComponent<CorridorGenerator>();
    }

    generator.width = pathInfo.width;
    generator.height = pathInfo.height;
    generator.cleanMaze = (pathInfo.type == "corridor");
    generator.createDoors = false;
    generator.hubPrefab = hubPrefab;
    generator.tileBlock = tilePrefab;
    generator.cellSize = cellSize;
    createDoors(generator);
    generator.generateCorridor();

    if (roomEntrance != null) {
      generator.setEntranceMotion(roomEntrance);
    } else {
      Debug.LogWarning("Weird room with no entrance. Name: "+this.name);
    }

    if (roomExit != null) {
      generator.setExitMotion(roomExit);
    }
  }

  void createDoors(CorridorGenerator generator) {
    //corridor sprout direction
    generator.direction = Constants.DIRECTION_UP|Constants.DIRECTION_RIGHT;

    // entrance position
    generator.entrance = new MapCoord(Mathf.FloorToInt(generator.width/2),0);
    generator.doorDirectionIn = Constants.DIRECTION_DOWN;

    //exit position
    generator.exit = new MapCoord(Mathf.FloorToInt(generator.roomWidth/2),generator.height-1);
    generator.doorDirectionOut = Constants.DIRECTION_UP;
  }
}