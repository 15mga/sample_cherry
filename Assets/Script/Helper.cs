using Cherry;
using Script.Ctrl;
using Script.Model;
using Script.State;

namespace Script
{
    public class Helper : GameHelperBase
    {
        public override void Begin()
        {
            base.Begin();
            
            InitViewLayer();

            BindModel();
            BindCtrl();
            BindState();
        
            Game.Asset.Init("Default", StartGame);
        }

        private void InitViewLayer()
        {
            Game.View.AddLayer(ViewLayers.Default, true);
        }

        private void BindState()
        {
            Game.Fsm.Main.RegisterState<StatePlayer>();
            Game.Fsm.Main.RegisterState<StatePlayerSelect>();
            Game.Fsm.Main.RegisterState<StateRoom>();
            Game.Fsm.Main.RegisterState<StateScene>();
            Game.Fsm.Main.RegisterState<StateUser>();
        }

        private void BindModel()
        {
            Game.Model.Add<MConf>();
            Game.Model.Add<MPlayer>();
            Game.Model.Add<MRoom>();
            Game.Model.Add<MScene>();
            Game.Model.Add<MUser>();
        }

        private void BindCtrl()
        {
            Game.Ctrl.Add<CConn>();
        }

        private void StartGame()
        {
            Game.Ctrl.Get<CConn>().Initialize();
            Game.Fsm.Main.ChangeState<StateUser>();
        }
    }
    public static class ViewLayers
    {
        public const string Background = "Background";
        public const string Default = "Default";
        public const string Top = "Top";
    }
}
