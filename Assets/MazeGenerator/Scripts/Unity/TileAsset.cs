using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class TileAsset : MonoBehaviour, ITileCreator {
  private GameObject _representation;
	public Tile tile { get; set; } = null;
	public float cellSize { get; set; } = 1f;
  public GameObject representation { get => this._representation; }
  
  public MazeGenerator.MazeWallPrefabs mazeWall;
  public MazeGenerator.MazeWallPrefabs mazeDoors;
  public GameObject floorPrefab;

  public void create(bool hasDoors = true) {
		if (_representation != null) {
      clearChildren();
    	_representation = null;
		}
		
    if (
      (tile.occupation == Constants.TILE_TYPE.CORRIDOR) || 
      (tile.occupation == Constants.TILE_TYPE.ROOM) ) {
			_representation = Instantiate(floorPrefab, this.transform.position, Quaternion.identity, this.transform);
			createWalls();
			if (hasDoors)
				createDoors();
    }
  }

	void generateTile(){
      // if (value != null) {
      //   value.transform.parent = this.transform;
      // }
      // this._representation = value;
	}

  void createWalls() {
		if (tile.passages == Constants.DIRECTION_NONE)
			return;
		if ((tile.passages&Constants.DIRECTION_UP) == 0) {
			if (mazeWall.TileN) {
				Instantiate(mazeWall.TileN, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
		if ((tile.passages & Constants.DIRECTION_RIGHT) == 0) {
			if (mazeWall.TileE) {
				Instantiate(mazeWall.TileE, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
		if ((tile.passages & Constants.DIRECTION_DOWN) == 0) {
			if (mazeWall.TileS) {
				Instantiate(mazeWall.TileS, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
		if ((tile.passages & Constants.DIRECTION_LEFT) == 0) {
			if (mazeWall.TileW) {
				Instantiate(mazeWall.TileW, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
  }

  void createDoors() {
		if (tile.doors == Constants.DIRECTION_NONE)
			return;
		if ((tile.doors & Constants.DIRECTION_UP) != 0) {
			if (mazeDoors.TileN) {
				Instantiate(mazeDoors.TileN, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
		if ((tile.doors & Constants.DIRECTION_RIGHT) != 0) {
			if (mazeDoors.TileE) {
				Instantiate(mazeDoors.TileE, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
		if ((tile.doors & Constants.DIRECTION_DOWN) != 0) {
			if (mazeDoors.TileS) {
				Instantiate(mazeDoors.TileS, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
		if ((tile.doors & Constants.DIRECTION_LEFT) != 0) {
			if (mazeDoors.TileW) {
				Instantiate(mazeDoors.TileW, _representation.transform.position, Quaternion.identity, _representation.transform);
			}
		}
  }

  void clearChildren() {
    for (int i = this.transform.childCount; i < 0; i--) {
      Destroy(this.transform.GetChild(i));
    }
  }
}

public interface ITileCreator {
  GameObject representation { get; }
	Tile tile { get; set; }
	float cellSize { get; set; }

  void create(bool hasDoors = true);
}
