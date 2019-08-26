
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;

namespace larcom.MazeGenerator.Models {
	[System.Serializable]
	public class Map : ISerializationCallbackReceiver {
		public int width;
		public int height;
		public Constants.TILE_TYPE borderType = Constants.TILE_TYPE.WALL;

		public Tile[, ] mapGrid;
		[HideInInspector] [System.NonSerialized]
		public Tile[ ] _SerialGrid;

		[System.NonSerialized]
		List<Space> spaces;
		public List<Space> rooms {
			get {
				return spaces.FindAll (x => x.spaceType.Equals (SPACE_TYPE.ROOM));
			}
		}
		public List<Space> corridors {
			get {
				return spaces.FindAll (x => x.spaceType.Equals (SPACE_TYPE.CORRIDOR));
			}
		}
		public List<Space> allSpaces {
			get {
				return spaces;
			}
		}

		public Map (int width, int height) {
			this.width = width;
			this.height = height;

			spaces = new List<Space> ( );
			mapGrid = new Tile[width, height];
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					mapGrid[i, j] = new Tile (i, j, this);
				}
			}
		}

		public Tile randomTile (bool border = true) {
			if (border)
				return tile (Random.Range (1, width - 2), Random.Range (1, height - 2));
			return tile (Random.Range (0, width - 1), Random.Range (0, height - 1));
		}

		public void addSpace (Space space) {
			spaces.Add (space);
		}

		public void removeSpace (Space space) {
			spaces.Remove (space);
		}

		public bool isInside (int x, int y) { return isInside (new MapCoord (x, y)); }
		public bool isInside (MapCoord coord) {
			if ((coord.x < 0) || (coord.x >= width) ||
				(coord.y < 0) || (coord.y >= height)) {
				return false;
			}
			return true;
		}

		public Tile tile (int x, int y) { return tile (new MapCoord (x, y)); }
		public Tile tile (MapCoord coord) {
			if (!isInside (coord))
				return null;
			try {
				return mapGrid[coord.x, coord.y];
			} catch (System.IndexOutOfRangeException e) {
				Debug.Log (e.StackTrace);
				Debug.Log (coord);
				return null;
			}
		}

		public void OnBeforeSerialize ( ) {
			if (this.mapGrid != null) {
				this._SerialGrid = new Tile[width * height];
				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						this._SerialGrid[i * height + j] = mapGrid[i, j];
					}
				}
			}
		}

		public void OnAfterDeserialize ( ) {
			if (this._SerialGrid != null) {
				this.mapGrid = new Tile[width, height];
				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						mapGrid[i, j] = this._SerialGrid[i * height + j];
					}
				}
			}
		}
	}

	[System.Serializable]
	public class MapCoord {
		public int x;
		public int y;
		public MapCoord (int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override string ToString ( ) {
			return x + ", " + y;
		}

		// override object.Equals
		public override bool Equals (object obj) {
			if (obj == null || GetType ( ) != obj.GetType ( )) {
				return false;
			}

			MapCoord other = obj as MapCoord;
			if ((other.x == this.x) && (other.y == this.y))
				return true;
			return base.Equals (obj);
		}

		public static bool operator == (MapCoord p1, MapCoord p2) {
			return p1.Equals (p2);
		}

		public static bool operator != (MapCoord p1, MapCoord p2) {
			return !p1.Equals (p2);
		}

		// override object.GetHashCode
		public override int GetHashCode ( ) {
			return this.ToString ( ).GetHashCode ( );
		}

#region accessories
		public MapCoord n_up {
			get { return (this + UP); }
		}
		public MapCoord n_right {
			get { return (this + RIGHT); }
		}
		public MapCoord n_down {
			get { return (this + DOWN); }
		}
		public MapCoord n_left {
			get { return (this + LEFT); }
		}

		public static MapCoord operator + (MapCoord c1, MapCoord c2) {
			return new MapCoord (c1.x + c2.x, c1.y + c2.y);
		}
		public static MapCoord operator - (MapCoord c1, MapCoord c2) {
			return new MapCoord (c1.x - c2.x, c1.y - c2.y);
		}

		public static MapCoord ZERO { get { return new MapCoord (0, 0); } }
		public static MapCoord ONE { get { return new MapCoord (1, 1); } }
		public static MapCoord UP { get { return new MapCoord (0, 1); } }
		public static MapCoord RIGHT { get { return new MapCoord (1, 0); } }
		public static MapCoord DOWN { get { return new MapCoord (0, -1); } }
		public static MapCoord LEFT { get { return new MapCoord (-1, 0); } }
#endregion
	}
}
