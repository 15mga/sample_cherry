using Cherry;
using Cherry.State;
using Script.Ctrl;
using Script.SceneObj;
using Script.View;

namespace Script.State
{
    public class StatePlayerSelect : StateBase
    {
        public override void Enter()
        {
            base.Enter();

            AddCtrl<CPlayer>();

            BindSceneObj<SPlayerSelect>("PlayerSelect");

            LoadView<VPlayerSelect>();
            
            LoadScene("PlayerSelect", () =>
            {
                Game.View.GetView<VPlayerSelect>().Show();
            });
        }
    }
}