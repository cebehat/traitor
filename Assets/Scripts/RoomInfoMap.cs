using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RoomInfoMap : INetworkVariable
{
    private List<RoomInfo> _roomInfos;

    public ushort RemoteTick => throw new NotImplementedException();

    public RoomInfoMap()
    {
        _roomInfos = new List<RoomInfo>();
    }

    public bool CreateRoomNextTo(RoomInfo originRoom, RoomDirection roomDirection)
    {
        if (!GetSpawnableDirections(originRoom).HasFlag(roomDirection)) return false;

        coordinates newCoords = GetCoordinatesForAdjacent(originRoom, roomDirection);
        int X = newCoords.X;
        int Z = newCoords.Z;
        
        var newRoom = new RoomInfo()
        {
            X = X,
            Z = Z
        };

        _roomInfos.Add(newRoom);
        return true;
    }

    public void Add(RoomInfo roomInfo)
    {
        _roomInfos.Add(roomInfo);
    }

    public void AddRange(IEnumerable<RoomInfo> rooms)
    {
        _roomInfos.AddRange(rooms);
    }

    public RoomInfo GetRoomAtPosition(int X, int Z)
    {
        return _roomInfos.SingleOrDefault(x => x.X == X && x.Z == Z);
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

    public IEnumerable<RoomInfo> GetUnspawnedRooms()
    {
        return _roomInfos.Where(x => !x.IsSpawned);
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

    public NetworkChannel GetChannel()
    {
        throw new NotImplementedException();
    }

    public void ResetDirty()
    {
        throw new NotImplementedException();
    }

    public bool IsDirty()
    {
        throw new NotImplementedException();
    }

    public bool CanClientWrite(ulong clientId)
    {
        throw new NotImplementedException();
    }

    public bool CanClientRead(ulong clientId)
    {
        throw new NotImplementedException();
    }

    public void WriteDelta(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void WriteField(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void ReadField(Stream stream, ushort localTick, ushort remoteTick)
    {
        throw new NotImplementedException();
    }

    public void ReadDelta(Stream stream, bool keepDirtyDelta, ushort localTick, ushort remoteTick)
    {
        throw new NotImplementedException();
    }

    public void SetNetworkBehaviour(NetworkBehaviour behaviour)
    {
        throw new NotImplementedException();
    }

    private class coordinates
    {
        public int X { get; set; }
        public int Z { get; set; }
    }
}
