using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;

public class HouseGenerator : MonoBehaviour
{

    //make use of "hacked" multi floor generation, with static placed stairs to travel from one floor to another,
    //and use "2d" generation for each floor instead of making use of 3d generation of the entire house.

    //place rooms
    //triangulate using Delaunay triangulation graph
    //

    enum CellType
    {
        None,
        Room,
        Hallway
    }

    [SerializeField]
    Vector2Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector2Int roomMaxSize;



    Random random;
    Grid<CellType> grid;
    List<Room> rooms;
    Delaunay2D delaunay;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    void Generate()
    {
        random = new Random(0);
        grid = new Grid<CellType>(size, Vector2Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
    }

    void PlaceRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int roomLocation = new Vector2Int(random.Next(0, size.x), random.Next(0,size.y));

            Vector2Int roomSize = new Vector2Int(random.Next(0, roomMaxSize.x + 1), random.Next(0, roomMaxSize.y + 1));

            bool add = true;
            Room newRoom = new Room(roomLocation, roomSize);
            Room buffer = new Room(roomLocation + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms)
            {
                if(Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if(add && (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y))
            {
                add = false;
            }

            if (add)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach(var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }




    void PlaceRoom(Vector2Int position, Vector2Int size)
    {

    }

    void PlaceHallway(Vector2Int position)
    {

    }
}
