using System;
using System.Collections.Generic;
using System.Linq;
using Cherry;
using Cherry.Ctrl;
using Pb;
using Script.Model;
using Script.State;
using Script.View;

namespace Script.Ctrl
{
    public class CRoom : CtrlBase<MRoom>
    {
        public override void Initialize(Action onComplete = null)
        {
            Game.Ctrl.Get<CConn>().BindPus<RoomEntryPus>(OnRoomEntryPus);
            Game.Ctrl.Get<CConn>().BindPus<RoomModifyPus>(OnRoomModifyPus);
            Game.Ctrl.Get<CConn>().BindPus<RoomStartPus>(OnRoomStartPus);
            Game.Ctrl.Get<CConn>().BindPus<RoomReadyPus>(OnRoomReadyPus);
            Game.Ctrl.Get<CConn>().BindPus<RoomExitPus>(OnRoomExitPus);
            Game.Ctrl.Get<CConn>().BindPus<RoomDisconnectPus>(OnRoomDisconnectPus);
            Game.Ctrl.Get<CConn>().BindPus<RoomReconnectPus>(OnRoomStartReconnectPus);

            var mPlayer = Game.Model.Get<MPlayer>();
            if (mPlayer.Reconnect == null)
            {
                Game.View.GetView<VRoomList>().Show();
            }
            else
            {
                ReconnectRoom();
            }
            onComplete?.Invoke();
        }

        private void OnRoomEntryPus(RoomEntryPus obj)
        {
            Game.Log.Debug($"玩家: {obj.PlayerId} 进入房间");
        }

        private void OnRoomExitPus(RoomExitPus obj)
        {
            Game.Log.Debug($"玩家: {obj.PlayerId} 退出房间");
        }

        private void OnRoomReadyPus(RoomReadyPus obj)
        {
            Game.Log.Debug($"玩家: {obj.PlayerId} 准备: {obj.Ready}");
        }

        private void OnRoomDisconnectPus(RoomDisconnectPus obj)
        {
            Game.Log.Debug($"玩家: {obj.PlayerId} 掉线");
        }

        private void OnRoomStartReconnectPus(RoomReconnectPus obj)
        {
            Game.Log.Debug($"玩家: {obj.PlayerId} 重连");
        }

        public void GetRoomList(string nameFilter, int modelFilter, int sceneTplFilter, 
            int page, int count, Action<List<Room>> handler)
        {
            var req = new RoomListReq
            {
                ModeFilter = modelFilter,
                SceneTplFilter = sceneTplFilter,
                Page = page,
                Count = count,
            };
            if (!string.IsNullOrEmpty(nameFilter))
            {
                req.NameFilter = nameFilter;
            }
            Game.Log.Debug($"get room list: {req}");
            Game.Ctrl.Get<CConn>().Request<RoomListRes>(req, null, res =>
            {
                handler(res.List.ToList());
            });
        }

        public void EntryRoom(string roomId)
        {
            Game.Ctrl.Get<CConn>().Request<RoomEntryRes>(new RoomEntryReq
            {
                RoomId = roomId,
            }, code =>
            {
                //todo 弹提示界面
                switch (code)
                {
                    case 1:
                        Game.Log.Error("房间不存在");
                        break;
                    case 2:
                        Game.Log.Error("房间已满");
                        break;
                    case 3:
                        Game.Log.Error("不能进入该房间");
                        break;
                }
            }, res =>
            {
                Game.View.GetView<VRoomList>().Hide();
                Model.SetRoom(res.Room);
                ShowRoom();
            });
        }

        private void ReconnectRoom()
        {
            Game.Ctrl.Get<CConn>().Request<RoomReconnectRes>(new RoomReconnectReq()
            {
            }, code =>
            {
                //todo 弹提示界面
                switch (code)
                {
                    case 1:
                        Game.Log.Error("房间不存在");
                        break;
                    case 2:
                        Game.Log.Error("房间已满");
                        break;
                    case 3:
                        Game.Log.Error("不能进入该房间");
                        break;
                }
            }, res =>
            {
                Game.View.GetView<VRoomList>().Hide();
                Model.SetRoom(res.Room);
                ShowRoom();
            });
        }

        public void NewRoom(string name, ESceneMode mode, int sceneTpl, string pw, int maxPlayers, Action<string> callback)
        {
            Game.Ctrl.Get<CConn>().Request<RoomNewRes>(new RoomNewReq
            {
                Name = name,
                Mode = mode,
                SceneTplId = sceneTpl,
                Password = pw,
                MaxPlayers = maxPlayers,
            }, code =>
            {
                switch (code)
                {
                    case 1:
                        callback("房间名已存在");
                        break;
                }
            }, res =>
            {
                callback("");
                Model.SetRoom(res.Room);
                ShowRoom();
            });
        }

        public void EditRoom(string name, int sceneTpl, string pw, int maxPlayers, Action<ushort> handler)
        {
            var room = Model.Room;
            if (room.Name.Equals(name) && room.SceneTplId.Equals(sceneTpl) &&
                room.Password.Equals(pw) && room.MaxPlayers == maxPlayers)
            {
                handler(0);
                return;
            }
            Game.Ctrl.Get<CConn>().Request<RoomModifyRes>(new RoomModifyReq
            {
                Name = name,
                SceneTplId = sceneTpl,
                Password = pw,
                MaxPlayers = maxPlayers,
            }, handler, res =>
            {
                handler(0);
            });
        }

        private void ShowRoom()
        {
            switch (Model.Room.Mode)
            {
                case ESceneMode.Pve:
                    Game.View.GetView<VRoomPVE>().Show();
                    break;
                case ESceneMode.Pvp:
                    Game.View.GetView<VRoomPVP>().Show();
                    break;
                case ESceneMode.Pvr:
                    Game.View.GetView<VRoomPVR>().Show();
                    break;
                case ESceneMode.War:
                    Game.View.GetView<VRoomWar>().Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ExitRoom(Action<ushort> handler)
        {
            Game.Ctrl.Get<CConn>().Request<RoomExitRes>(new RoomExitReq(),
                handler, res =>
                {
                    handler(0);
                });
        }

        public void GetRoom(string roomId, Action<Room, ushort> handler)
        {
            Game.Ctrl.Get<CConn>().Request<RoomGetRes>(new RoomGetReq
            {
                RoomId = roomId,
            }, code =>
            {
                handler(null, code);
            }, res =>
            {
                handler(res.Room, 0);
            });
        }

        public void StartRoom(Action<ushort> handler)
        {
            Game.Ctrl.Get<CConn>().Request<RoomStartRes>(new RoomStartReq(), 
                handler, 
                res =>
            {
                handler(0);
            });
        }

        public void ReadyRoom(Action<ushort> handler)
        {
            Game.Ctrl.Get<CConn>().Request<RoomReadyRes>(new RoomReadyReq
                {
                    IsReady = !Model.IsReady
                }, 
                handler, 
                res =>
                {
                    Model.SetReady(!Model.IsReady);
                    handler(0);
                });
        }

        private void OnRoomModifyPus(RoomModifyPus ntc)
        {
            Model.SetRoom(ntc.Room);
        }

        private void OnRoomStartPus(RoomStartPus obj)
        {
            Game.Fsm.Main.ChangeState<StateScene>();
        }
    }
}