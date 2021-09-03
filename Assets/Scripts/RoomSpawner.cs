using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;
using System.Threading;

public class RoomSpawner : NetworkBehaviour
{
    public NetworkList<Room> spawnedRooms = new NetworkList<Room>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    //public NetworkVariable<List<Room>> spawnedRooms = new NetworkVariable<List<Room>>(new NetworkVariableSettings
    //{
    //    WritePermission = NetworkVariablePermission.ServerOnly,
    //    ReadPermission = NetworkVariablePermission.Everyone
    //});

    [ServerRpc]
    void AddRoomToHouseServerRpc(string roomJSON, ServerRpcParams rpcParams = default)
    {
        Debug.Log(rpcParams.ToString());
        try
        {
            Room room = JsonUtility.FromJson<Room>(roomJSON);
            spawnedRooms.Add(room);
        }
        catch (Exception e)
        {

        }
        
    }

    void AddRoomToHouse(Room room)
    {
        AddRoomToHouseServerRpc(JsonUtility.ToJson(room));
    }


    public void Start()
    {
        NetworkManager.Singleton.OnServerStarted += ServerStarted;
    }
    // Start is called before the first frame update
    public override void NetworkStart()
    {
        
        //if(NetworkManager.Singleton.)
        Debug.Log("NetworkStart");
        
    }

    private void ServerStarted()
    {

        var originRoomNorth = new Room();
        originRoomNorth.x = 0;
        originRoomNorth.z = 1;
        originRoomNorth.Walls[RoomDirection.NORTH] = RoomComponent.DOORWAY;
        originRoomNorth.Walls[RoomDirection.SOUTH] = RoomComponent.NO_WALL;
        originRoomNorth.Walls[RoomDirection.EAST] = RoomComponent.DOORWAY;
        originRoomNorth.Walls[RoomDirection.WEST] = RoomComponent.DOORWAY;
        originRoomNorth.roomType = RoomType.FOYER;
        AddRoomToHouse(originRoomNorth);

        var originRoomSouth = new Room();
        originRoomSouth.x = 0;
        originRoomSouth.z = 0;
        originRoomSouth.Walls[RoomDirection.NORTH] = RoomComponent.NO_WALL;
        originRoomSouth.Walls[RoomDirection.SOUTH] = RoomComponent.DECOY_DOOR;
        originRoomSouth.roomType = RoomType.FOYER;
        AddRoomToHouse(originRoomSouth);
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
        newRoom.Walls[GetOppositeDirection(direction)] = room.Walls[direction] == RoomComponent.DOORWAY ? RoomComponent.DOOR : (room.Walls[GetOppositeDirection(direction)] == RoomComponent.DOOR ? RoomComponent.DOORWAY : room.Walls[GetOppositeDirection(direction)]);
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
                newRoom.Walls[dir] = RoomComponent.DOOR;
            }
            else if (ar != null && (ar.Walls[GetOppositeDirection(dir)] == RoomComponent.DOORWAY || ar.Walls[GetOppositeDirection(dir)] == RoomComponent.NO_WALL))
            {
                newRoom.Walls[dir] = ar.Walls[GetOppositeDirection(dir)];
            }
        }
        //newRoom.SpawnRoom();
        //AddRoomToHouse(newRoom);
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
            if (ShouldGenerateRoom(room.Walls[dir]))
            {
                //GenerateAdjacentRoom(room, dir);
            }
        }
    }

    private bool ShouldGenerateRoom(RoomComponent roomComponent)
    {
        var passageWays = new List<RoomComponent> { RoomComponent.DOORWAY, RoomComponent.NO_WALL, RoomComponent.DOOR };
        return passageWays.Contains(roomComponent);
    }
}
