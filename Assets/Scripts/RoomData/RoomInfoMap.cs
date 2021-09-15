using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cebt.Shared;


namespace Cebt.RoomData
{
    public class RoomInfoMap
    {

        private List<RoomInfo> _roomInfos;

        public int Count { get { return _roomInfos.Count; } }

        public RoomInfoMap()
        {
            _roomInfos = new List<RoomInfo>();
        }

        //public bool CreateRoomNextTo(RoomInfo originRoom, RoomDirection roomDirection)
        //{
        //    if (!GetSpawnableDirections(originRoom).HasFlag(roomDirection)) return false;

        //    coordinates newCoords = GetCoordinatesForAdjacent(originRoom, roomDirection);
        //    int X = newCoords.X;
        //    int Z = newCoords.Z;

        //    var newRoom = new RoomInfo()
        //    {
        //        X = X,
        //        Z = Z
        //    };

        //    _roomInfos.Add(newRoom);
        //    return true;
        //}

        public bool TryAddRoom(RoomInfo roomInfo)
        {
            if(GetRoomAtPosition(roomInfo.X, roomInfo.Z, roomInfo.FloorNumber) == null)
            {
                _roomInfos.Add(roomInfo);
                return true;
            }
            return false;
        }

        public void AddRange(IEnumerable<RoomInfo> rooms)
        {
            _roomInfos.AddRange(rooms);
        }

        public RoomInfo GetRoomAtPosition(int X, int Z, int Floor)
        {
            return _roomInfos.SingleOrDefault(x => x.X == X && x.Z == Z && x.FloorNumber == Floor);
        }

        public Dictionary<RoomDirection, RoomInfo> GetAdjacentRooms(RoomInfo room)
        {
            Dictionary<RoomDirection, RoomInfo> pairs = new Dictionary<RoomDirection, RoomInfo>();
            var adjacent = _roomInfos.Where(x => room.IsAdjacent(x));
            foreach (var item in adjacent)
            {
                RoomDirection dir;
                room.IsAdjacent(item, out dir);
                pairs.Add(dir, item);
            }
            return pairs;
        }

        public RoomDirection GetSpawnableDirections(RoomInfo room)
        {
            RoomDirection dir = 0;
            var adjacent = GetAdjacentRooms(room).Keys.ToList();
            adjacent.ForEach(x => dir = dir | x);
            RoomDirection allDirections = RoomDirection.NORTH | RoomDirection.SOUTH | RoomDirection.EAST | RoomDirection.WEST;
            return allDirections & ~dir;
        }

        private bool HasSpawnableDirections(RoomInfo room)
        {
            var adjacent = GetAdjacentRooms(room);
            foreach(var direction in room.OpenDoorways)
            {
                if (!adjacent.ContainsKey(direction)) return true;
            }
            return false;
        }

        public IEnumerable<RoomInfo> GetRoomsWithSpawnableDirections()
        {
            return _roomInfos.Where(x => HasSpawnableDirections(x));
        }

        public IEnumerable<RoomInfo> GetUnspawnedRooms()
        {
            return _roomInfos.Where(x => !x.IsRendered);
        }

        private coordinates GetCoordinatesForAdjacent(RoomInfo room, RoomDirection direction)
        {
            coordinates returnCoords = new coordinates() { X = room.X, Z = room.Z };
            switch (direction)
            {
                case RoomDirection.NORTH:
                    returnCoords.Z++;
                    break;
                case RoomDirection.SOUTH:
                    returnCoords.Z--;
                    break;
                case RoomDirection.EAST:
                    returnCoords.X++;
                    break;
                case RoomDirection.WEST:
                    returnCoords.X--;
                    break;
                default:
                    break;
            }
            return returnCoords;
        }

        private class coordinates
        {
            public int X { get; set; }
            public int Z { get; set; }
        }
    }
}