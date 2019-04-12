using larcom.MazeGenerator.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace larcom.MazeGenerator.Models {
	/// <summary>
	/// Classes that describes a contiguous Space in the map. 
	/// It can be specialized as, for an example: Room and Corridor.
	/// </summary>
	/// 
	[Serializable]
	public class Space: System.Object, ISerializationCallbackReceiver, IDisposable {
		public int space_id = -1;
		public SPACE_TYPE spaceType;
		public List<Tile> tiles;

		protected Map map;

		public Space(int id, Map map, SPACE_TYPE type) {
			this.space_id = id;
			this.spaceType = type;
			this.tiles = new List<Tile>();
			this.map = map;
		}

		public Space AddTile(Tile tile) {
			tiles.Add(tile);
			tile.space = this;
			return this;
		}

		public static bool checkArea (MapCoord topLeft, int width, int height, Map map, Constants.TILE_TYPE[] allowed) {
			MapCoord pos = topLeft;
			List<Constants.TILE_TYPE> allowable = new List<Constants.TILE_TYPE>(allowed);
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					Tile tile = map.tile(pos);
					if ((tile == null) || (!allowable.Contains(tile.occupation)))
						return false;

					//walk right
					pos = pos.n_right;
				}
				//walk down and to the left
				pos = pos.n_down;
				pos.x = topLeft.x;
			}
			return true;
		}

		#region interfaces_implementation
		Tile[] _tiles;

		public void OnAfterDeserialize () {
			tiles = new List<Tile>(_tiles);
			_tiles = null;
		}

		public void OnBeforeSerialize () {
			_tiles = tiles.ToArray();
		}

		public void Dispose () {
			this.map.removeSpace(this);
			this.tiles = null;
			this._tiles = null;
			this.map = null;
		}
		#endregion
	}

	public enum SPACE_TYPE { ROOM, CORRIDOR, FOREST, LAKE, VACUUM };
}