using Cherry;
using Cherry.State;
using Script.Ctrl;

namespace Script.State
{
    public class StatePlayer : StateBase
    {
        public override void Enter()
        {
            base.Enter();

            AddCtrl<CPlayer>();
            
            Game.Ctrl.Get<CPlayer>().GetPlayer();
        }
    }
}