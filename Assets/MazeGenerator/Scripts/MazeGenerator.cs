using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Generators;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
	public bool gizmosOn = true;
	[Header ("Random properties")]
	public int seed;
	public bool randomizeSeed = true;
	[Header ("Map setup")]
	public int width = 32;
	public int height = 32;
	public float cellSize = 1f;
	public int roomAmount = 10;
	public int cleanIterations = 3;
	public ROOM_PLACER_TYPES roomPlacerType;

	[Header ("Map Creation Assets")]
	public GameObject rootNode;
	public GameObject tileBlock;

	public Map map;

	private Vector3 _center;
	private Vector3 _delta;
	public Vector3 topLeft { get { return _center - _delta; } }
	public Vector3 bottomRight { get { return _center + _delta; } }

	public void generate () {
		if (randomizeSeed) {
			seed = Random.Range (int.MinValue, int.MaxValue);
		}
		Random.InitState (seed);

		map = new Map (width, height);
		_center = this.transform.position;
		_delta = new Vector3 ((width - cellSize) / 2f, 0f, (height - cellSize) / 2f);

		//Rooms
		if (roomPlacerType == ROOM_PLACER_TYPES.NO_OVERLAPPING_NO_RETRY) {
			NotOverlappingRoomPlacer roomPlacer = new NotOverlappingRoomPlacer ();
			roomPlacer.PlaceRooms (map, roomAmount);
		}

		//Corridors
		IMazeConstructor mazer = new RecursiveBackTrackerMaze ();
		mazer.generateMaze (map, map.rooms.Count);

		//Open Doors
		foreach (Room r in map.rooms) {
			r.openDoors ();
		}
		//Clean Maze
		IMazeCleaner cleaner = new CorridorCleaner ();
		cleaner.cleanMaze (map, cleanIterations);

		//Clean map
		if (rootNode == null) {
			rootNode = new GameObject ("MapRoot");
		} else {
			//clear root node
			clean ();
		}

		render ();
	}

	void render () {
		List<TileAsset> blocks = new List<TileAsset> ();
		for (int i = 0; i < map.width; i++) {
			for (int j = 0; j < map.height; j++) {
				Vector3 position = this.topLeft + new Vector3 (i * cellSize, 0f, j * cellSize);
				GameObject prefab = tileBlock;
				Tile tile = map.tile (i, j);
				switch (tile.occupation) {
					case Constants.TILE_TYPE.EMPTY:
						prefab = null;
						break;
					case Constants.TILE_TYPE.CORRIDOR:
					case Constants.TILE_TYPE.ROOM:
					case Constants.TILE_TYPE.WALL:
					default:
						break;
				}
				if (prefab != null) {
					GameObject go = Instantiate (prefab, position, Quaternion.identity, rootNode.transform);
					TileAsset block = go.GetComponent<TileAsset> ();
					block.tile = tile;
					blocks.Add (block);
				}
			}
		}
		//Create Renderer
		foreach (TileAsset block in blocks) {
			block.create ();
		}
	}

	public void OnDrawGizmosSelected () {
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube (this.transform.position, new Vector3 (width, 1f, height));
		if (gizmosOn && (map != null)) {

			for (int i = 0; i < map.width; i++) {
				for (int j = 0; j < map.height; j++) {
					Vector3 pos = topLeft + new Vector3 (i, 0f, j) * cellSize;
					switch (map.mapGrid[i, j].occupation) {
						case Constants.TILE_TYPE.EMPTY:
							Gizmos.color = Color.black;
							break;
						case Constants.TILE_TYPE.CORRIDOR:
							Gizmos.color = Color.HSVToRGB ((float) (map.mapGrid[i, j].space.space_id + 1 - map.rooms.Count) / map.corridors.Count, 1f, 1f);
							break;
						case Constants.TILE_TYPE.ROOM:
							Gizmos.color = Color.HSVToRGB ((float) (map.mapGrid[i, j].space.space_id) / map.rooms.Count, 1f, 1f);
							break;
						case Constants.TILE_TYPE.WALL:
							Gizmos.color = Color.green;
							break;
						default:
							Gizmos.color = Color.cyan;
							break;
					}
					Gizmos.DrawCube (pos, Vector3.one);
				}
			}
		}
	}

	public void clean () {
		for (int i = rootNode.transform.childCount - 1; i >= 0; i--) {
			DestroyImmediate (rootNode.transform.GetChild (i).gameObject);
		}
	}

	[System.Serializable]
	public class MazeWallPrefabs {
		public GameObject FullTile;
		public GameObject TileW;
		public GameObject TileE;
		public GameObject TileN;
		public GameObject TileS;
	}
}