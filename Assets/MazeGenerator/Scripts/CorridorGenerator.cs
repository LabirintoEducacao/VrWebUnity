using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Generators;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour {
	public bool gizmosOn = true;

	[Header ("Random properties")]
	public int seed;
	public bool randomizeSeed = true;

	[Header ("Map Definition")]
	public bool cleanMaze = false;
	public int width = 5;
	public int height = 2;
	public int direction = Constants.DIRECTION_UP | Constants.DIRECTION_RIGHT;
	public float cellSize = 1;
	public GameObject tileBlock;

	private Map _map;
	public Map map { get => _map; }

	private Transform rootNode;
	private Vector3 _center;
	private Vector3 _delta;
	public Vector3 topLeft { get { return _center - _delta; } }
	public Vector3 bottomRight { get { return _center + _delta; } }

	[Header ("Entrada e saída")]
	public MapCoord entrance;
	public int doorDirectionIn;
	public MapCoord exit;
	public int doorDirectionOut;

	[Header ("sala")]
	public int roomWidth;
	public int roomHeight;

	PathFinder pathFinder;

	public void generateCorridor ( ) {
		if (randomizeSeed) {
			seed = Random.Range (int.MinValue, int.MaxValue);
		}
		Random.InitState (seed);

		_map = new Map (width, height);
		redefineCenter ( );

		IMazeConstructor mazer = new RecursiveBackTrackerMaze ( );
		mazer.generateMaze (_map, _map.rooms.Count);

		openIO ( );
		if (cleanMaze) {
			CorridorCleaner cc = new CorridorCleaner ( );
			int b4clean = _map.corridors[0].tiles.Count;
			while (true) {
				cc.cleanMaze (_map, 1);
				if (_map.corridors[0].tiles.Count == b4clean) {
					break;
				}
				b4clean = _map.corridors[0].tiles.Count;
			}
		}

		pathFinder = new PathFinder (map, entrance);
		generateMesh ( );
	}

	void openIO ( ) {
		Tile tile;
		if (doorDirectionIn > 0) {
			tile = _map.tile (entrance);
			tile.passages |= doorDirectionIn;
			tile.doors |= doorDirectionIn;
		}

		if (doorDirectionOut > 0) {
			tile = _map.tile (exit);
			tile.passages |= doorDirectionOut;
			tile.doors |= doorDirectionOut;
		}
	}

	void generateMesh ( ) {
		//Clean map
		if (rootNode == null) {
			rootNode = this.transform;
		} else {
			//clear root node
			clean ( );
		}

		//Blocks
		List<TileAsset> blocks = new List<TileAsset> ( );
		for (int i = 0; i < _map.width; i++) {
			for (int j = 0; j < _map.height; j++) {
				Vector3 position = this.topLeft + new Vector3 (i * cellSize, 0f, j * cellSize);
				GameObject prefab = tileBlock;
				Tile tile = _map.tile (i, j);
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
					TileAsset block = go.GetComponent<TileAsset> ( );
					block.tile = tile;
					blocks.Add (block);
				}
			}
		}
		//Create Renderer
		foreach (TileAsset block in blocks) {
			block.create ( );
		}
	}

	public void clean ( ) {
		if (rootNode == null)
			rootNode = this.transform;
		for (int i = rootNode.childCount - 1; i >= 0; i--) {
			DestroyImmediate (rootNode.GetChild (i).gameObject);
		}
	}

	void redefineCenter ( ) {
		_center = this.transform.position;
		if ((direction & Constants.DIRECTION_RIGHT) > 0) {
			_center += cellSize * Vector3.right * width / 2.0f;
		}
		if ((direction & Constants.DIRECTION_LEFT) > 0) {
			_center += cellSize * Vector3.left * width / 2.0f;
		}
		if ((direction & Constants.DIRECTION_UP) > 0) {
			_center += cellSize * Vector3.forward * height / 2.0f;
		}
		if ((direction & Constants.DIRECTION_DOWN) > 0) {
			_center += cellSize * Vector3.back * height / 2.0f;
		}
		_delta = new Vector3 (width - 1f, 0f, height - 1f) * cellSize / 2f;
	}

	private void OnDrawGizmosSelected ( ) {
		if (gizmosOn) {
			if (_map != null) {
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube (_center, new Vector3 (width, 1f, height));

				for (int i = 0; i < _map.width; i++) {
					for (int j = 0; j < _map.height; j++) {
						Vector3 pos = topLeft + new Vector3 (i, 0f, j) * cellSize;
						switch (_map.mapGrid[i, j].occupation) {
							case Constants.TILE_TYPE.EMPTY:
								Gizmos.color = Color.black;
								break;
							case Constants.TILE_TYPE.CORRIDOR:
								Gizmos.color = Color.HSVToRGB ((float) (_map.mapGrid[i, j].space.space_id + 1 - _map.rooms.Count) / _map.corridors.Count, 1f, 1f);
								break;
							case Constants.TILE_TYPE.ROOM:
								Gizmos.color = Color.HSVToRGB ((float) (_map.mapGrid[i, j].space.space_id) / _map.rooms.Count, 1f, 1f);
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

				if (pathFinder == null) {
					pathFinder = new PathFinder(map, entrance);
				}

				for (int i = 0; i < pathFinder.hubs.Count; i++) {
					PathHub hub = pathFinder.hubs[i];
					Gizmos.color = new Color (0f, 1f, 1f, 0.7f);
					Vector3 pos = topLeft + new Vector3 (hub.coord.x, 1f, hub.coord.y) * cellSize;
					Gizmos.DrawSphere (pos, 0.5f);
					// hubs
					foreach (PathHub h in hub.paths) {
						if (h != null) {
							Vector3 newPos = topLeft + new Vector3 (h.coord.x, 1f, h.coord.y) * cellSize;
							Gizmos.color = Color.white;
							Gizmos.DrawLine (pos, newPos);
						}
					}
				}
			}
		}
	}
}
