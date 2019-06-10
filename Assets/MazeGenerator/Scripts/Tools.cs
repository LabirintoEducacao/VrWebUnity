using System;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
using UnityEngine;

namespace larcom.MazeGenerator.Support {

	public static class Tools {
		public static int getDirectionInt (MapCoord coord) {
			return getDirectionInt (new Vector2 (coord.x, coord.y));
		}
		public static int getDirectionInt (Vector2 delta) {
			for (int i = 0; i < Constants.DELTA.Length; i++) {
				MapCoord d_type = Constants.DELTA[i];
				if ((delta.x == d_type.x) && (delta.y == d_type.y)) {
					return i;
				}
			}
			Debug.LogWarning (String.Format ("Could not find direction for: {0}, {1}.", new object[2] { delta.x, delta.y }));
			return -1;
		}

		public static int deltaToDirectionInt (Tile t1, Tile t2) {
			return deltaToDirectionInt (t1.coord, t2.coord);
		}
		public static int deltaToDirectionInt (MapCoord p1, MapCoord p2) {
			MapCoord delta = p2 - p1;
			return getDirectionInt (delta);
		}

		public static void Shuffle<T> (this IList<T> list) {
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = UnityEngine.Random.Range (0, n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static int directionToIndex(int direction) {
			for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
				if (Constants.DIRECTIONS[i] == direction)
					return i;
			}
			return -1;
		}

		public static int getOpositeDirection (int direction) {
			for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
				if (Constants.DIRECTIONS[i] == direction)
					return Constants.OPPOSITE_DIRECTIONS[i];
			}
			return Constants.DIRECTION_NONE;
		}

		public static bool checkPassages (int passages, int[ ] possibilities) {
			foreach (int possib in possibilities) {
				if (passages == possib)
					return true;
			}
			return false;
		}

		public static bool isDeadEnd (Tile tile) {
			return checkPassages (tile.passages, Constants.DEAD_ENDS);
		}

		public static bool isBifurcation (Tile tile) {
			return checkPassages (tile.passages, Constants.BIFURCATION);
		}

		public static bool hasDoors (Tile tile) {
			return (tile.doors != Constants.DIRECTION_NONE);
		}
	}

	public class Constants {
		public static Color GIZMOS_WALL_COLOR = Color.black;

		public enum TILE_TYPE { EMPTY, WALL, CORRIDOR, ROOM, DOOR, DEBUG };

 public const int DIRECTION_NONE = 0;
 public const int DIRECTION_UP = 1;
 public const int DIRECTION_RIGHT = 2;
 public const int DIRECTION_DOWN = 4;
 public const int DIRECTION_LEFT = 8;
 public const int DIRECTION_ALL = DIRECTION_RIGHT | DIRECTION_LEFT | DIRECTION_DOWN | DIRECTION_UP;

 public static int[ ] DOORS = { DIRECTION_UP, DIRECTION_RIGHT, DIRECTION_DOWN, DIRECTION_LEFT };
 public static int[ ] DIRECTIONS = { DIRECTION_UP, DIRECTION_RIGHT, DIRECTION_DOWN, DIRECTION_LEFT };
 public static int[ ] OPPOSITE_DIRECTIONS = { DIRECTION_DOWN, DIRECTION_LEFT, DIRECTION_UP, DIRECTION_RIGHT };

 public const int WALL_UP = DIRECTION_RIGHT | DIRECTION_DOWN | DIRECTION_LEFT;
 public const int WALL_RIGHT = DIRECTION_UP | DIRECTION_DOWN | DIRECTION_LEFT;
 public const int WALL_DOWN = DIRECTION_UP | DIRECTION_RIGHT | DIRECTION_LEFT;
 public const int WALL_LEFT = DIRECTION_UP | DIRECTION_RIGHT | DIRECTION_DOWN;

 public static MapCoord[ ] DELTA = { MapCoord.UP, MapCoord.RIGHT, MapCoord.DOWN, MapCoord.LEFT };
 public static float[ ] ROTATIONS = {0f, 90f, 180f, 270f};

 public static int[ ] BIFURCATION = { //0111, 1011, 1101, 1110, 1111
 DIRECTION_ALL & ~DIRECTION_UP,
 DIRECTION_ALL & ~DIRECTION_RIGHT,
 DIRECTION_ALL & ~DIRECTION_DOWN,
 DIRECTION_ALL & ~DIRECTION_LEFT,
 DIRECTION_ALL
 };

 public static int[ ] CORRIDORS = { //1100, 1010, 1001, 0110, 0101, 0011
 DIRECTION_UP | DIRECTION_RIGHT,
 DIRECTION_UP | DIRECTION_DOWN,
 DIRECTION_UP | DIRECTION_LEFT,
 DIRECTION_RIGHT | DIRECTION_DOWN,
 DIRECTION_RIGHT | DIRECTION_LEFT,
 DIRECTION_DOWN | DIRECTION_LEFT
 };
 public static int[ ] DEAD_ENDS = { DIRECTION_UP, DIRECTION_RIGHT, DIRECTION_DOWN, DIRECTION_LEFT }; // 1000, 0100, 0010, 0001
 public static int[ ] UNREACHABLE = { DIRECTION_NONE }; // 0000
	}
}
