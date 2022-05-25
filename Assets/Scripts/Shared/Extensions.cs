using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cebt.Shared
{
    public static class Extensions
    {
        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }

        public static bool OneGreaterOrLessThan(this int val, int val2)
        {
            return (val + 1) == val2 || (val - 1) == val2;
        }

        public static RoomDirection OppositeDirection(this RoomDirection dir)
        {
            if (dir == RoomDirection.NORTH) return RoomDirection.SOUTH;
            else if (dir == RoomDirection.SOUTH) return RoomDirection.NORTH;
            else if (dir == RoomDirection.WEST) return RoomDirection.EAST;
            else return RoomDirection.WEST;
        }

        public static IEnumerable<RoomDirection> AllOthers(this RoomDirection dir)
        {
            return Enum.GetValues(typeof(RoomDirection)).OfType<RoomDirection>().Where(x => x != dir).OrderBy(x=>x);
        }
    } 
}
