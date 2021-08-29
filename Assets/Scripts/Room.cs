using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField]
    public int x;
    [SerializeField]
    public int z;
    [SerializeField]
    public RoomType roomType = RoomType.GENERIC;

    private RoomSpawner parent;
    private PrefabManager prefabManager;

    [SerializeField]
    public Dictionary<RoomDirection, RoomComponent> Walls = 
        new Dictionary<RoomDirection, RoomComponent> { 
            { RoomDirection.NORTH, RoomComponent.SOLID_WALL },
            { RoomDirection.SOUTH, RoomComponent.SOLID_WALL },
            { RoomDirection.EAST, RoomComponent.SOLID_WALL },
            { RoomDirection.WEST, RoomComponent.SOLID_WALL }};


    private void Awake()
    {
        parent = FindObjectOfType<RoomSpawner>();
        prefabManager = FindObjectOfType<PrefabManager>();
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(10, 4, 10);
        Debug.Log("Room created");
    }

    void OnTriggerEnter(Collider coll){
        if(coll.CompareTag("Player")){
            Debug.Log("Player Entered");
            parent.SpawnRooms(gameObject);
        }
    }

    public void SpawnRoom()
    {
        SpawnWall(RoomDirection.NORTH);
        SpawnWall(RoomDirection.SOUTH);
        SpawnWall(RoomDirection.EAST);
        SpawnWall(RoomDirection.WEST);
        SpawnFloorAndCeiling();
    }

    private void SpawnFloorAndCeiling()
    {
        GameObject prefabToSpawn = prefabManager.roomComponentDictionary[RoomComponent.FLOOR];
        var position = transform.position + new Vector3(0f, 0f, 0f);
        Instantiate(prefabToSpawn, position, transform.rotation, transform);
        prefabToSpawn = prefabManager.roomComponentDictionary[RoomComponent.CEILING];
        position = transform.position + new Vector3(0f, 4f, 0f);
        Instantiate(prefabToSpawn, position, transform.rotation, transform);
        return;

    }
    private void SpawnWall(RoomDirection direction)
    {
        var roomComponent = Walls[direction];

        if (roomComponent == RoomComponent.NO_WALL) return;

        Vector3 position;
        Quaternion rotation = new Quaternion();
        GameObject prefabToSpawn = prefabManager.roomComponentDictionary[roomComponent];
        if(roomComponent == RoomComponent.FLOOR)
        {
            position = transform.position + new Vector3(0f, 0f, 0f);
            Instantiate(prefabToSpawn, position, rotation, transform);
            return;
        }

        if (roomComponent == RoomComponent.CEILING)
        {
            position = transform.position + new Vector3(0f, 4f, 0f);
            Instantiate(prefabToSpawn, position, rotation, transform);
            return;
        }



        switch (direction)
        {
            case RoomDirection.NORTH:
                position = transform.position + new Vector3(0f, 0f, 5f);
                break;
            case RoomDirection.SOUTH:
                position = transform.position - new Vector3(0f, 0f, 5f);
                break;
            case RoomDirection.EAST:
                position = transform.position + new Vector3(5f, 0f, 0f);
                rotation = Quaternion.AngleAxis(90f, Vector3.up);
                Quaternion.AngleAxis(90f, Vector3.up);
                break;
            case RoomDirection.WEST:
                position = transform.position - new Vector3(5f, 0f, 0f);
                rotation = Quaternion.AngleAxis(90f, Vector3.up);
                break;            
            default:
                position = transform.position;
                break;
        }
        Instantiate(prefabToSpawn, position, rotation, transform);
    }

    public bool IsAdjacent(Room otherRoom)
    {
        return ((otherRoom.x == this.x && ((otherRoom.z + 1) == this.z || (otherRoom.z - 1) == this.z)) ||
            (otherRoom.z == this.z && ((otherRoom.x + 1) == this.x || (otherRoom.x - 1) == this.x)));
    }

    /// <summary>
    /// Returns what direction otherRoom is, relative to this room.
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <returns></returns>
    public RoomDirection GetRelativeDirection(Room otherRoom)
    {
        RoomDirection direction = RoomDirection.NORTH;

        if (otherRoom.z < z) direction = RoomDirection.SOUTH;
        if (otherRoom.z > z) direction = RoomDirection.NORTH;
        if (otherRoom.x < x) direction = RoomDirection.WEST;
        if (otherRoom.x > x) direction = RoomDirection.EAST;

        return direction;
    }
}
