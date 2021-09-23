using Cebt.Shared;
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Cebt.RoomData
{
    public struct NetworkedRoomInfo : INetworkSerializable, IEquatable<NetworkedRoomInfo>
    {
        public int X;
        public int Z;
        public int Floor;
        public RoomType RoomType;

        public int NorthWall;
        public int SouthWall;
        public int EastWall;
        public int WestWall;

        //public string roomId;

        public NetworkedRoomInfo(int x, int z, int floor, RoomType roomType, Dictionary<RoomDirection, RoomComponent> Walls)
        {
            X = x;
            Z = z;
            Floor = floor;

            RoomType = roomType;

            if(Walls == null)
            {
                Walls = new Dictionary<RoomDirection, RoomComponent>();
            }

            SouthWall = (int)(Walls.ContainsKey(RoomDirection.SOUTH) ? Walls[RoomDirection.SOUTH] : RoomComponent.NO_WALL);
            NorthWall = (int)(Walls.ContainsKey(RoomDirection.NORTH) ? Walls[RoomDirection.NORTH] : RoomComponent.NO_WALL);
            EastWall = (int)(Walls.ContainsKey(RoomDirection.EAST) ? Walls[RoomDirection.EAST] : RoomComponent.NO_WALL);
            WestWall = (int)(Walls.ContainsKey(RoomDirection.WEST) ? Walls[RoomDirection.WEST] : RoomComponent.NO_WALL);
        }

        public bool Equals(NetworkedRoomInfo other)
        {
            return X == other.X &&
                Z == other.Z &&
                Floor == other.Floor &&
                SouthWall == other.SouthWall &&
                NorthWall == other.NorthWall &&
                EastWall == other.EastWall &&
                WestWall == other.WestWall;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref X);
            serializer.SerializeValue(ref Z);
            serializer.SerializeValue(ref Floor);
            serializer.SerializeValue(ref RoomType);
            serializer.SerializeValue(ref NorthWall);
            serializer.SerializeValue(ref SouthWall);
            serializer.SerializeValue(ref EastWall);
            serializer.SerializeValue(ref WestWall);
        }
    }
}
