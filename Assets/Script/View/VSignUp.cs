using Cherry;
using Cherry.Attr;
using Cherry.View;
using Script.Ctrl;
using TMPro;
using UnityEngine.UI;

namespace Script.View
{
    public class VSignUp : ViewBase
    {
        [AChild("BtnOk")] private Button _btnOk;
        [AChild("BtnCancel")] private Button _btnCancel;
        [AChild("TxtTip")] private TMP_Text _txtTip;
        [AChild("IptUser")] private TMP_InputField _iptUser;
        [AChild("IptPassword")] private TMP_InputField _iptPassword;

        protected override void OnShow()
        {
            base.OnShow();
            
            _btnOk.onClick.AddListener(OnClickOk);
            _btnCancel.onClick.AddListener(OnClickCancel);
        }

        protected override void OnHide()
        {
            _btnOk.onClick.RemoveAllListeners();
            _btnCancel.onClick.RemoveAllListeners();
            
            base.OnHide();
        }

        private void OnClickOk()
        {
            Game.Ctrl.Get<CUser>().SignUp(_iptUser.text, _iptPassword.text, msg =>
            {
                
            });
        }

        private void OnClickCancel()
        {
            Game.View.GetView<VSignUp>().Hide();
            Game.View.GetView<VSignIn>().Show();
        }
    }
}