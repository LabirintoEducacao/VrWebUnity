using larcom.MazeGenerator.Models;
using System;
using UnityEngine;

namespace larcom.MazeGenerator.Support {

	public class Tools {
		public static int getDirectionInt(Vector2 delta) {
			for (int i = 0; i < Constants.DELTA.Length; i++) {
				MapCoord d_type = Constants.DELTA[i];
				if ((delta.x == d_type.x) && (delta.y == d_type.y)) {
					return i;
				}
			}
			Debug.LogWarning(String.Format("Could not find direction for: {0}, {1}.", new object[2] { delta.x, delta.y}));
			return -1;
		}
	}

	public class Constants {
		public static Color GIZMOS_WALL_COLOR = Color.black;

		public enum TILE_TYPE { EMPTY, WALL, CORRIDOR, ROOM, DOOR, DEBUG };

		public static int DIRECTION_NONE = 0;
		public static int DIRECTION_UP = 1;
		public static int DIRECTION_RIGHT = 2;
		public static int DIRECTION_DOWN = 4;
		public static int DIRECTION_LEFT = 8;
		public static int DIRECTION_ALL = DIRECTION_RIGHT|DIRECTION_LEFT|DIRECTION_DOWN|DIRECTION_UP;

		public static int[] DOORS = { DIRECTION_UP, DIRECTION_RIGHT, DIRECTION_DOWN, DIRECTION_LEFT };
		public static int[] DIRECTIONS = { DIRECTION_UP, DIRECTION_RIGHT, DIRECTION_DOWN, DIRECTION_LEFT };
		public static int[] OPPOSITE_DIRECTIONS = { DIRECTION_DOWN, DIRECTION_LEFT, DIRECTION_UP, DIRECTION_RIGHT };

		public static int WALL_UP = DIRECTION_RIGHT|DIRECTION_DOWN|DIRECTION_LEFT;
		public static int WALL_RIGHT = DIRECTION_UP|DIRECTION_DOWN|DIRECTION_LEFT;
		public static int WALL_DOWN = DIRECTION_UP|DIRECTION_RIGHT|DIRECTION_LEFT;
		public static int WALL_LEFT = DIRECTION_UP|DIRECTION_RIGHT|DIRECTION_DOWN;


		public static MapCoord[] DELTA = { MapCoord.UP, MapCoord.RIGHT, MapCoord.DOWN, MapCoord.LEFT };

		public static int[] BIFURCATION = { //0111, 1011, 1101, 1110, 1111
			DIRECTION_ALL &~DIRECTION_UP,
			DIRECTION_ALL &~DIRECTION_RIGHT,
			DIRECTION_ALL &~DIRECTION_DOWN,
			DIRECTION_ALL &~DIRECTION_LEFT,
			DIRECTION_ALL
		};

		public static int[] CORRIDORS = { //1100, 1010, 1001, 0110, 0101, 0011
			DIRECTION_UP|DIRECTION_RIGHT, DIRECTION_UP|DIRECTION_DOWN, DIRECTION_UP|DIRECTION_LEFT,
			DIRECTION_RIGHT|DIRECTION_DOWN, DIRECTION_RIGHT|DIRECTION_LEFT,
			DIRECTION_DOWN|DIRECTION_LEFT
		};
		public static int[] DEAD_ENDS = { DIRECTION_UP, DIRECTION_RIGHT, DIRECTION_DOWN, DIRECTION_LEFT }; // 1000, 0100, 0010, 0001
		public static int[] UNREACHABLE = { DIRECTION_NONE }; // 0000
	}
}
