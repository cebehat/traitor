using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    //X is the east/west coordinate
    public int X { get; set; }
    //Z is the north/south coordinate
    public int Z { get; set; }
    public RoomType RoomType { get; set; }
    public Dictionary<RoomDirection, RoomComponent> Walls { get; set; }
        = new Dictionary<RoomDirection, RoomComponent> {
            { RoomDirection.NORTH, RoomComponent.SOLID_WALL },
            { RoomDirection.SOUTH, RoomComponent.SOLID_WALL },
            { RoomDirection.EAST, RoomComponent.SOLID_WALL },
            { RoomDirection.WEST, RoomComponent.SOLID_WALL }
        };


    public bool IsNorthOf(RoomInfo otherRoomInfo)
    {
        return otherRoomInfo.Z > this.Z;
    }

    public bool IsEastOf(RoomInfo otherRoomInfo)
    {
        return otherRoomInfo.X > this.X;
    }
}
