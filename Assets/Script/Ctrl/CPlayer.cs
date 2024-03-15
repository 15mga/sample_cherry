using System;
using Cherry;
using Cherry.Ctrl;
using Pb;
using Script.Model;
using Script.State;

namespace Script.Ctrl
{
    public class CPlayer : CtrlBase<MPlayer>
    {
        public static string N_PlayerShow = "player_create_show";
        
        public int SelectIdx { get; private set; }
        
        public override void Initialize(Action onComplete = null)
        {
            
        }

        public void ShowLeft()
        {
            ShowIndex(SelectIdx - 1);
        }

        public void ShowRight()
        {
            ShowIndex(SelectIdx + 1);
        }

        private void ShowIndex(int idx)
        {
            var names = Game.Model.Get<MConf>().Heros();
            if (idx < 0)
            {
                idx += names.Count;
            } 
            else if (idx >= names.Count)
            {
                idx -= names.Count;
            }
            Game.Log.Debug($"show index: {idx}");
            SelectIdx = idx;
            Game.Notice.DispatchNotice(N_PlayerShow, names[idx]);
        }

        public void PlayerNew(string nick, Action<string> callback)
        {
            if (nick.Length < 2)
            {
                callback("昵称不能少于2个字符");
                return;
            }

            Game.Ctrl.Get<CConn>().Request<PlayerNewRes>(new PlayerNewReq
            {
                Nick = nick,
                Hero = SelectIdx
            }, code =>
            {
                switch (code)
                {
                    case 1:
                        callback("已存在的昵称");
                        break;
                }
            }, res =>
            {
                GetPlayerComplete(res.Player);
            });
        }

        public void GetPlayer()
        {
            Game.Ctrl.Get<CConn>().Request<PlayerRes>(new PlayerReq(),
                code =>
                {
                    switch (code)
                    {
                        case 1:
                            Game.Fsm.Main.ChangeState<StatePlayerSelect>();
                            break;
                    }
                }, res =>
                {
                    GetPlayerComplete(res.Player);
                });
        }

        private void GetPlayerComplete(Player player)
        {
            Model.SetPlayer(player);
            switch (player.Status)
            {
                case PlayerStatus.Online:
                case PlayerStatus.Disconnect:
                    Reconnect();
                    break;
                default:
                    Game.Fsm.Main.ChangeState<StateRoom>();
                    break;
            }
        }

        private void Reconnect()
        {
            Game.Ctrl.Get<CConn>().Request<PlayerReconnectRes>(new PlayerReconnectReq(),
                code =>
                {
                    Game.Log.Info("重连失败");
                    Game.Fsm.Main.ChangeState<StateRoom>();
                }, res =>
                {
                    if (res.RoomId == "")
                    {
                        Game.Fsm.Main.ChangeState<StateRoom>();
                        return;
                    }
                    Model.SetReconnect(new MPlayer.ReconnectData
                    {
                        RoomId = res.RoomId,
                        SceneId = res.SceneId,
                    });
                    if (!string.IsNullOrEmpty(res.SceneId))
                    {
                        Game.Fsm.Main.ChangeState<StateScene>();
                        return;
                    }
                    Game.Fsm.Main.ChangeState<StateRoom>();
                });
        }
    }
}