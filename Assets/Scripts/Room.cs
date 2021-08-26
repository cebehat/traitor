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

    private RoomSpawner parent;
    private PrefabManager prefabManager;

    [SerializeField]
    public RoomComponent northWall = RoomComponent.SOLID_WALL;
    [SerializeField]
    public RoomComponent southWall = RoomComponent.SOLID_WALL;
    [SerializeField]
    public RoomComponent eastWall = RoomComponent.SOLID_WALL;
    [SerializeField]
    public RoomComponent westWall = RoomComponent.SOLID_WALL;


    private void Awake()
    {
        parent = GetComponentInParent<RoomSpawner>();
        prefabManager = FindObjectOfType<PrefabManager>();
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
        SpawnWall(RoomDirection.NORTH, northWall);
        SpawnWall(RoomDirection.SOUTH, southWall);
        SpawnWall(RoomDirection.EAST, eastWall);
        SpawnWall(RoomDirection.WEST, westWall);
        SpawnWall(RoomDirection.NORTH, RoomComponent.CEILING);
        SpawnWall(RoomDirection.NORTH, RoomComponent.FLOOR);
    }

    private void SpawnWall(RoomDirection direction, RoomComponent roomComponent)
    {
        if (roomComponent == RoomComponent.NO_WALL) return;

        Vector3 position;
        Quaternion rotation = new Quaternion();
        GameObject prefabToSpawn = prefabManager._myDictionary[roomComponent];
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
