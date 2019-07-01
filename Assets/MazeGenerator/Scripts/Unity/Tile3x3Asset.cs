using larcom.MazeGenerator.Models;
using UnityEngine;
using larcom.MazeGenerator.Support;

public class Tile3x3Asset : MonoBehaviour, ITileCreator {
  public GameObject representation { get => this.gameObject; }
  public Tile tile { get; set; }
  public float cellSize { get; set; } = 1f;

  public GameObject[] floorPrefabs;
  public GameObject[] lateralWallPrefabs;
  public GameObject[] cornerWallPrefabs;

  public void create (bool hasDoors = true) {
    clearChildren ( );

    if (tile.passages == Constants.DIRECTION_NONE) {
      // inacessible path, not creating
      return;
    }

    createFloors ( );
    createWalls ( );
    if (hasDoors)
      createDoors ( );
  }

  void createFloors ( ) {
    int area_tiles = 3;
    float tile_size = cellSize / area_tiles;
    for (int i = 0; i < area_tiles; i++) {
      for (int j = 0; j < area_tiles; j++) {
        Vector3 pos = new Vector3(-1 + i, 0f, -1 + j) * tile_size;
        GameObject go = createRandomPrefab(floorPrefabs, pos);
      }
    }
  }

  GameObject createRandomPrefab(GameObject[] prefabs, Vector3 pos) {
    return Instantiate(prefabs[Random.Range(0,prefabs.Length)], this.transform.position + pos, Quaternion.identity, this.transform);
  }

  void createWalls ( ) {
    int area_tiles = 3;
    float tile_size = cellSize / area_tiles;
    // corredores tem cantos extras
    if (tile.occupation == Constants.TILE_TYPE.CORRIDOR) {
      int[] corners = new int[] {
        Constants.DIRECTION_UP|Constants.DIRECTION_RIGHT,
        Constants.DIRECTION_RIGHT|Constants.DIRECTION_DOWN,
        Constants.DIRECTION_DOWN|Constants.DIRECTION_LEFT,
        Constants.DIRECTION_LEFT|Constants.DIRECTION_UP};

      Vector3[] cornerPos = new Vector3[] {
        new Vector3(0.5f, 0f, 1.5f),
        new Vector3(1.5f, 0f, -0.5f),
        new Vector3(-0.5f, 0f, -1.5f),
        new Vector3(-1.5f, 0f, 0.5f)
      };
      
      for (int i = 0; i < corners.Length; i++) {
        if ((tile.passages&corners[i]) == corners[i]) {
          Vector3 pos = new Vector3(-1, 0f, 1) * tile_size;
          GameObject go = createRandomPrefab(cornerWallPrefabs, Vector3.zero);
          go.transform.Rotate(0f, 90f * i, 0f);
          go.transform.localPosition = cornerPos[i];
        }
      }
    }

    // paredes de fato
    for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
      if ((tile.passages&Constants.DIRECTIONS[i]) != 0)
        continue;

      Vector3 pos = new Vector3(Constants.DELTA[i].x, 0f, Constants.DELTA[i].y - 0.5f);
      bool isX = (i%2 == 0); // 0 e 2 estão na vertical e variam apenas o x pra colocar o muro
      if (isX) {
        pos.x -= 1f;
        pos.z = 1.5f * Mathf.Sign(pos.z);
      } else {
        pos.z -= 0.5f;
      }
      for (int dx = 0; dx < area_tiles; dx++) {
        GameObject go = createRandomPrefab(lateralWallPrefabs, Vector3.zero);
        go.transform.Rotate(0f, -90f * i, 0f);
        go.transform.localPosition = pos * tile_size;

        if (isX)
          pos.x ++;
        else
          pos.z ++;
          
      }
    }
  }

  void createDoors ( ) {

  }

  void clearChildren ( ) {
    for (int i = this.transform.childCount; i < 0; i--) {
      Destroy (this.transform.GetChild (i));
    }
  }
}
