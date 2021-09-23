using System.Collections;
using UnityEngine;

namespace Cebt.Shared
{
    public static class RoomHelper
    {
        public static string GetRoomId(int X, int Z, int Floor)
        {
            return string.Format("{0}:{0}:{0}", X, Z, Floor);
        } 

      
    }
}