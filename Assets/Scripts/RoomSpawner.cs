using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        var originRoomNorthObject = new GameObject(String.Format("Room [{0}:{1}]", 0, 0));
        var originRoomNorth = originRoomNorthObject.AddComponent<Room>();
        originRoomNorth.x = 0;
        originRoomNorth.z = 0;
        originRoomNorth.northWall = RoomComponent.DOORWAY;
        originRoomNorth.southWall = RoomComponent.NO_WALL;
        originRoomNorth.eastWall = RoomComponent.DOORWAY;
        originRoomNorth.westWall = RoomComponent.DOORWAY;
        originRoomNorthObject.name = String.Format("Room [{0}:{1}]", 0, 0);
        originRoomNorth.SpawnRoom();
        
        
        var originRoomSouthObject = new GameObject(String.Format("Room [{0}:{1}]", 0, -1));
        originRoomSouthObject.transform.position -= new Vector3(0f, 0f, 10f);
        var originRoomSouth = originRoomSouthObject.AddComponent<Room>();
        originRoomSouth.x = 0;
        originRoomSouth.z = -1;
        originRoomSouth.northWall = RoomComponent.NO_WALL;
        originRoomSouth.southWall = RoomComponent.DECOY_DOOR;
        originRoomSouthObject.name = String.Format("Room [{0}:{1}]", 0, -1);
        originRoomSouth.SpawnRoom();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateAdjacentRoom(GameObject room, RoomDirection direction)
    {
        Room originRoomComponent = room.GetComponent<Room>();
        Vector3 spawnPosition = new Vector3();

        int x = originRoomComponent.x;
        int z = originRoomComponent.z;
        switch (direction)
        {
            case RoomDirection.NORTH:
                spawnPosition = room.transform.position + new Vector3(0f, 0f, 10f);
                z++;
                break;
            case RoomDirection.SOUTH:
                spawnPosition = room.transform.position - new Vector3(0f, 0f, 10f);
                z--;
                break;
            case RoomDirection.EAST:
                spawnPosition = room.transform.position + new Vector3(10f, 0f, 0f);
                x++;
                break;
            case RoomDirection.WEST:
                spawnPosition = room.transform.position - new Vector3(10f, 0f, 0f);
                x--;
                break;
        }
    }

    public void SpawnRooms(GameObject originRoom)
    {
        var adjacentRooms = GetComponentsInChildren<Room>().Where(room => room.IsAdjacent(originRoom.GetComponent<Room>())).ToList();
        var adjacentRoomDirections = adjacentRooms.Select(r => r.GetRelativeDirection(originRoom.GetComponent<Room>())).ToList();
        
        foreach(RoomDirection rd in Enum.GetValues(typeof(RoomDirection)))
        {
            if (!adjacentRoomDirections.Contains(rd))
            {

            }
        }

        //RoomDirection directionToSpawner;
        //Room originRoomComponent = originRoom.GetComponent<Room>();

            //int x = originRoomComponent.x;
            //int z = originRoomComponent.z;
            //switch (directionFromSpawner)
            //{
            //    case RoomDirection.NORTH:                
            //        spawnPosition = originRoom.transform.position + new Vector3(0f, 0f, 10f);
            //        z++;
            //        directionToSpawner = RoomDirection.SOUTH;
            //        break;
            //    case RoomDirection.SOUTH:
            //        spawnPosition = originRoom.transform.position - new Vector3(0f, 0f, 10f);
            //        z--;
            //        directionToSpawner = RoomDirection.NORTH;
            //        break;
            //    case RoomDirection.EAST:
            //        spawnPosition = originRoom.transform.position + new Vector3(10f, 0f, 0f);
            //        x++;
            //        directionToSpawner = RoomDirection.WEST;
            //        break;
            //    case RoomDirection.WEST:
            //        spawnPosition = originRoom.transform.position - new Vector3(10f, 0f, 0f);
            //        x--;
            //        directionToSpawner = RoomDirection.EAST;
            //        break;
            //    default:
            //        spawnPosition = new Vector3();
            //        directionToSpawner = RoomDirection.NORTH;
            //        break;
            //}
            //Debug.Log(spawnPosition);


            //Room newRoomComponent;

            //if(GetComponentsInChildren<Room>().Any(room => room.x == x && room.z == z))
            //{
            //    newRoomComponent = GetComponentsInChildren<Room>().Single(room => room.x == x && room.z == z);
            //    Debug.Log("Attaching existing room");
            //}
            //else
            //{
            //    var newRoom = Instantiate(roomPrefab, spawnPosition, originRoom.transform.rotation, transform);
            //    newRoom.name = String.Format("Room [{0}:{1}]", x, z);
            //    newRoomComponent = newRoom.GetComponent<Room>();
            //    if(GetComponentsInChildren<Room>().Count(r => r.HasOpenPathway()) > 2)
            //    {
            //        newRoomComponent.AllowDeadEnd = true;
            //    }
            //    else
            //    {
            //        newRoomComponent.AllowDeadEnd = false;
            //    }
            //    Debug.Log("Spawning new room");
            //}
            //newRoomComponent.x = x;
            //newRoomComponent.z = z;
            //AddAdjacentRooms(newRoomComponent);
    }

    void AddAdjacentRooms(Room room)
    {
        //var adjacentRooms = GetComponentsInChildren<Room>().Where(r => r.IsAdjacent(room)).ToList();

        //if (adjacentRooms.Any())
        //{
        //    foreach(var aRoom in adjacentRooms)
        //    {
        //        RoomDirection relativeDirection = room.GetRelativeDirection(aRoom);
        //        if (!room.adjacentRooms.ContainsKey(relativeDirection))
        //        {
        //            room.AddAdjacentRoom(relativeDirection, aRoom);
        //        }
        //        relativeDirection = aRoom.GetRelativeDirection(room);
        //        if (!aRoom.adjacentRooms.ContainsKey(relativeDirection))
        //        {
        //            aRoom.AddAdjacentRoom(relativeDirection, room);
        //        }
        //        if (aRoom.HasConnectingDoor(room))
        //        {
        //            room.OpenDoorways.Add(room.GetRelativeDirection(aRoom));
        //        }
        //    }
        //}
    }
}
