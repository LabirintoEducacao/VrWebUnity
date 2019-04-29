using System;
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;

namespace larcom.MazeGenerator.Models {
	/// <summary>
	/// Class specialization of larcom.MazeGenerator.Models.Space to define a Room kind of space.
	/// </summary>
	public class Room : Space, IDisposable {
		public List<Border> border;
		public List<Space> neighbours;
		public List<Border> doors;

		public Room (int id, Map map, MapCoord topLeft, int width, int height) : base (id, map, SPACE_TYPE.ROOM) {
			border = new List<Border> ();
			neighbours = new List<Space> ();
			doors = new List<Border>();

			MapCoord pos = topLeft;
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					Tile tile = map.tile (pos);
					if ((tile.space != null) && (tile.space.space_id != this.space_id)) {
						//merge overlaping rooms
						assimilateRoom (tile.space);
					}

					tile.space = this;
					tile.occupation = Constants.TILE_TYPE.ROOM;
					this.tiles.Add (tile);

					//walk right
					pos = pos.n_right;
				}
				//walk down and to the left
				pos = pos.n_down;
				pos.x = topLeft.x;
			}
		}

		public Room assimilateRoom (Space oldRoom) {
			foreach (Tile tile in oldRoom.tiles) {
				tile.space = this;
				tiles.Add (tile);
			}
			Tile[] output = oldRoom.tiles.ToArray ();
			oldRoom.Dispose ();

			return this;
		}

		public Room closeRoom () {
			foreach (Tile tile in tiles) {
				int neighbourhood = tile.sameRoomNeighbours ();
				int negativeSpace = ~neighbourhood & Constants.DIRECTION_ALL;
				if (negativeSpace > 0) {
					//some direction has no conection in the room... soo.... it's a border! Yay....
					//tile.occupation = Constants.TILE_TYPE.WALL;
					for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
						if ((negativeSpace & Constants.DIRECTIONS[i]) > 0) {
							// this direction has another thing, what is it?
							Tile neighbour = map.tile (tile.coord + Constants.DELTA[i]);

							if (neighbour != null) {
								//raise wall in that direction as it's my neighbour property from here on
								tile.passages &= ~Constants.DIRECTIONS[i];
								neighbour.passages &= ~Constants.OPPOSITE_DIRECTIONS[i];
							}

							//add it as a border element.
							Border newBorder = new Border ();
							newBorder.tile = tile;
							newBorder.nextTile = neighbour;
							newBorder.doorDirection = Constants.DIRECTIONS[i];
							newBorder.oppositeDirection = Constants.OPPOSITE_DIRECTIONS[i];
							this.border.Add (newBorder);
						}
					}
				}
			}
			return this;
		}

		public void openDoors () {
			List<Space> remainingNeighbours = new List<Space> ();
			Border[] reducedBorders = null;
			// Firstly, check for borders that are not borders anymore
			foreach (Border b in border) {
				if (b.nextTile != null) {
					if (b.nextTile.space != null) {
						if ((b.tile.doors & b.doorDirection) == 0) {
							//there is no door here, it's a neighbour
							if (remainingNeighbours.IndexOf (b.nextTile.space) == -1)
								remainingNeighbours.Add (b.nextTile.space);
						} else {
							doors.Add(b);
						}
					}
				}
			}
			foreach (Border b in doors) {
				remainingNeighbours.Remove(b.nextTile.space);
			}
			reducedBorders = getRemainingBorder (remainingNeighbours);

			while (reducedBorders.Length > 0) {
				int borderI = UnityEngine.Random.Range (0, reducedBorders.Length);
				Border b = reducedBorders[borderI];
				openDoor(b);
				//remove the newly connected space from available options and continue.
				remainingNeighbours.Remove (b.nextTile.space);
				reducedBorders = getRemainingBorder (remainingNeighbours);
			}
		}

		void openDoor(Border b) {
			doors.Add(b);
			b.tile.doors |= b.doorDirection;
			b.tile.passages |= b.doorDirection;
			b.nextTile.doors |= b.oppositeDirection;
			b.nextTile.passages |= b.oppositeDirection;
		}

		Border[] getRemainingBorder (List<Space> validSpaces) {
			List<Border> reducedBorders = new List<Border> ();
			foreach (Border b in border) {
				if ((b.nextTile != null) && (b.nextTile.space != null)) {
					if (validSpaces.IndexOf (b.nextTile.space) != -1) {
						//this place has not been seen yet, add as border
						reducedBorders.Add (b);
					}
				}
			}
			return reducedBorders.ToArray ();
		}

		public new void Dispose () {
			base.Dispose ();

			this.border = null;
			this.neighbours = null;
		}

		public override string ToString () {
			return String.Format ("ROOM {0} - Tiles: {1} - Border: {2} - Neighbours: {3}", new object[4] { space_id, tiles.Count, border.Count, neighbours.Count });
		}
	}

	[Serializable]
	public struct Border {
		public Tile tile;
		public Tile nextTile;
		public int doorDirection;
		public int oppositeDirection;
	}
}