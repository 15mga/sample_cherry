using Cherry;
using Cherry.Model;
using Pb;

namespace Script.Model
{
    public class MRoom : ModelBase
    {
        public const string NRoomModify = "room_edited";
        public Room Room { get; private set; }

        private bool _isReady;
        
        public void SetRoom(Room room)
        {
            Room = room;
            Game.Notice.DispatchNotice(NRoomModify);
        }

        public bool ChangeReady()
        {
            _isReady = !_isReady;
            return _isReady;
        }
    }
}