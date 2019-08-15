using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDescriptor : MonoBehaviour {
    public Vector2 size;
    public POSITIONING pivot;
    public Vector3 topLeft {
        get {
            Vector2 delta = Vector2.zero;
            //check the x pivoting possibilities that differ from TOP_LEFT
            if (checkPivotType(pivot, new POSITIONING[] {POSITIONING.RIGHT, POSITIONING.BOTTOM_RIGHT, POSITIONING.TOP_RIGHT})) {
                delta.x += size.x;
            } else if (checkPivotType(pivot, new POSITIONING[] {POSITIONING.TOP, POSITIONING.CENTER, POSITIONING.BOTTOM})) {
                delta.x += size.x/2f;
            }

            //check the y pivoting possibilities that differ from TOP_LEFT
            if (checkPivotType(pivot, new POSITIONING[] {POSITIONING.BOTTOM_LEFT, POSITIONING.BOTTOM, POSITIONING.BOTTOM_RIGHT})) {
                delta.y += size.y;
            } else if (checkPivotType(pivot, new POSITIONING[] {POSITIONING.LEFT, POSITIONING.CENTER, POSITIONING.RIGHT})) {
                delta.y += size.y/2f;
            }
            return transform.position + new Vector3(delta.x, 0f, delta.y);
        }
    }

    public Vector3 center {
        get {
            return this.topLeft + new Vector3(size.x, 0f, size.y)/2f;
        }
    }

    public Vector3 bottomRight {
        get {
            return this.topLeft + new Vector3(size.x, 0f, size.y);
        }
    }

    public static bool checkPivotType (POSITIONING pivot, POSITIONING[ ] possibilities) {
        foreach (POSITIONING option in possibilities) {
            if (option == pivot)
                return true;
        }
        return false;
    }
}

public enum POSITIONING { TOP_LEFT, TOP, TOP_RIGHT, LEFT, CENTER, RIGHT, BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT };
