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
		NotOverlappingRoomPlacer roomPlacer = new NotOverlappingRoomPlacer();
		roomPlacer.PlaceRooms(map, roomAmount);
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
							Gizmos.color = Color.red;
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
}
