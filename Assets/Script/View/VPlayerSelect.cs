using Cherry;
using Cherry.Attr;
using Cherry.View;
using Script.Ctrl;
using TMPro;
using UnityEngine.UI;

namespace Script.View
{
    public class VPlayerSelect : ViewBase
    {
        [AChild("BtnLeft")] private Button _btnLeft;
        [AChild("BtnRight")] private Button _btnRight;
        [AChild("IptNick")] private TMP_InputField _iptNick;
        [AChild("TxtTip")] private TMP_Text _txtTip;
        [AChild("BtnOk")]private Button _btnOk;

        protected override void OnShow()
        {
            base.OnShow();
            
            _btnLeft.onClick.AddListener(OnClickLeft);
            _btnRight.onClick.AddListener(OnClickRight);
            _btnOk.onClick.AddListener(OnClickOk);
        }

        protected override void OnHide()
        {
            _btnLeft.onClick.RemoveAllListeners();
            _btnRight.onClick.RemoveAllListeners();
            
            base.OnHide();
        }

        private void OnClickLeft()
        {
            Game.Ctrl.Get<CPlayer>().ShowLeft();
        }

        private void OnClickRight()
        {
            Game.Ctrl.Get<CPlayer>().ShowRight();
        }

        private void OnClickOk()
        {
            _txtTip.text = "";
            Game.Ctrl.Get<CPlayer>().PlayerNew(_iptNick.text, msg =>
            {
                _txtTip.text = msg;
            });
        }
    }
}