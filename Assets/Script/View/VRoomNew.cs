using System.Collections.Generic;
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
    public class VRoomNew : ViewBase
    {
        [AChild("IptName")] private TMP_InputField _iptName;
        [AChild("DpdMode")] private TMP_Dropdown _dpdMode;
        [AChild("DpdScene")] private TMP_Dropdown _dpdScene;
        [AChild("IptPassword")] private TMP_InputField _iptPassword;
        [AChild("DpdMaxPlayers")] private TMP_Dropdown _dpdMaxPlayers;
        [AChild("TxtTip")] private TMP_Text _txtTip;
        [AChild("BtnOk")] private Button _btnOk;
        [AChild("BtnCancel")] private Button _btnCancel;
        protected override void OnLoad()
        {
            base.OnLoad();
            
            _dpdMode.options = new List<TMP_Dropdown.OptionData>();
            foreach (var name in Game.Model.Get<MConf>().GetModeNames())
            {
                _dpdMode.options.Add(new TMP_Dropdown.OptionData(name));
            }
            _dpdScene.options = new List<TMP_Dropdown.OptionData>();
            var mConf = Game.Model.Get<MConf>();
            foreach (var name in mConf.GetSceneTplNames())
            {
                _dpdScene.options.Add(new TMP_Dropdown.OptionData(name));
            }
            _dpdMaxPlayers.options = new List<TMP_Dropdown.OptionData>();
            foreach (var count in mConf.GetMaxPlayers())
            {
                _dpdMaxPlayers.options.Add(new TMP_Dropdown.OptionData(count.ToString()));
            }
        }

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
            _txtTip.text = "";
            var maxPlayer = Game.Model.Get<MConf>().GetMaxPlayer(_dpdMaxPlayers.value);
            Game.Ctrl.Get<CRoom>().NewRoom(_iptName.text, (ESceneMode)_dpdMode.value, _dpdScene.value, 
                _iptPassword.text, maxPlayer,msg =>
            {
                if (string.IsNullOrEmpty(msg))
                {
                    Game.View.GetView<VRoomNew>().Hide();
                    return;
                }
                _txtTip.text = msg;
            });
        }

        private void OnClickCancel()
        {
            Game.View.GetView<VRoomNew>().Hide();
            Game.View.GetView<VRoomList>().Show();
        }
    }
}