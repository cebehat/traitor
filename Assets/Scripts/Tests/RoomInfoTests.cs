using System.Collections;
using System.Collections.Generic;
using Cebt.RoomData;
using Cebt.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoomInfoTests
{
    private RoomInfo GetOriginTestRoom()
    {
        return new RoomInfo()
        {
            X = 0,
            Z = 0,
        };
    }

    [Test]
    public void RoomInfoTestIsAdjacent()
    {
        var originRoom = GetOriginTestRoom();
        var northRoom = new RoomInfo() { X = 0, Z = 1 };
        var southRoom = new RoomInfo() { X = 0, Z = -1 };
        var eastRoom = new RoomInfo() { X = 1, Z = 0 };
        var westRoom = new RoomInfo() { X = -1, Z = 0 };

        RoomDirection direction = 0;
        //test north room;
        Assert.IsTrue(originRoom.IsAdjacent(northRoom, out direction));
        Assert.AreEqual(RoomDirection.NORTH, direction, "Direction was wrong");
        //test south room;
        Assert.IsTrue(originRoom.IsAdjacent(southRoom, out direction));
        Assert.AreEqual(RoomDirection.SOUTH, direction, "Direction was wrong");
        //test east room;
        Assert.IsTrue(originRoom.IsAdjacent(eastRoom, out direction));
        Assert.AreEqual(RoomDirection.EAST, direction, "Direction was wrong");
        //test west room;
        Assert.IsTrue(originRoom.IsAdjacent(westRoom, out direction));
        Assert.AreEqual(RoomDirection.WEST, direction, "Direction was wrong");
    }
    [Test]
    public void RoomInfoTestIsNotAdjacent()
    {
        var originRoom = GetOriginTestRoom();
        var northRoom = new RoomInfo() { X = 0, Z = 2 };
        var southRoom = new RoomInfo() { X = 0, Z = -2 };
        var eastRoom = new RoomInfo() { X = 2, Z = 0 };
        var westRoom = new RoomInfo() { X = -2, Z = 0 };

        //test north room;
        Assert.IsFalse(originRoom.IsAdjacent(northRoom));
        //test south room;
        Assert.IsFalse(originRoom.IsAdjacent(southRoom));
        //test east room;
        Assert.IsFalse(originRoom.IsAdjacent(eastRoom));
        //test west room;
        Assert.IsFalse(originRoom.IsAdjacent(westRoom));
    }

    [Test]
    public void RoomInfoTestTransformPosition()
    {
        var originPosition = new Vector3(0,0,0);
        var originRoom = GetOriginTestRoom();
        Assert.AreEqual(originPosition, originRoom.TransformPosition);

        var northEastPosition = new Vector3(10f, 0f, 30f);
        var northEastRoom = new RoomInfo() { X = 1, Z = 3 };
        Assert.AreEqual(northEastPosition, northEastRoom.TransformPosition);
    }
}
