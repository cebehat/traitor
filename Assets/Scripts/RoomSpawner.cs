using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    List<Room> spawnedRooms = new List<Room>();

    // Start is called before the first frame update
    void Start()
    {
        var originRoomNorthObject = new GameObject(String.Format("Room [{0}:{1}]", 0, 0));
        var originRoomNorth = originRoomNorthObject.AddComponent<Room>();
        originRoomNorth.x = 0;
        originRoomNorth.z = 0;
        originRoomNorth.Walls[RoomDirection.NORTH] = RoomComponent.DOORWAY;
        originRoomNorth.Walls[RoomDirection.SOUTH] = RoomComponent.NO_WALL;
        originRoomNorth.Walls[RoomDirection.EAST] = RoomComponent.DOORWAY;
        originRoomNorth.Walls[RoomDirection.WEST] = RoomComponent.DOORWAY;
        originRoomNorthObject.name = String.Format("Room [{0}:{1}]", 0, 0);
        originRoomNorth.roomType = RoomType.FOYER;
        originRoomNorth.SpawnRoom();
        spawnedRooms.Add(originRoomNorth);
        
        
        var originRoomSouthObject = new GameObject(String.Format("Room [{0}:{1}]", 0, -1));
        originRoomSouthObject.transform.position -= new Vector3(0f, 0f, 10f);
        var originRoomSouth = originRoomSouthObject.AddComponent<Room>();
        originRoomSouth.x = 0;
        originRoomSouth.z = -1;
        originRoomSouth.Walls[RoomDirection.NORTH] = RoomComponent.NO_WALL;
        originRoomSouth.Walls[RoomDirection.SOUTH] = RoomComponent.DECOY_DOOR;
        originRoomSouthObject.name = String.Format("Room [{0}:{1}]", 0, -1);
        originRoomSouth.roomType = RoomType.FOYER;
        originRoomSouth.SpawnRoom();
        spawnedRooms.Add(originRoomSouth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Room> GetAdjacentRooms(Room room)
    {
        return spawnedRooms.Where(r => room.IsAdjacent(r)).ToList();
    }

    RoomDirection GetOppositeDirection(RoomDirection dir)
    {
        if (dir == RoomDirection.NORTH) return RoomDirection.SOUTH;
        if (dir == RoomDirection.SOUTH) return RoomDirection.NORTH;
        if (dir == RoomDirection.EAST) return RoomDirection.WEST;
        return RoomDirection.EAST;
    }

    void GenerateAdjacentRoom(Room room, RoomDirection direction)
    {
        int x = room.x;
        int z = room.z;
        var newRoomObject = new GameObject();
        var newRoom = newRoomObject.AddComponent<Room>();



        switch (direction)
        {
            case RoomDirection.NORTH:
                z++;
                newRoom.Walls[direction] = room.Walls[GetOppositeDirection(direction)];
                break;
            case RoomDirection.SOUTH:
                z--;
                newRoom.Walls[direction] = room.Walls[GetOppositeDirection(direction)];
                break;
            case RoomDirection.EAST:
                x++;
                newRoom.Walls[direction] = room.Walls[GetOppositeDirection(direction)];
                break;
            case RoomDirection.WEST:
                x--;
                newRoom.Walls[direction] = room.Walls[GetOppositeDirection(direction)];
                break;
            default:
                break;
        }
        newRoom.x = x;
        newRoom.z = z;
        newRoomObject.name = String.Format("Room [{0}:{1}]", x, z);
        newRoomObject.transform.position = new Vector3(10f * x, 0f, 10f * z);
        var availableSpawnDirs = GetAvailableSpawnDirectionsForRoom(newRoom);
        for (int i = 0; i < 4; i++)
        {
            var dir = (RoomDirection)i;
            var ar = GetAdjacentRoom(newRoom, dir);

            if (availableSpawnDirs.Contains(dir) && (UnityEngine.Random.Range(0f, 1f) < 0.7f))
            {
                newRoom.Walls[dir] = RoomComponent.DOORWAY;
            }
            else if (ar != null && (ar.Walls[GetOppositeDirection(dir)] == RoomComponent.DOORWAY || ar.Walls[GetOppositeDirection(dir)] == RoomComponent.NO_WALL))
            {
                newRoom.Walls[dir] = ar.Walls[GetOppositeDirection(dir)];
            }
        }
        newRoom.SpawnRoom();
        spawnedRooms.Add(newRoom);
    }

    public Room GetAdjacentRoom(Room relativeRoom, RoomDirection dir)
    {
        int x = relativeRoom.x;
        int z = relativeRoom.z;
        switch (dir)
        {
            case RoomDirection.NORTH:
                z++;
                break;
            case RoomDirection.SOUTH:
                z--;
                break;
            case RoomDirection.EAST:
                x++;
                break;
            case RoomDirection.WEST:
                x--;
                break;
            default:
                break;
        }
        return spawnedRooms.SingleOrDefault(sr => sr.x == x && sr.z == z);
    }

    public List<RoomDirection> GetAvailableSpawnDirectionsForRoom(Room room)
    {
        var adjacentRooms = GetAdjacentRooms(room);
        return Enum.GetValues(typeof(RoomDirection))
                .Cast<RoomDirection>()
                .Except(adjacentRooms.Select(ar => room.GetRelativeDirection(ar)))
                .ToList();
    }

    public List<RoomDirection> GetAlreadyExistingAdjacentRooms(Room room)
    {
        var adjacentRooms = GetAdjacentRooms(room);
        return adjacentRooms.Select(ar => room.GetRelativeDirection(ar))
                .ToList();
    }

    public void SpawnRooms(GameObject originRoom)
    {
        var room = originRoom.GetComponent<Room>();
        var adjacentRoomDirections = GetAvailableSpawnDirectionsForRoom(room);

        foreach(var dir in adjacentRoomDirections)
        {
            if (room.Walls[dir] == RoomComponent.DOORWAY || room.Walls[dir] == RoomComponent.NO_WALL)
            {
                GenerateAdjacentRoom(room, dir);
            }
        }
    }
}
