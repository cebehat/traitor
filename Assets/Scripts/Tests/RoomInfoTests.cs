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
        return new RoomInfo(0, 0, 0, RoomType.GENERIC, null);
    }

    [Test]
    public void RoomInfoTestIsAdjacent()
    {
        var originRoom = GetOriginTestRoom();
        var southOriginRoom = new RoomInfo(0, -1, 0, RoomType.GENERIC, null);
        //var northRoom = new RoomInfoWrapper() { X = 0, Z = 1 };
        var northRoom = new RoomInfo(0, 1, 0, RoomType.GENERIC, null);
        //var southRoom = new RoomInfoWrapper() { X = 0, Z = -1 };
        var southRoom = new RoomInfo(0, -1, 0, RoomType.GENERIC, null);
        //var eastRoom = new RoomInfoWrapper() { X = 1, Z = 0 };
        var eastRoom = new RoomInfo(1, 0, 0, RoomType.GENERIC, null);
        //var westRoom = new RoomInfoWrapper() { X = -1, Z = 0 };
        var westRoom = new RoomInfo(-1, 0, 0, RoomType.GENERIC, null);


        RoomDirection direction = 0;
        //test north room;
        Assert.IsTrue(originRoom.IsAdjacent(northRoom, out direction));
        Assert.AreEqual(RoomDirection.NORTH, direction, "Direction was wrong");
        Assert.IsFalse(southOriginRoom.IsAdjacent(northRoom, out direction));
        //test south room;
        Assert.IsTrue(originRoom.IsAdjacent(southRoom, out direction));
        Assert.AreEqual(RoomDirection.SOUTH, direction, "Direction was wrong");
        //test east room;
        Assert.IsTrue(originRoom.IsAdjacent(eastRoom, out direction));
        Assert.AreEqual(RoomDirection.EAST, direction, "Direction was wrong");
        Assert.IsFalse(southOriginRoom.IsAdjacent(eastRoom, out direction));
        //test west room;
        Assert.IsTrue(originRoom.IsAdjacent(westRoom, out direction));
        Assert.AreEqual(RoomDirection.WEST, direction, "Direction was wrong");
        Assert.IsFalse(southOriginRoom.IsAdjacent(westRoom, out direction));
    }
    [Test]
    public void RoomInfoTestIsNotAdjacent()
    {
        var originRoom = GetOriginTestRoom();
        var northRoom = new RoomInfo(0, 2, 0, RoomType.GENERIC, null);
        var southRoom = new RoomInfo(0, -2, 0, RoomType.GENERIC, null);
        var eastRoom = new RoomInfo(2, 0, 0, RoomType.GENERIC, null);
        var westRoom = new RoomInfo(-2, 0, 0, RoomType.GENERIC, null);

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
        var originPosition = new Vector3(0, 0, 0);
        var originRoom = GetOriginTestRoom();
        Assert.AreEqual(originPosition, originRoom.TransformPosition);

        var northEastPosition = new Vector3(10f, 0f, 30f);
        var northEastRoom = new RoomInfo(1, 3, 0, RoomType.GENERIC, null);
        Assert.AreEqual(northEastPosition, northEastRoom.TransformPosition);
    }
}
