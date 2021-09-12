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

    public float MaxRooms { get; set; } = 100;
    public float MinRooms { get; set; } = 30;

    public bool DevMode { get; set; } = true;

    public RoomInfoMap roomInfoMap = new RoomInfoMap();

    private bool IsOnlineGame = false;



    #region Server-stuff

    public NetworkVariable<RoomInfoMap> networkedRoomInfoMap = new NetworkVariable<RoomInfoMap>(new NetworkVariableSettings
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
            networkedRoomInfoMap.Value.TryAddRoom(room);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

    [ServerRpc]
    void SetRoomAsSpawnedServerRpc(int X, int Z, int Floor)
    {
        networkedRoomInfoMap.Value.GetRoomAtPosition(X, Z, Floor).IsRendered = true;
    }

    #endregion

    #region Unity-Events
    // Start is called before the first frame update
    public void Start()
    {
        SpawnEntryHall();
        //if (!DevMode)
        //{
        //    NetworkManager.Singleton.OnServerStarted += ServerStarted;
        //}
        //else
        //{
        //    GenerateMap();
        //}
    }

    // Update is called once per frame
    public void Update()
    {
        IEnumerable<RoomInfo> unspawnedRooms = new List<RoomInfo>();
        if (DevMode)
        {
            //Debug.Log("devmode client update");
            unspawnedRooms = roomInfoMap.GetUnspawnedRooms();            
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            unspawnedRooms = networkedRoomInfoMap.Value.GetUnspawnedRooms();
        }

        if (unspawnedRooms.Any())
        {
            foreach (var item in unspawnedRooms)
            {
                SpawnRoom(item);
            }
        }
    }

    public void OnEnable()
    {
        
    }

    public override void NetworkStart()
    {

        //if(NetworkManager.Singleton.)
        Debug.Log("NetworkStart");
        if (NetworkManager.Singleton.IsServer)
        {
            networkedRoomInfoMap.Value = new RoomInfoMap();
        }

    }

    #endregion

    void AddRoomToHouse(RoomInfo room)
    {
        Debug.Log("Adding room to house");
        if (IsOnlineGame)
        {
            AddRoomToHouseServerRpc(JsonUtility.ToJson(room));
        }
        else
        {
            roomInfoMap.TryAddRoom(room);
        }
        
    }

    void SetRoomAsSpawned(RoomInfo room)
    {
        Debug.Log("Adding room to house");
        if (IsOnlineGame)
        {
            SetRoomAsSpawnedServerRpc(room.X, room.Z, room.FloorNumber);
        }
        else
        {
            roomInfoMap.GetRoomAtPosition(room.X, room.Z, room.FloorNumber).IsRendered = true;
        }
    }

    void SpawnRoom(RoomInfo room)
    {
        try
        {
            var roomObject = new GameObject("test");
            var roomComponent = roomObject.AddComponent<Room>();
            roomComponent.RoomInfo = room;
            roomObject.transform.position = room.TransformPosition;
            roomComponent.SpawnRoom();
            SetRoomAsSpawned(room);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        
        //spawn the room prefab with the roomInfo object
        
    }


    

    private void ServerStarted()
    {

        
    }

    private void SpawnEntryHall()
    {
        var foyerNorth = new RoomInfo();
        foyerNorth.X = 0;
        foyerNorth.Z = 1;
        foyerNorth.Walls[RoomDirection.NORTH] = RoomComponent.DOORWAY;
        foyerNorth.Walls[RoomDirection.SOUTH] = RoomComponent.NO_WALL;
        foyerNorth.Walls[RoomDirection.EAST] = RoomComponent.DOORWAY;
        foyerNorth.Walls[RoomDirection.WEST] = RoomComponent.DOORWAY;
        foyerNorth.RoomType = RoomType.FOYER;
        AddRoomToHouse(foyerNorth);

        var foyerSouth = new RoomInfo();
        foyerSouth.X = 0;
        foyerSouth.Z = 0;
        foyerSouth.Walls[RoomDirection.NORTH] = RoomComponent.NO_WALL;
        foyerSouth.Walls[RoomDirection.SOUTH] = RoomComponent.DECOY_DOOR;
        foyerSouth.Walls[RoomDirection.EAST] = RoomComponent.SOLID_WALL;
        foyerSouth.Walls[RoomDirection.WEST] = RoomComponent.SOLID_WALL;
        foyerSouth.RoomType = RoomType.FOYER;
        AddRoomToHouse(foyerSouth);
    }

    private void GenerateMap()
    {
        int roomCount = (int)Math.Round(UnityEngine.Random.Range(MinRooms, MaxRooms));

        Debug.Log($"RoomCount {roomCount}");


    }

    private bool GenerateRoom()
    {


        return false;
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
        int floor = relativeRoom.FloorNumber;
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
        return networkedRoomInfoMap.Value.GetRoomAtPosition(x, z, floor);
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
