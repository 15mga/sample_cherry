using Cherry.Attr;

namespace Script.View
{
    [APool("RoomPlayer1", 8)]
    [APool("RoomPlayer2", 8)]
    public class VRoomPVP : VRoomBase
    {
        protected override void OnShow()
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }
    }
}