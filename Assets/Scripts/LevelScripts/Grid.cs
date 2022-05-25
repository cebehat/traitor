using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Grid<T>
{
    T[] data;

    //size of the grid in which to generate the house
    public Vector2Int Size { get; private set; }
    public Vector2Int Offset { get; set; }

    [SerializeField]
    RectInt bounds;

    public Grid(Vector2Int size, Vector2Int offset)
    {
        Size = size;
        Offset = offset;

        data = new T[size.x * size.y];
        bounds = new RectInt(Vector2Int.zero, Size);
    }

    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (Size.x * pos.y);
    }

    public bool InBounds(Vector2Int pos)
    {
        return bounds.Contains(pos + Offset);
    }

    public T this[int x, int y]
    {
        get 
        { 
            return this[new Vector2Int(x, y)]; 
        }
        set 
        { 
            this[new Vector2Int(x, y)] = value; 
        }
    }

    public T this[Vector2Int pos]
    {
        get
        {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set
        {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }

}

