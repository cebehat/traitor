using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cebt.Shared;

namespace Cebt.RoomData
{
    public class RoomInfo
    {
        //X is the east/west coordinate
        public int X { get; set; } = 0;
        //Z is the north/south coordinate
        public int Z { get; set; } = 0;

        public int FloorNumber { get; set; } = 0;

        //the east/west physical size of the room
        public float RoomWidth = 10f;
        //the north/south physical size of the room
        public float RoomDepth = 10f;

        public Vector3 TransformPosition {
            get 
            {
                return new Vector3(x: RoomWidth * X, y: 0, z: RoomDepth * Z); 
            }
        }

        public bool IsRendered { get; set; } = false;
        public RoomType RoomType { get; set; } = RoomType.GENERIC;
        public Dictionary<RoomDirection, RoomComponent> Walls { get; set; }
            = new Dictionary<RoomDirection, RoomComponent> {
            { RoomDirection.NORTH, RoomComponent.NO_WALL },
            { RoomDirection.SOUTH, RoomComponent.NO_WALL },
            { RoomDirection.EAST, RoomComponent.NO_WALL },
            { RoomDirection.WEST, RoomComponent.NO_WALL }
            };

        public IEnumerable<RoomDirection> OpenDoorways 
        { 
            get 
            {
                List<RoomDirection> openDirections = new List<RoomDirection>();
                foreach(RoomDirection dir  in Enum.GetValues(typeof(RoomDirection)))
                    {
                        if (IsOpenDoorway(Walls[dir]))
                        {
                        openDirections.Add(dir);
                        }
                    }
                return openDirections;
            } 
        }

        private bool IsOpenDoorway(RoomComponent component)
        {
            switch (component)
            {
                case RoomComponent.DOORWAY:
                case RoomComponent.NO_WALL:
                case RoomComponent.DOOR:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to distinguish if a room is adjacent to this room 
        /// and if so which direction it is in
        /// </summary>
        /// <param name="otherRoom"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsAdjacent(RoomInfo otherRoom, out RoomDirection direction)
        {
            if (IsAdjacent(otherRoom))
            {
                direction = GetRelativeDirection(otherRoom);
                return true;
            }
            direction = 0;
            return false;
        }

        public string GetPrefabStringPrefix()
        {
            return String.Format("Rooms/{0}",Enum.GetName(typeof(RoomType), RoomType));
        }
        /// <summary>
        /// Method that returns wether a room is next to this room
        /// non diagonally, only in plus shape
        /// </summary>
        /// <param name="otherRoom"></param>
        /// <returns></returns>
        public bool IsAdjacent(RoomInfo otherRoom)
        {
            return (otherRoom.X.OneGreaterOrLessThan(this.X) && otherRoom.Z == this.Z) ||
                (otherRoom.X == this.X && otherRoom.Z.OneGreaterOrLessThan(this.Z));
        }        

        /// <summary>
        /// Returns the direction otherRoom is relative to the invoking object.
        /// </summary>
        /// <param name="otherRoom"></param>
        /// <returns>RoomDirection</returns>
        private RoomDirection GetRelativeDirection(RoomInfo otherRoom)
        {
            if (Z == -1) Debug.Log("Pause");
            RoomDirection direction = default(RoomDirection);
            if (otherRoom.Z < Z) direction |= RoomDirection.SOUTH;
            if (otherRoom.Z > Z) direction |= RoomDirection.NORTH;
            if (otherRoom.X < X) direction |= RoomDirection.WEST;
            if (otherRoom.X > X) direction |= RoomDirection.EAST;
            return direction;
        }
    }

}