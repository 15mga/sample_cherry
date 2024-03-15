using System;
using Cherry;
using Cherry.Attr;
using Cherry.View;
using Pb;
using Script.Ctrl;
using Script.Model;
using TMPro;
using UnityEngine.UI;

namespace Script.View
{
    public class VSignIn : ViewBase
    {
        [AChild("BtnOk")] private Button _btnOk;
        [AChild("BtnSignUp")] private Button _btnSignUp;
        [AChild("TxtTip")] private TMP_Text _txtTip;
        [AChild("IptUser")] private TMP_InputField _iptUser;
        [AChild("IptPassword")] private TMP_InputField _iptPassword;

        protected override void OnShow()
        {
            base.OnShow();
            
            _btnOk.onClick.AddListener(OnClickOk);
            _btnSignUp.onClick.AddListener(OnClickSignUp);

            _iptUser.text = "15m@15m.games";
            _iptPassword.text = "12341234";
        }

        protected override void OnHide()
        {
            _btnOk.onClick.RemoveAllListeners();
            _btnSignUp.onClick.RemoveAllListeners();
            
            base.OnHide();
        }

        private void OnClickSignUp()
        {
            Game.View.GetView<VSignIn>().Hide();
            Game.View.GetView<VSignUp>().Show();
        }

        private void OnClickOk()
        {
            Game.Ctrl.Get<CUser>().SignIn(_iptUser.text, _iptPassword.text, msg =>
            {
                _txtTip.text = msg;
            });
        }

        private void GetPlayer()
        {
            Game.Ctrl.Get<CConn>().Request<PlayerRes>(new PlayerReq(),
                code =>
            {
                Game.Log.Error("这里应该弹出获取角色信息失败！！");
            }, res =>
            {
                Game.Model.Get<MPlayer>().SetPlayer(res.Player);
                Game.View.GetView<VSignIn>().Hide();
                Game.Scene.LoadScene("Room", () =>
                {
                    Game.Log.Debug("room list show");
                    Game.View.GetView<VRoomList>().Show();
                });
            });
        }
    }
}