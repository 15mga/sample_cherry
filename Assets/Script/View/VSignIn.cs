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
            BindNotice(CConn.N_Connect_Change, OnUpdateConnState);

            _iptUser.text = "15m@15m.games";
            _iptPassword.text = "12341234";
            
            OnUpdateConnState(null);
        }

        protected override void OnHide()
        {
            _btnOk.onClick.RemoveAllListeners();
            _btnSignUp.onClick.RemoveAllListeners();
            
            base.OnHide();
        }

        private void OnUpdateConnState(object data)
        {
            if (Game.Ctrl.Get<CConn>().IsConnected)
            {
                _btnOk.enabled = true;
                _btnSignUp.enabled = true;
            }
            else
            {
                _btnOk.enabled = false;
                _btnSignUp.enabled = false;
            }
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
    }
}