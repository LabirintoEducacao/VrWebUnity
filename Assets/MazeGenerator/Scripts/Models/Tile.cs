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
		public int passages;
		public int doors;
		public TILE_TYPE occupation = TILE_TYPE.EMPTY;

		public int x { get { return coord.x; } }
		public int y { get { return coord.y; } }

		public Tile (int x, int y, Map map) {
			occupation = TILE_TYPE.EMPTY;
			this.coord = new MapCoord(x, y);

			setStartingPassages();
			this.doors = DIRECTION_NONE;
			this.map = map;
		}

		void setStartingPassages() {
			// start allowing all directions
			passages = DIRECTION_ALL;
			// check which passages go outside the game board and block them.
			if (this.left == null) passages &= ~DIRECTION_LEFT;
			if (this.right == null) passages &= ~DIRECTION_RIGHT;
			if (this.down == null) passages &= ~DIRECTION_DOWN;
			if (this.up == null) passages &= ~DIRECTION_UP;
		}

		public override string ToString () {
			int space_id = -1;
			if (space != null)
				space_id = space.space_id;
			return string.Format("Tile: {0}, {1}. Passage: {3} - Doors: {4} - Space: {5}", new object[] {coord.x, coord.y, passages, doors, space_id });
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