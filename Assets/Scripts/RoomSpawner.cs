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
using Cebt.RoomData;
using Cebt.Shared;

public class RoomSpawner : NetworkBehaviour
{

    #region Server-stuff

    public NetworkVariable<RoomInfoMap> roomInfoMap = new NetworkVariable<RoomInfoMap>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    [ServerRpc]
    void AddRoomToHouseServerRpc(string roomJSON, ServerRpcParams rpcParams = default)
    {
        Debug.Log(rpcParams.ToString());
        try
        {
            RoomInfo room = JsonUtility.FromJson<RoomInfo>(roomJSON);
            roomInfoMap.Value.Add(room);
        }
        catch (Exception e)
        {

        }
        
    }

    [ServerRpc]
    void SetRoomAsSpawnedServerRpc(int X, int Z)
    {
        roomInfoMap.Value.GetRoomAtPosition(X, Z).IsRendered = true;
    }

    #endregion

    void AddRoomToHouse(RoomInfo room)
    {
        Debug.Log("Adding room to house");
        AddRoomToHouseServerRpc(JsonUtility.ToJson(room));
    }

    void SpawnRoom(RoomInfo room)
    {
        //spawn the room prefab with the roomInfo object
        SetRoomAsSpawnedServerRpc(room.X, room.Z);
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
        if (NetworkManager.Singleton.IsServer)
        {
            roomInfoMap.Value = new RoomInfoMap();
        }
        
    }

    private void ServerStarted()
    {

        var originRoomNorth = new RoomInfo();
        originRoomNorth.X = 0;
        originRoomNorth.Z = 1;
        originRoomNorth.Walls[RoomDirection.NORTH] = RoomComponent.DOORWAY;
        originRoomNorth.Walls[RoomDirection.SOUTH] = RoomComponent.NO_WALL;
        originRoomNorth.Walls[RoomDirection.EAST] = RoomComponent.DOORWAY;
        originRoomNorth.Walls[RoomDirection.WEST] = RoomComponent.DOORWAY;
        originRoomNorth.RoomType = RoomType.FOYER;
        AddRoomToHouse(originRoomNorth);

        var originRoomSouth = new RoomInfo();
        originRoomSouth.X = 0;
        originRoomSouth.Z = 0;
        originRoomSouth.Walls[RoomDirection.NORTH] = RoomComponent.NO_WALL;
        originRoomSouth.Walls[RoomDirection.SOUTH] = RoomComponent.DECOY_DOOR;
        originRoomSouth.RoomType = RoomType.FOYER;
        AddRoomToHouse(originRoomSouth);
    }

    // Update is called once per frame
    public void Update()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            var unspawnedRooms = roomInfoMap.Value.GetUnspawnedRooms();
            if (unspawnedRooms.Any())
            {
                foreach (var item in unspawnedRooms)
                {
                    SpawnRoom(item);
                }
            }
        }
        
    }

    //void GenerateAdjacentRoom(Room room, RoomDirection direction)
    //{
    //    int x = room.x;
    //    int z = room.z;
    //    var newRoomObject = new GameObject();
    //    var newRoom = newRoomObject.AddComponent<Room>();



    //    switch (direction)
    //    {
    //        case RoomDirection.NORTH:
    //            z++;
    //            break;
    //        case RoomDirection.SOUTH:
    //            z--;
    //            break;
    //        case RoomDirection.EAST:
    //            x++;
    //            break;
    //        case RoomDirection.WEST:
    //            x--; 
    //            break;
    //        default:
    //            break;
    //    }
    //    newRoom.Walls[GetOppositeDirection(direction)] = room.Walls[direction] == RoomComponent.DOORWAY ? RoomComponent.DOOR : (room.Walls[GetOppositeDirection(direction)] == RoomComponent.DOOR ? RoomComponent.DOORWAY : room.Walls[GetOppositeDirection(direction)]);
    //    newRoom.x = x;
    //    newRoom.z = z;
    //    newRoomObject.name = String.Format("Room [{0}:{1}]", x, z);
    //    newRoomObject.transform.position = new Vector3(10f * x, 0f, 10f * z);
    //    var availableSpawnDirs = GetAvailableSpawnDirectionsForRoom(newRoom);
    //    for (int i = 0; i < 4; i++)
    //    {
    //        var dir = (RoomDirection)i;
    //        var ar = GetAdjacentRoom(newRoom, dir);

    //        if (availableSpawnDirs.Contains(dir) && (UnityEngine.Random.Range(0f, 1f) < 0.7f))
    //        {
    //            newRoom.Walls[dir] = RoomComponent.DOOR;
    //        }
    //        else if (ar != null && (ar.Walls[GetOppositeDirection(dir)] == RoomComponent.DOORWAY || ar.Walls[GetOppositeDirection(dir)] == RoomComponent.NO_WALL))
    //        {
    //            newRoom.Walls[dir] = ar.Walls[GetOppositeDirection(dir)];
    //        }
    //    }
    //    //newRoom.SpawnRoom();
    //    //AddRoomToHouse(newRoom);
    //}

    public RoomInfo GetAdjacentRoom(RoomInfo relativeRoom, RoomDirection dir)
    {
        int x = relativeRoom.X;
        int z = relativeRoom.Z;
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
        return roomInfoMap.Value.GetRoomAtPosition(x, z);
    }

    //public List<RoomDirection> GetAvailableSpawnDirectionsForRoom(RoomInfo room)
    //{
    //    var adjacentRooms = roomInfoMap.Value.GetAdjacentRooms(room);
    //    return Enum.GetValues(typeof(RoomDirection))
    //            .Cast<RoomDirection>()
    //            .Except(adjacentRooms.Select(ar => room.GetRelativeDirection(ar)))
    //            .ToList();
    //}

    //public List<RoomDirection> GetAlreadyExistingAdjacentRooms(Room room)
    //{
    //    var adjacentRooms = GetAdjacentRooms(room);
    //    return adjacentRooms.Select(ar => room.GetRelativeDirection(ar))
    //            .ToList();
    //}

    public void SpawnRooms(GameObject originRoom)
    {
        var roomInfo = originRoom.GetComponent<Room>().RoomInfo;
        //var adjacentRoomDirections = GetAvailableSpawnDirectionsForRoom(room);


        //foreach(var dir in roomInfoMap.Value.GetSpawnableDirections(roomInfo))
        //{
        //    if (ShouldGenerateRoom(room.Walls[dir]))
        //    {
        //        //GenerateAdjacentRoom(room, dir);
        //    }
        //}
    }

    private bool ShouldGenerateRoom(RoomComponent roomComponent)
    {
        var passageWays = new List<RoomComponent> { RoomComponent.DOORWAY, RoomComponent.NO_WALL, RoomComponent.DOOR };
        return passageWays.Contains(roomComponent);
    }
}
