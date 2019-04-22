using larcom.MazeGenerator.Generators;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
	public bool gizmosOn = true;
	[Header("Random properties")]
	public int seed;
	public bool randomizeSeed = true;
	[Header("Map setup")]
	public int width = 32;
	public int height = 32;
	public float cellSize = 1f;
	public int roomAmount = 10;
	public ROOM_PLACER_TYPES roomPlacerType;

	[Header("Map Creation Assets")]
	public GameObject rootNode;
	public GameObject corridorPrefab;
	public MazeWallPrefabs wallPrefabs;
	public GameObject emptyPrefab;
	public GameObject roomPrefab;

	public Map map;

	private Vector3 _center;
	private Vector3 _delta;
	public Vector3 topLeft { get { return _center - _delta; } }
	public Vector3 bottomRight { get { return _center + _delta; } }

	public void generate() {
		if (randomizeSeed) {
			seed = Random.Range(int.MinValue, int.MaxValue);
		}
		Random.InitState(seed);

		map = new Map(width, height);
		_center = this.transform.position;
		_delta = new Vector3((width - cellSize) / 2f, 0f, (height - cellSize) / 2f);

		//Rooms
		if (roomPlacerType == ROOM_PLACER_TYPES.NO_OVERLAPPING_NO_RETRY) {
			NotOverlappingRoomPlacer roomPlacer = new NotOverlappingRoomPlacer();
			roomPlacer.PlaceRooms(map, roomAmount);
		}

		//Corridors

		//Clean Maze

		//Clean map
		if (rootNode == null) {
			rootNode = new GameObject("MapRoot");
		} else {
			//clear root node
			for (int i = rootNode.transform.childCount-1; i >= 0; i--) {
				DestroyImmediate(rootNode.transform.GetChild(i).gameObject);
			}
		}

		//Render
		for (int i = 0; i < map.width; i++) {
			for (int j = 0; j < map.height; j++) {
				Vector3 position = this.topLeft + new Vector3(i * cellSize, 0f, j * cellSize);
				GameObject prefab = null;
				Tile tile = map.tile(i, j);
				switch (tile.occupation) {
					case Constants.TILE_TYPE.EMPTY:
						prefab = emptyPrefab;
						break;
					case Constants.TILE_TYPE.CORRIDOR:
						prefab = corridorPrefab;
						break;
					case Constants.TILE_TYPE.ROOM:
						prefab = roomPrefab;
						break;
					case Constants.TILE_TYPE.WALL:
						prefab = wallPrefabs.FullWall;
						break;
					default:
						break;
				}
				if (prefab != null) {
					GameObject go = Instantiate(prefab, position, Quaternion.identity, rootNode.transform);
					createTileSpecificWall(tile, position, go);
				}
				
			}
		}
	}

	void createTileSpecificWall(Tile tile, Vector3 position, GameObject parent) {
		if (tile.passages == Constants.DIRECTION_NONE)
			return;
		if ((tile.passages&Constants.DIRECTION_UP) == 0) {
			if (wallPrefabs.WallN) {
				Instantiate(wallPrefabs.WallN, position, Quaternion.identity, parent.transform);
			}
		}
		if ((tile.passages & Constants.DIRECTION_RIGHT) == 0) {
			if (wallPrefabs.WallE) {
				Instantiate(wallPrefabs.WallE, position, Quaternion.identity, parent.transform);
			}
		}
		if ((tile.passages & Constants.DIRECTION_DOWN) == 0) {
			if (wallPrefabs.WallS) {
				Instantiate(wallPrefabs.WallS, position, Quaternion.identity, parent.transform);
			}
		}
		if ((tile.passages & Constants.DIRECTION_LEFT) == 0) {
			if (wallPrefabs.WallW) {
				Instantiate(wallPrefabs.WallW, position, Quaternion.identity, parent.transform);
			}
		}

	}

	public void OnDrawGizmosSelected () {
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.transform.position, new Vector3(width, 1f, height));
		if (gizmosOn && (map != null)) {
			

			for (int i = 0; i < map.width; i++) {
				for (int j = 0; j < map.height; j++) {
					Vector3 pos = topLeft + new Vector3(i, 0f, j) * cellSize;
					switch (map.mapGrid[i, j].occupation) {
						case Constants.TILE_TYPE.EMPTY:
							Gizmos.color = Color.black;
							break;
						case Constants.TILE_TYPE.CORRIDOR:
							Gizmos.color = Color.yellow;
							break;
						case Constants.TILE_TYPE.ROOM:
							Gizmos.color = Color.HSVToRGB((float) (map.mapGrid[i, j].space.space_id)/roomAmount, 1f,1f);
							break;
						case Constants.TILE_TYPE.WALL:
							Gizmos.color = Color.green;
							break;
						default:
							Gizmos.color = Color.cyan;
							break;
					}
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}
		}
	}

	[System.Serializable]
	public class MazeWallPrefabs {
		public GameObject FullWall;
		public GameObject WallW;
		public GameObject WallE;
		public GameObject WallN;
		public GameObject WallS;
	}
}