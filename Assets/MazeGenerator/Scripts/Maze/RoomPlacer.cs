using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace larcom.MazeGenerator.Generators {

	public class NotOverlappingRoomPlacer: IRoomPlacer {
		List<RoomGenModel> roomModels;
		Constants.TILE_TYPE[] allowedTilesToOverwrite = { Constants.TILE_TYPE.EMPTY };

		public NotOverlappingRoomPlacer() {
			roomModels = new List<RoomGenModel>();

			roomModels.Add(new RoomGenModel(4, 3, 0.3f));
			roomModels.Add(new RoomGenModel(2, 3, 0.3f));
			roomModels.Add(new RoomGenModel(4, 4, 0.2f));
			roomModels.Add(new RoomGenModel(2, 2, 0.2f));
		}

		public void PlaceRooms (Map map, int amount) {
			int rid = 0;
			for (int i = 0; i < amount; i++) {
				Tile tile = map.randomTile();
				RoomGenModel roomGen = selectModel();
				if (Models.Space.checkArea(tile.coord, roomGen.width, roomGen.height, map, allowedTilesToOverwrite)) {
					Room room = new Room(rid, map, tile.coord, roomGen.width, roomGen.height);
					rid++;
					map.addSpace(room);
				}
			}

			foreach (Room room in map.rooms) {
				room.closeRoom();
				Debug.Log(room);
			}
		}

		public RoomGenModel selectModel() {
			float odd = UnityEngine.Random.value;
			foreach (RoomGenModel model in roomModels) {
				if (model.odd > odd)
					return model;
				odd -= model.odd;
			}
			return roomModels[roomModels.Count-1];
		}

		public void placeRoom() {

		}
	}



	public interface IRoomPlacer {
		void PlaceRooms (Map map, int amount);
	}

	[Serializable]
	public struct RoomGenModel {
		public int width;
		public int height;
		public float odd;
		public RoomGenModel(int w, int h, float odd) {
			this.width = w;
			this.height = h;
			this.odd = odd;
		}
	}
}
