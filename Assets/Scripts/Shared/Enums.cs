using System;

namespace Cebt.Shared
{
    [Flags]
    public enum RoomDirection
    {
        NORTH = 1,
        SOUTH = 2,
        EAST = 3,
        WEST = 4
    }
    public enum RoomType
    {
        GENERIC,
        HALLWAY,
        KITCHEN,
        BATHROOM,
        LABORATORY,
        RITUAL,
        BEDROOM,
        TEENROOM,
        CHILDROOM,
        LIBRARY,
        FOYER,
    }

    public enum RoomFloor
    {
        GROUND_FLOOR,
        FIRST_FLOOR,
        BASEMENT
    }

    public enum RoomComponent
    {
        SOLID_WALL,
        DOORWAY,
        NO_WALL,
        DECOY_DOOR,
        FLOOR,
        CEILING,
        DOOR,
    }
}