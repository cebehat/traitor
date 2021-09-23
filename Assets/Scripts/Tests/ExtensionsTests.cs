//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Cebt.Shared;
//using NUnit.Framework;
//using UnityEngine;
//using UnityEngine.TestTools;

//public class ExtensionsTests
//{
//    // A Test behaves as an ordinary method
//    [Test]
//    public void RoomDirectionAllOthersTestSuccess()
//    {
//        var direction = RoomDirection.NORTH;
//        var expectedResult = (new List<RoomDirection> { RoomDirection.SOUTH, RoomDirection.EAST, RoomDirection.WEST }).OrderBy(x => x);

//        var result = direction.AllOthers();

//        Assert.AreEqual(expectedResult, result);

//        direction = RoomDirection.EAST;
//        expectedResult = (new List<RoomDirection> { RoomDirection.SOUTH, RoomDirection.NORTH, RoomDirection.WEST }).OrderBy(x => x);
//        result = direction.AllOthers();
//        Assert.AreEqual(expectedResult, result);
//    }

//    [Test]
//    public void RoomDirectionOppositeDirectionTestSuccess()
//    {
//        Assert.AreEqual(RoomDirection.NORTH, RoomDirection.SOUTH.OppositeDirection());
//        Assert.AreEqual(RoomDirection.SOUTH, RoomDirection.NORTH.OppositeDirection());
//        Assert.AreEqual(RoomDirection.EAST, RoomDirection.WEST.OppositeDirection());
//        Assert.AreEqual(RoomDirection.WEST, RoomDirection.EAST.OppositeDirection());
//    }

//    [Test]
//    public void IntOneGreaterOrLessThanTest()
//    {
//        Assert.IsTrue((1).OneGreaterOrLessThan(2));
//        Assert.IsTrue((3).OneGreaterOrLessThan(2));
//        Assert.IsTrue((1).OneGreaterOrLessThan(0));
//        Assert.IsTrue((-1).OneGreaterOrLessThan(-2));
//        Assert.IsFalse((1).OneGreaterOrLessThan(3));
//        Assert.IsFalse((1).OneGreaterOrLessThan(-1));
//        Assert.IsFalse((0).OneGreaterOrLessThan(2));
//        Assert.IsFalse((-4).OneGreaterOrLessThan(5));
//    }
//}
