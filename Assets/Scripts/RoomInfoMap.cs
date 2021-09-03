using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomInfoMap
{
    private List<RoomInfo> _roomInfos;

    public RoomInfoMap()
    {
         _roomInfos = new List<RoomInfo>();
    }

    public void AddRoom(RoomInfo roomInfo)
    {
        _roomInfos.Add(roomInfo);
    }

    public RoomInfo GetRoomAtPosition(int X, int Z)
    {
        return _roomInfos.SingleOrDefault(x => x.X == X && x.Z == Z);
    }
}
