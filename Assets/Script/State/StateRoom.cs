using Cherry;
using Cherry.State;
using Script.Ctrl;
using Script.View;

namespace Script.State
{
    public class StateRoom : StateBase
    {
        public override void Enter()
        {
            base.Enter();

            AddCtrl<CRoom>();

            LoadView<VRoomList>();
            LoadView<VRoomNew>();
            LoadView<VRoomPVE>();
            LoadView<VRoomPVP>();
            LoadView<VRoomPVR>();
            LoadView<VRoomWar>();

            Game.Ctrl.Get<CRoom>().Initialize();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}