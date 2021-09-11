using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cebt.RoomData;
using Cebt.Shared;

public class Room : MonoBehaviour
{

    public RoomInfo RoomInfo;

    private RoomSpawner parent;
    private PrefabManager prefabManager;


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
        //SpawnWall(RoomDirection.NORTH);
        //SpawnWall(RoomDirection.SOUTH);
        //SpawnWall(RoomDirection.EAST);
        //SpawnWall(RoomDirection.WEST);
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

    //private void SpawnDoor(RoomDirection direction)
    //{
    //    if(Walls[direction] == RoomComponent.DOORWAY)
    //    {
    //        //check if door is already spawned there
    //        var doorPrefab = prefabManager.roomComponentDictionary[RoomComponent.DOOR];
    //        //Instantiate(doorPrefab, )


    //    }
    //}
    //private void SpawnWall(RoomDirection direction)
    //{
    //    var roomComponent = Walls[direction];

    //    if (roomComponent == RoomComponent.NO_WALL) return;

    //    Vector3 position;
    //    Quaternion rotation = new Quaternion();
    //    GameObject prefabToSpawn = prefabManager.roomComponentDictionary[roomComponent];
    //    if(roomComponent == RoomComponent.FLOOR)
    //    {
    //        position = transform.position + new Vector3(0f, 0f, 0f);
    //        Instantiate(prefabToSpawn, position, rotation, transform);
    //        return;
    //    }

    //    if (roomComponent == RoomComponent.CEILING)
    //    {
    //        position = transform.position + new Vector3(0f, 4f, 0f);
    //        Instantiate(prefabToSpawn, position, rotation, transform);
    //        return;
    //    }



    //    switch (direction)
    //    {
    //        case RoomDirection.NORTH:
    //            position = transform.position + new Vector3(0f, 0f, 5f);
    //            break;
    //        case RoomDirection.SOUTH:
    //            position = transform.position - new Vector3(0f, 0f, 5f);
    //            break;
    //        case RoomDirection.EAST:
    //            position = transform.position + new Vector3(5f, 0f, 0f);
    //            rotation = Quaternion.AngleAxis(90f, Vector3.up);
    //            Quaternion.AngleAxis(90f, Vector3.up);
    //            break;
    //        case RoomDirection.WEST:
    //            position = transform.position - new Vector3(5f, 0f, 0f);
    //            rotation = Quaternion.AngleAxis(90f, Vector3.up);
    //            break;            
    //        default:
    //            position = transform.position;
    //            break;
    //    }
    //    Instantiate(prefabToSpawn, position, rotation, transform);
    //}

    //public bool IsAdjacent(Room otherRoom)
    //{
    //    return ((otherRoom.x == this.x && ((otherRoom.z + 1) == this.z || (otherRoom.z - 1) == this.z)) ||
    //        (otherRoom.z == this.z && ((otherRoom.x + 1) == this.x || (otherRoom.x - 1) == this.x)));
    //}

    /// <summary>
    /// Returns what direction otherRoom is, relative to this room.
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <returns></returns>
    //public RoomDirection GetRelativeDirection(Room otherRoom)
    //{
    //    RoomDirection direction = default(RoomDirection);
    //    if(direction == default(RoomDirection))
    //    if (otherRoom.z < z) direction = RoomDirection.SOUTH;
    //    if (otherRoom.z > z) direction = RoomDirection.NORTH;
    //    if (otherRoom.x < x) direction = RoomDirection.WEST;
    //    if (otherRoom.x > x) direction = RoomDirection.EAST;

    //    return direction;
    //}
}