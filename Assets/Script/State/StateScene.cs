using Cherry;
using Cherry.State;
using Script.Ctrl;
using Script.SceneObj;
using Script.View;

namespace Script.State
{
    public class StateScene : StateBase
    {
        public override void Enter()
        {
            base.Enter();

            BindSceneObj<SSceneBackground>("Scene");
            BindSceneObj<SScenePawn>("Scene");
            BindSceneObj<SScenePrefab>("Scene");

            AddCtrl<CScene>().Initialize();

            LoadView<VScene>();

            //todo 更具房间信息加载地图,这里应该有加载界面过度
            Game.Scene.LoadScene("Scene", () =>
            {
            });
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}