using System;
using Cherry;
using Cherry.Ctrl;
using Pb;
using Script.Model;
using Script.View;

namespace Script.Ctrl
{
    public class CScene : CtrlBase<MScene>
    {
        public override void Initialize(Action onComplete = null)
        {
            onComplete?.Invoke();
        }

        public void EntryScene()
        {
            Game.View.GetView<VScene>().Show();
            Game.Ctrl.Get<CConn>().Request<SceneEntryRes>(new SceneEntryReq(),
                code =>
                {
                    Game.Log.Debug($"进入场景失败:{code}");
                }, res =>
                {
                    Game.Log.Debug($"进入场景成功");
                    Model.InitPlayerPosition(res.Position);
                });
        }

        public void Movement(SceneMovement movement)
        {
            Game.Ctrl.Get<CConn>().Request<SceneMovementRes>(new SceneMovementReq
            {
                Movement = movement,
            }, code =>
            {
                switch (code)
                {
                    case 1:
                        Game.Log.Debug("没有进入场景");
                        break;
                }
            }, res =>
            {
                
            });
        }

        public void AddRobot(int count, Action<int> handler)
        {
            Game.Ctrl.Get<CConn>().Request<SceneRobotAddRes>(new SceneRobotAddReq
            {
                Count = count,
            }, code =>
            {
                Game.Log.Debug("添加机器人失败");
            }, res =>
            {
                handler(res.CurrCount);
            });
        }

        public void ClearRobot()
        {
            Game.Ctrl.Get<CConn>().Request<SceneRobotClearRes>(new SceneRobotClearReq
            {
            }, code =>
            {
                
            }, res =>
            {
                
            });
        }
    }
}