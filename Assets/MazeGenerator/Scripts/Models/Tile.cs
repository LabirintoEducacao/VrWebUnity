using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static larcom.MazeGenerator.Support.Constants;

namespace larcom.MazeGenerator.Models {

	/// <summary>
	/// Class that defines the minimal logic space in the Map Grid, that is a tile.
	/// it holds its position (x,y) in the grid, the tile ocupational "function" and 
	/// the contiguous space (room or corridor) to which it belongs.
	/// </summary>
	[Serializable]
	public class Tile : System.Object {
		public Map map;
		public MapCoord coord;
		public Space space;
		public TILE_TYPE occupation = TILE_TYPE.EMPTY;

		public int x { get { return coord.x; } }
		public int y { get { return coord.y; } }

		public Tile (int x, int y, Map map) {
			occupation = TILE_TYPE.EMPTY;
			this.coord = new MapCoord(x, y);
			this.map = map;
		}

		public override string ToString () {
			return occupation.ToString() + " - space id: " + space.space_id;
		}

		public int getWallDirection() {
			if (occupation != TILE_TYPE.WALL) {
				return DIRECTION_NONE;
			}

			return surroundingWall();
		}

		#region neighbours
		public Tile left {
			get {
				if (x <= 0)
					return null;

				return map.tile(coord + MapCoord.LEFT);
			}
		}
		public Tile right {
			get {
				if (x >= map.width - 1)
					return null;

				return map.tile(coord + MapCoord.RIGHT);
			}
		}
		public Tile up {
			get {
				if (y >= map.height - 1)
					return null;

				return map.tile(coord + MapCoord.UP);
			}
		}
		public Tile down {
			get {
				if (y <= 0)
					return null;

				return map.tile(coord + MapCoord.DOWN);
			}
		}
		#endregion

		public int surroundingWall () {
			int wall_dir = DIRECTION_NONE;
			Tile[] neighbours = { this.up, this.right, this.down, this.left };
			for (int i = 0; i < DIRECTIONS.Length; i++) {
				if ((neighbours[i] != null) && (neighbours[i].occupation == TILE_TYPE.WALL)) {
					wall_dir |= DIRECTIONS[i];
				}
			}
			return wall_dir;
		}

		public int sameRoomNeighbours () {
			int passage = DIRECTION_NONE;
			Tile[] neighbours = { this.up, this.right, this.down, this.left };
			for (int i = 0; i < DIRECTIONS.Length; i++) {
				if (neighbours[i] != null) {
					if ((neighbours[i].space != null) && (neighbours[i].space.space_id == this.space.space_id)) {
						passage |= DIRECTIONS[i];
					}
				}
			}
			return passage;
		}

		public int getDoorDirection() {
			if (occupation != TILE_TYPE.DOOR) {
				return DIRECTION_NONE;
			}
			int wall_dir = surroundingWall();

			if (wall_dir == (DIRECTION_LEFT|DIRECTION_RIGHT)) {
				if ((up != null) && (up.occupation == TILE_TYPE.CORRIDOR)) {
					return DIRECTION_DOWN;
				}
				return DIRECTION_UP;
			} else {
				if ((right != null) && (right.occupation == TILE_TYPE.CORRIDOR)) {
					return DIRECTION_RIGHT;
				}
				return DIRECTION_LEFT;
			}
		}
	}
}