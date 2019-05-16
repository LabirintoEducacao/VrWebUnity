using System;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class PathFinder {
  List<MapCoord> visitedTiles;
  public List<PathHub> hubs;

  public PathFinder (Map map, MapCoord startingPosition) {
    hubs = new List<PathHub>();
    visitedTiles = new List<MapCoord>();
    List<MapCoord> path = new List<MapCoord> ();
    path.Add(startingPosition);
    walkAbout (map, startingPosition, Constants.DIRECTION_NONE, path, null);
  }

  void walkAbout (Map map, MapCoord entrance, int entranceDir, List<MapCoord> path, PathHub oHub) {
    this.visitedTiles.Add (entrance);
    Tile tile = map.tile (entrance);
    PathHub hub;
    List<MapCoord> newPath = new List<MapCoord>();
    //checking if it is the case to open a new path or just keep walking
    bool newHub = (Tools.isDeadEnd(tile) || Tools.isBifurcation(tile) || Tools.hasDoors(tile));
    if (newHub) {
      //got to a door, it, definetly is a hub
      hub = new PathHub(tile.coord);
      hub.doors = tile.doors;
      if (path.Count > 0) {
        //not starting point
        int dir = entranceDir;
        if ((dir != Constants.DIRECTION_NONE) && (oHub != null)) {
          hub.setPath(dir, oHub);
        } else {
          Debug.Log("No direction found.");
        }
      }
      this.hubs.Add(hub);
      if (Tools.isDeadEnd(tile))
        return;
    } else {
      hub = oHub;
      newPath = new List<MapCoord>(path);
    }

    for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
      if ((tile.passages & Constants.DIRECTIONS[i]) == 0) {
        // facing wall, change dir.
        continue;
      }

      newPath.Add(entrance);
      Tile newTile = map.tile (entrance + Constants.DELTA[i]);
      if (newTile == null) {
        // void, return
        continue;
      }

      if (this.visitedTiles.IndexOf(newTile.coord) != -1) {
        // já passou aqui
        continue;
      }
      walkAbout(map, newTile.coord, Constants.DIRECTIONS[i], newPath, hub);
    }
  }

  public PathHub getHubInTile(Tile tile) {
    return getHubInTile(tile.coord);
  }

  public PathHub getHubInTile(MapCoord pos) {
    return hubs.Find( t => t.coord.Equals(pos));
  }
}
