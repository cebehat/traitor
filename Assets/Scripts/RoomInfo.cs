using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    //X is the east/west coordinate
    public int X { get; set; }
    //Z is the north/south coordinate
    public int Z { get; set; }

    public bool IsSpawned { get; set; } = false;
    public RoomType RoomType { get; set; }
    public Dictionary<RoomDirection, RoomComponent> Walls { get; set; }
        = new Dictionary<RoomDirection, RoomComponent> {
            { RoomDirection.NORTH, RoomComponent.SOLID_WALL },
            { RoomDirection.SOUTH, RoomComponent.SOLID_WALL },
            { RoomDirection.EAST, RoomComponent.SOLID_WALL },
            { RoomDirection.WEST, RoomComponent.SOLID_WALL }
        };

    /// <summary>
    /// Method to distinguish if a room is adjacent to this room 
    /// and if so which direction it is in
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool IsAdjacent(RoomInfo otherRoom, out RoomDirection direction)
    {
        if (IsAdjacent(otherRoom))
        {
            direction = GetRelativeDirection(otherRoom);
            return true;
        }
        direction = 0;
        return false;
    }
    public bool IsAdjacent(RoomInfo otherRoom)
    {
        return IsNextTo(otherRoom);
    }

    /// <summary>
    /// Method that returns wether a room is next to this room
    /// non diagonally, only in plus shape
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <returns></returns>
    private bool IsNextTo(RoomInfo otherRoom)
    {
        return (otherRoom.X.OneGreaterOrLessThan(this.X) && otherRoom.Z == this.X) ||
            (otherRoom.X == this.X && otherRoom.Z.OneGreaterOrLessThan(this.Z));
    }

    /// <summary>
    /// Returns the direction otherRoom is relative to the invoking object.
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <returns>RoomDirection</returns>
    private RoomDirection GetRelativeDirection(RoomInfo otherRoom)
    {
        RoomDirection direction = default(RoomDirection);
        if (otherRoom.Z < Z) direction |= RoomDirection.SOUTH;
        if (otherRoom.Z > Z) direction |= RoomDirection.NORTH;
        if (otherRoom.X < X) direction |= RoomDirection.WEST;
        if (otherRoom.X > X) direction |= RoomDirection.EAST;
        return direction;
    }
}
