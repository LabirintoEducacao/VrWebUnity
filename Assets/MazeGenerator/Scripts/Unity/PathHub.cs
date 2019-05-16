
using System;
using larcom.MazeGenerator.Models;
using larcom.MazeGenerator.Support;

public class PathHub {
  public int doors = Constants.DIRECTION_NONE;
  public MapCoord coord;
  public PathHub pathUp;
  public PathHub pathRight;
  public PathHub pathDown;
  public PathHub pathLeft;

  public PathHub(MapCoord coord) {
    this.coord = coord;
  }

  public PathHub[] paths {
    get {
      return new PathHub[] { pathUp, pathRight, pathDown, pathLeft };
    }
  }
  
  public PathHub getPath(int direction) {
    switch (direction) {
      case Constants.DIRECTION_UP:
        return this.pathUp;
      case Constants.DIRECTION_RIGHT:
        return this.pathRight;
      case Constants.DIRECTION_DOWN:
        return this.pathDown;
      case Constants.DIRECTION_LEFT:
        return this.pathLeft;
    }
    return null;
  }

  public void setPath(int direction, PathHub destination){
    switch (direction) {
      case Constants.DIRECTION_UP:
        this.pathUp = destination;
        break;
      case Constants.DIRECTION_RIGHT:
        this.pathRight = destination;
        break;
      case Constants.DIRECTION_DOWN:
        this.pathDown = destination;
        break;
      case Constants.DIRECTION_LEFT:
        this.pathLeft = destination;
        break;
    }
  }
}
