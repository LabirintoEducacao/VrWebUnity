﻿using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

namespace larcom.MazeGenerator.Generators {
    public interface IMazeCleaner {
        void cleanMaze (Map map, int amount);

    }

    public class CorridorCleaner : IMazeCleaner {
        public List<Tile> visitedTiles;
        public void cleanMaze (Map map, int amount) {
            
            for (int i = 0; i < amount; i++) {
                visitedTiles = new List<Tile> ();
                foreach (Corridor c in map.corridors) {
                    cleanCorridor (map, c);
                }
            }
        }

        void cleanCorridor (Map map, Corridor corridor) {
            Tile door = null;
            List<Tile> path = new List<Tile> ();
            foreach (Tile t in corridor.tiles) {
                if (t.doors != Constants.DIRECTION_NONE) {
                    door = t;
                    path.Add (t);
                    break;
                }
            }
            //flood maze, looking for useless branches
            floodMaze (map, door, path);
        }

        void floodMaze (Map map, Tile tile, List<Tile> path) {
            visitedTiles.Add (tile);
            List<Tile> newPath = new List<Tile> ();
            if (tile.doors != Constants.DIRECTION_NONE) {
                //door beggings a new path
                newPath.Add (tile);
            } else {
                //not a door, append to path
                newPath.AddRange (path);

                //check for deadend
                bool deadEnd = false;
                foreach (int ded in Constants.DEAD_ENDS) {
                    if (tile.passages == ded) {
                        deadEnd = true;
                        break;
                    }
                }

                if (deadEnd) {
                    //dead end, remove
                    removePath (tile, newPath.ToArray ());
                    return;
                } else {
                    //not a dead end, so... maybe... a bifurcation
                    foreach (int bif in Constants.BIFURCATION) {
                        if (tile.passages == bif) {
                            newPath = new List<Tile>(new Tile[] {tile});
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
                List<Tile> finalPath = new List<Tile>(newPath);
                if ((tile.passages & Constants.DIRECTIONS[i]) > 0) {
                    Tile newTile = map.tile (tile.coord + Constants.DELTA[i]);
                    if ((newTile != null) && (visitedTiles.IndexOf (newTile) == -1)) {
                        //not visited yet
                        finalPath.Add (newTile);
                        floodMaze (map, newTile, finalPath);
                    }
                }
            }
        }

        void removePath (Tile tile, Tile[] path) {
            Debug.Log ("CorridorCleaner::removePath - " + tile.ToString () + " path size: " + path.Length);
            int direction = Tools.getDirectionInt (path[1].coord - path[0].coord);
            path[0].passages &= ~Constants.DIRECTIONS[direction];
            for (int i = 1; i < path.Length; i++) {
                Tile t = path[i];
                if (t.space != null)
                    t.space.RemoveTile(tile);
                t.space = null;
                t.makeWall (false);
                
            }
        }
    }
}