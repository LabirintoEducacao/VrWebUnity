using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;
using static larcom.MazeGenerator.Support.Constants;

namespace larcom.MazeGenerator.Generators {
    public interface IMazeConstructor {
        void generateMaze (Map map, int startId);
    }
    public class RecursiveBackTrackerMaze : IMazeConstructor {
        List<Tile> availableTiles;
        public void generateMaze (Map map, int startId) {
            availableTiles = new List<Tile> ();
            for (int i = 0; i < map.width; i++) {
                for (int j = 0; j < map.height; j++) {
                    Tile t = map.mapGrid[i,j];
                    if (t.occupation == TILE_TYPE.EMPTY) {
                        //remove all passages to be open during maze carving
                        t.passages = DIRECTION_NONE;
                        availableTiles.Add (t);
                    }
                }
            }

            int spaceID = startId;
            while (availableTiles.Count > 0) {
                Tile newTile = availableTiles[0];
                Corridor newCorridor = new Corridor(spaceID, map);
                _walkThrough(map, newTile, newCorridor);
                map.addSpace(newCorridor);
                spaceID ++;
            }

        }

        void _walkThrough (Map map, Tile tile, Corridor corridor) {
            availableTiles.Remove(tile);
            corridor.AddTile(tile);
            tile.occupation = TILE_TYPE.CORRIDOR;
            tile.space = corridor;
            List<int> directions = new List<int>(new int[] {0, 1, 2, 3});
            Tools.Shuffle(directions);
            while (directions.Count > 0) {
                int dir = directions[0];
                directions.Remove(dir);
                MapCoord pos = tile.coord + DELTA[dir];
                if (map.isInside(pos)) {
                    Tile newTile = map.tile(pos);
                    if (availableTiles.IndexOf(newTile) != -1) {
                        // quebrando paredes
                        tile.passages |= DIRECTIONS[dir];
                        newTile.passages |= OPPOSITE_DIRECTIONS[dir];
                        _walkThrough(map, newTile, corridor);
                    }
                }

            }
        }
    }
}