using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using Cebt.RoomData;
using Cebt.Shared;
using Unity.Netcode;

public class RoomSpawner : NetworkBehaviour
{

    public float MaxRooms { get; set; } = 150;
    public float MinRooms { get; set; } = 80;

    public bool DevMode { get; set; } = true;

    

    public int houseSeed { get; set; } = 0;



    #region Server-stuff

    public NetworkList<NetworkedRoomInfo> networkedRoomInfoDictionary;

    [SerializeField]
    public NetworkVariable<bool> mapGenerated;

    [SerializeField]
    public RoomInfoMap localRoomInfoMap;

    [SerializeField]
    private bool foyerSpawned = false;




    [ServerRpc]
    void AddRoomToHouseServerRpc(NetworkedRoomInfo room, string roomId, ServerRpcParams rpcParams = default)
    {
        Debug.Log(rpcParams.ToString());
        try
        {
            networkedRoomInfoDictionary.Add(room);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    #endregion

    #region Unity-Events
    // Start is called before the first frame update
    void Start()
    {
        localRoomInfoMap = new RoomInfoMap();
        if (houseSeed == 0) houseSeed = (int)UnityEngine.Random.Range(100f, 10000f);
        UnityEngine.Random.InitState(houseSeed);
        NetworkManager.Singleton.OnServerStarted += OnServerStartup;
        NetworkManager.OnClientConnectedCallback += OnClientConnected;

    }

    // Update is called once per frame
    void Update()
    {
        //if (!mapGenerated)
        //{
        //    GenerateMap();
        //}
        //else
        //{
        if (mapGenerated.Value)
        {
            IEnumerable<RoomInfo> unspawnedRooms = localRoomInfoMap.GetUnspawnedRooms();

            //Debug.Log("unspawned rooms: " + unspawnedRooms.Count());
            foreach (var item in unspawnedRooms)
            {
                SpawnRoom(item);
            }
        }


            
        //}
        
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        StopAllCoroutines();
        networkedRoomInfoDictionary.Dispose();
    }

    private void OnServerStartup()
    {
        Debug.Log("OnServerStartup");
        StartCoroutine("GenerateMap");
    }

    public void Awake()
    {
        Debug.Log("Awake");
        networkedRoomInfoDictionary = new NetworkList<NetworkedRoomInfo>();
        networkedRoomInfoDictionary.OnListChanged += OnNetworkedRoomInfoListChanged;
        
        mapGenerated = new NetworkVariable<bool>();
        mapGenerated.Value = false;
    }

    private void OnClientConnected(ulong obj)
    {
        if (networkedRoomInfoDictionary.Count > 0)
        {
            for (int i = 0; i < networkedRoomInfoDictionary.Count; i++)
            {
                AddRoomToLocalMapClientRpc(networkedRoomInfoDictionary[i], i);
            }
        }
    }

    private void OnNetworkedRoomInfoListChanged(NetworkListEvent<NetworkedRoomInfo> changeEvent)
    {
        if (changeEvent.Type == NetworkListEvent<NetworkedRoomInfo>.EventType.Add && !localRoomInfoMap.HasIndex(changeEvent.Index))
        {
            localRoomInfoMap.TryAddRoom(changeEvent.Value, changeEvent.Index);
            //AddRoomToLocalMapClientRpc(changeEvent.Value, changeEvent.Index);
        }
    }

    [ClientRpc]
    private void AddRoomToLocalMapClientRpc(NetworkedRoomInfo roomInfo, int index)
    {
        localRoomInfoMap.TryAddRoom(roomInfo, index);
    }

    #endregion

    public int GetRoomCount 
    { 
        get 
        {
            return networkedRoomInfoDictionary.Count;
        } 
    }

    private RoomInfo GetRoomAtCoords(int X, int Z, int Floor)
    {
        return localRoomInfoMap.GetRoomAtPosition(X, Z, Floor);
    }

    private IEnumerable<RoomInfo> GetRoomsWithSpawnableDirections()
    {
        return localRoomInfoMap.GetRoomsWithSpawnableDirections();
    }
    void AddRoomToHouse(RoomInfo room)
    {
        Debug.Log("Adding room to house");
        AddRoomToHouseServerRpc(room.GetAsNetworkedRoomInfo(), room.RoomName);
    }

    void SetRoomAsSpawned(RoomInfo room)
    {
        Debug.Log("Adding room to house");
        localRoomInfoMap.GetRoomAtPosition(room.X, room.Z, room.FloorNumber).IsRendered = true;
    }

    void SpawnRoom(RoomInfo room)
    {
        try
        {
            var roomObject = new GameObject(room.RoomName);
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
    }

    private void SpawnEntryHall()
    {
        var walls = new Dictionary<RoomDirection, RoomComponent>
        {
            { RoomDirection.NORTH, RoomComponent.DOORWAY },
            { RoomDirection.SOUTH, RoomComponent.NO_WALL },
            { RoomDirection.EAST, RoomComponent.DOORWAY },
            { RoomDirection.WEST, RoomComponent.DOORWAY },
        };

        var foyerNorth = new RoomInfo(0, 0, 0, RoomType.FOYER, walls);
        AddRoomToHouse(foyerNorth);

        var walls2 = new Dictionary<RoomDirection, RoomComponent>
        {
            { RoomDirection.NORTH, RoomComponent.NO_WALL },
            { RoomDirection.SOUTH, RoomComponent.DECOY_DOOR },
            { RoomDirection.EAST, RoomComponent.SOLID_WALL },
            { RoomDirection.WEST, RoomComponent.SOLID_WALL },
        };

        var foyerSouth = new RoomInfo(0, -1, 0, RoomType.FOYER, walls2);
        AddRoomToHouse(foyerSouth);
        foyerSpawned = true;
    }


    //Generate map needs to only be run by host if multiplayer.
    //it needs to be able to set a state when it's done, that allows players to spawn in.
    IEnumerator GenerateMap()
    {   
        SpawnEntryHall();
        yield return null; 
        
        int roomCount = (int)Math.Round(UnityEngine.Random.Range(MinRooms, MaxRooms));

        Debug.Log($"RoomCount {roomCount}");

        while (!mapGenerated.Value)
        {
            mapGenerated.Value = !GetRoomsWithSpawnableDirections().Any() && MinRooms <= GetRoomCount;
            if (GetRoomsWithSpawnableDirections().Any())
            {
                var originRoom = GetRoomsWithSpawnableDirections().First();
                GenerateRooms(originRoom, GetRoomCount >= roomCount);
            }            
            yield return null;
        }
        StopCoroutine("GenerateMap");
    }

    private bool GenerateRooms(RoomInfo relativeRoom, bool finalRooms = false)
    {
        Debug.Log(String.Format("Generating adjacent rooms for room at X{0}, Y{1}, Floor{2}", relativeRoom.X, relativeRoom.Z, relativeRoom.FloorNumber));
        foreach(var direction in relativeRoom.OpenDoorways)
        {
            var walls = new Dictionary<RoomDirection, RoomComponent>();
            var coords = GetAdjacentRoomCoords(relativeRoom, direction);
            //if(GetAdjacentRoom(relativeRoom, direction, out var coords) == null)
            if(!DoesCoordsHaveRoom(coords))
            {
                foreach (var dir in direction.OppositeDirection().AllOthers())
                {
                    if (finalRooms)
                    {
                        walls[dir] = RoomComponent.SOLID_WALL;
                    }
                    else if(coords.Z == 0 && dir == RoomDirection.SOUTH)
                    {
                        walls[dir] = RoomComponent.SOLID_WALL;
                    }
                    else if (!DoesCoordsHaveRoom(coords.NextDirectional(dir)))
                    {
                        var solidWall = UnityEngine.Random.Range(0, 100) < (GetRoomCount < MinRooms ? 30f : 90f);
                        walls[dir] = solidWall ? RoomComponent.SOLID_WALL : RoomComponent.DOORWAY;
                    }
                }
                var newRoom = new RoomInfo(coords.X, coords.Z, coords.Floor, RoomType.GENERIC, walls);
                AddRoomToHouse(newRoom);
            }
        }
        return false;
    }

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

    public bool DoesCoordsHaveRoom(RoomCoords coords)
    {
        return localRoomInfoMap.GetRoomAtPosition(coords.X, coords.Z, coords.Floor) != null;
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

        public RoomCoords NextDirectional(RoomDirection dir) 
        {
            switch (dir)
            {
                case RoomDirection.NORTH:
                    return new RoomCoords { X = X, Z = Z + 1, Floor = Floor };
                case RoomDirection.SOUTH:
                    return new RoomCoords { X = X, Z = Z - 1, Floor = Floor };
                case RoomDirection.EAST:
                    return new RoomCoords { X = X + 1, Z = Z, Floor = Floor };
                case RoomDirection.WEST:
                    return new RoomCoords { X = X - 1, Z = Z, Floor = Floor };
                default:
                    return this;
            }
        }
    }
}
