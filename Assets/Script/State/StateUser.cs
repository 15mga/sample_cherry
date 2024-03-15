using Cherry;
using Cherry.State;
using Script.Ctrl;
using Script.View;

namespace Script.State
{
    public class StateUser : StateBase
    {
        public override void Enter()
        {
            base.Enter();

            AddCtrl<CUser>();

            LoadView<VSignIn>();
            LoadView<VSignUp>();
            
            Game.Log.Debug($"{Game.View}");

            Game.Ctrl.Get<CConn>().Conn();
            Game.View.GetView<VSignIn>().Show();
        }

        public override void Exit()
        {
            Game.View.UnloadView<VSignIn>();
            base.Exit();
        }
    }
}