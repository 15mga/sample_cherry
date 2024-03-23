using Cherry;
using Cherry.Model;
using Pb;

namespace Script.Model
{
    public class MRoom : ModelBase
    {
        public const string NRoomModify = "room_edited";
        public const string NRoomReady = "room_ready";
        public Room Room { get; private set; }

        public bool IsReady{ get;  private set; }
        
        public void SetRoom(Room room)
        {
            Room = room;
            Game.Notice.DispatchNotice(NRoomModify);
        }

        public void SetReady(bool isReady)
        {
            IsReady = isReady;
            Game.Notice.DispatchNotice(NRoomReady);
        }
    }
}