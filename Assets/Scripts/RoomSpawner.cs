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

    public float MaxRooms { get; set; } = 150;
    public float MinRooms { get; set; } = 80;

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
        GenerateMap();
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

    public int GetRoomCount 
    { 
        get 
        {
            if (IsOnlineGame)
            {
                return networkedRoomInfoMap.Value.Count;
            }
            else
            {
                return roomInfoMap.Count;
            }
        } 
    }

    private RoomInfo GetRoomAtCoords(int X, int Z, int Floor)
    {
        if (IsOnlineGame)
        {
            return networkedRoomInfoMap.Value.GetRoomAtPosition(X, Z, Floor);
        }
        else
        {
            return roomInfoMap.GetRoomAtPosition(X, Z, Floor);
        }
    }

    private IEnumerable<RoomInfo> GetRoomsWithSpawnableDirections()
    {
        if (IsOnlineGame)
        {
            return networkedRoomInfoMap.Value.GetRoomsWithSpawnableDirections();
        }
        else
        {
            return roomInfoMap.GetRoomsWithSpawnableDirections();
        }
    }
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
        foyerNorth.Z = 0;
        foyerNorth.Walls[RoomDirection.NORTH] = RoomComponent.DOORWAY;
        foyerNorth.Walls[RoomDirection.SOUTH] = RoomComponent.NO_WALL;
        foyerNorth.Walls[RoomDirection.EAST] = RoomComponent.DOORWAY;
        foyerNorth.Walls[RoomDirection.WEST] = RoomComponent.DOORWAY;
        foyerNorth.RoomType = RoomType.FOYER;
        AddRoomToHouse(foyerNorth);

        var foyerSouth = new RoomInfo();
        foyerSouth.X = 0;
        foyerSouth.Z = -1;
        foyerSouth.Walls[RoomDirection.NORTH] = RoomComponent.NO_WALL;
        foyerSouth.Walls[RoomDirection.SOUTH] = RoomComponent.DECOY_DOOR;
        foyerSouth.Walls[RoomDirection.EAST] = RoomComponent.SOLID_WALL;
        foyerSouth.Walls[RoomDirection.WEST] = RoomComponent.SOLID_WALL;
        foyerSouth.RoomType = RoomType.FOYER;
        AddRoomToHouse(foyerSouth);
    }


    //Generate map needs to only be run by host if multiplayer.
    //it needs to be able to set a state when it's done, that allows players to spawn in.
    private void GenerateMap()
    {
        SpawnEntryHall();
        int roomCount = (int)Math.Round(UnityEngine.Random.Range(MinRooms, MaxRooms));

        Debug.Log($"RoomCount {roomCount}");

        
        while (GetRoomsWithSpawnableDirections().Any())
        {
            var originRoom = GetRoomsWithSpawnableDirections().First();
            GenerateRooms(originRoom, true);
        }
        //first find open doorways
        //then check if theres a room already connected to that doorway
        //if a room is found, do nothing
        //if a room is NOT found, create one 
        //the new rooms walls should be randomly generated, with any walls lining up to existing rooms be NO_WALL types.

        //spawn rules, if room Z coord is 0, south walls MUST be solid, unless it's a below surface room, in which case there are no coordinate limits.
    }

    private bool GenerateRooms(RoomInfo relativeRoom, bool recursiveGen = false)
    {
        Debug.Log(String.Format("Generating adjacent rooms for room at X{0}, Y{1}, Floor{2}", relativeRoom.X, relativeRoom.Z, relativeRoom.FloorNumber));
        foreach(var direction in relativeRoom.OpenDoorways)
        {
            if(GetAdjacentRoom(relativeRoom, direction, out var coords) == null)
            {
                var newRoom = new RoomInfo() { X = coords.X, Z = coords.Z, FloorNumber = coords.Floor};
                foreach (var dir in direction.OppositeDirection().AllOthers())
                {
                    if(newRoom.Z == 0 && dir == RoomDirection.SOUTH)
                    {
                        newRoom.Walls[dir] = RoomComponent.SOLID_WALL;
                    }
                    else if(GetAdjacentRoom(newRoom, dir) == null)
                    {
                        var solidWall = UnityEngine.Random.Range(0, 100) < (GetRoomCount < MinRooms ? 30f : 90f);
                        newRoom.Walls[dir] = solidWall ? RoomComponent.SOLID_WALL : RoomComponent.DOORWAY;
                    }
                }
                AddRoomToHouse(newRoom);
                if (recursiveGen) return GenerateRooms(newRoom, recursiveGen);
            }
        }
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
        RoomCoords coords;
        return GetAdjacentRoom(relativeRoom, dir, out coords);
    }

    public RoomInfo GetAdjacentRoom(RoomInfo relativeRoom, RoomDirection dir, out RoomCoords coords)
    {
        coords = GetAdjacentRoomCoords(relativeRoom, dir);
        return GetRoomAtCoords(coords.X, coords.Z, coords.Floor);
    }

    private RoomCoords GetAdjacentRoomCoords(RoomInfo relativeRoom, RoomDirection dir)
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
        return new RoomCoords() { X = x, Z = z, Floor = floor };
    }

    public class RoomCoords
    {
        public int X { get; set; }
        public int Z { get; set; }
        public int Floor { get; set; }
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
