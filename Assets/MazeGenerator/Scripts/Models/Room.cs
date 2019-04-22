using larcom.MazeGenerator.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace larcom.MazeGenerator.Models {
	/// <summary>
	/// Class specialization of larcom.MazeGenerator.Models.Space to define a Room kind of space.
	/// </summary>
	public class Room: Space, IDisposable {
		public List<Tile> border;
		public List<Space> neighbours;

		public Room (int id, Map map, MapCoord topLeft, int width, int height) : base(id, map, SPACE_TYPE.ROOM) {
			border = new List<Tile>();
			neighbours = new List<Space>();

			MapCoord pos = topLeft;
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					Tile tile = map.tile(pos);
					if ((tile.space != null) && (tile.space.space_id != this.space_id)) {
						//merge overlaping rooms
						assimilateRoom(tile.space);
					}

					tile.space = this;
					tile.occupation = Constants.TILE_TYPE.ROOM;
					this.tiles.Add(tile);

					//walk right
					pos = pos.n_right;
				}
				//walk down and to the left
				pos = pos.n_down;
				pos.x = topLeft.x;
			}
		}

		public Room assimilateRoom(Space oldRoom) {
			foreach (Tile tile in oldRoom.tiles) {
				tile.space = this;
				tiles.Add(tile);
			}
			Tile[] output = oldRoom.tiles.ToArray();
			oldRoom.Dispose();

			return this;
		}

		public Room closeRoom() {
			int corners = 0;
			foreach (Tile tile in tiles) {
				int neighbourhood = tile.sameRoomNeighbours();
				int negativeSpace = ~neighbourhood & Constants.DIRECTION_ALL;
				if (negativeSpace > 0) {
					//some direction has no conection in the room... soo.... it's a border! Yay....
					this.border.Add(tile);
					//tile.occupation = Constants.TILE_TYPE.WALL;
					for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
						if ((negativeSpace&Constants.DIRECTIONS[i]) > 0) {
							// this direction has another thing, what is it?
							Tile neighbour = map.tile(tile.coord + Constants.DELTA[i]);
							if (neighbour != null) {
								tile.passages &= ~Constants.DIRECTIONS[i];
								neighbour.passages &= ~Constants.OPPOSITE_DIRECTIONS[i];
							}
						}
					}
				}
			}
			return this;
		}

		public new void Dispose() {
			base.Dispose();

			this.border = null;
			this.neighbours = null;
		}

		public override string ToString () {
			return String.Format("ROOM {0} - Tiles: {1} - Border: {2} - Neighbours: {3}", new object[4] {space_id, tiles.Count, border.Count, neighbours.Count });
		}
	}
}
