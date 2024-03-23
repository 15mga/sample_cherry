using System.Collections.Generic;
using Cherry;
using Cherry.Attr;
using Cherry.Extend;
using Cherry.View;
using Pb;
using Script.Ctrl;
using Script.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    public class VRoomBase : ViewBase
    {
        [AChild("OwnerForm")] private RectTransform _owner;
        [AChild("IptName")] private TMP_InputField _iptName;
        [AChild("TxtMode1")] private TMP_Text _txtMode;
        [AChild("DpdScene")] private TMP_Dropdown _dpdScene;
        [AChild("DpdMaxPlayers")] private TMP_Dropdown _dpdMaxPlayers;
        [AChild("IptPassword")] private TMP_InputField _iptPassword;
        [AChild("TxtTip")] private TMP_Text _txtTip;
        [AChild("Btn1")] private RectTransform _btn1;
        [AChild("BtnEdit")] private Button _btnEdit;
        [AChild("BtnCancel")] private Button _btnCancel;
        [AChild("Btn2")] private RectTransform _btn2;
        [AChild("BtnExit")] private Button _btnExit;
        [AChild("BtnStart")] private Button _btnStart;
        
        [AChild("MemberForm")] private RectTransform _member;
        [AChild("MemberTxtName")] private TMP_Text _txtNameMember;
        [AChild("MemberTxtMode")] private TMP_Text _txtModeMember;
        [AChild("MemberTxtScene")] private TMP_Text _txtSceneMember;
        [AChild("MemberTxtMaxPlayers")] private TMP_Text _txtMaxPlayersMember;
        [AChild("MemberBtnExit")] private Button _btnExitMember;
        [AChild("MemberBtnReady")] private Button _btnReadyMember;
        private bool _isReady;
        protected override void OnShow()
        {
            base.OnShow();
            
            _btnEdit.onClick.AddListener(OnClickEdit);
            _btnCancel.onClick.AddListener(OnClickCancel);
            _btnExit.onClick.AddListener(OnClickExit);
            _btnStart.onClick.AddListener(OnClickStart);
            _btnExitMember.onClick.AddListener(OnClickExitMember);
            _btnReadyMember.onClick.AddListener(OnClickReadyMember);
            _iptName.onValueChanged.AddListener(OnModifyRoom);
            _dpdScene.onValueChanged.AddListener(OnModifyRoom1);
            _dpdMaxPlayers.onValueChanged.AddListener(OnModifyRoom1);
            _iptPassword.onValueChanged.AddListener(OnModifyRoom);
            
            BindNotice(MRoom.NRoomModify, OnShowRoom);
            
            OnShowRoom(null);
        }

        private void OnShowRoom(object obj)
        {
            var room = Game.Model.Get<MRoom>().Room;
            if (room.OwnerId == Game.Model.Get<MPlayer>().Player.Id)
            {

                var mConf = Game.Model.Get<MConf>();
                var sceneTplNames = mConf.GetSceneTplNames();
                _dpdScene.options = new List<TMP_Dropdown.OptionData>();
                foreach (var name in sceneTplNames)
                {
                    _dpdScene.options.Add(new TMP_Dropdown.OptionData(name));
                }

                _dpdScene.value = room.SceneTplId;
                _dpdMaxPlayers.options = new List<TMP_Dropdown.OptionData>();
                var maxPlayers = mConf.GetMaxPlayers();
                foreach (var count in maxPlayers)
                {
                    _dpdMaxPlayers.options.Add(new TMP_Dropdown.OptionData(count.ToString()));
                }

                _dpdMaxPlayers.value = maxPlayers.IndexOf(room.MaxPlayers);
                
                ShowOwnerRoom(room);
                
                _owner.gameObject.SetActive(true);
                _member.gameObject.SetActive(false);
                _btn1.gameObject.SetActive(false);
            }
            else
            {
                _owner.gameObject.SetActive(false);
                _member.gameObject.SetActive(true);
                
                ShowMemberRoom(room);
            }
        }

        private void ShowOwnerRoom(Room room)
        {
            var mConf = Game.Model.Get<MConf>();
            _iptName.text = room.Name;
            _txtMode.text = mConf.GetModeName(room.Mode);
            _dpdScene.value= room.SceneTplId;
            _dpdScene.RefreshShownValue();

            var maxPlayers = mConf.GetMaxPlayers();
            _dpdMaxPlayers.value = maxPlayers.IndexOf(room.MaxPlayers);
            _dpdMaxPlayers.RefreshShownValue();

            _iptPassword.text = room.Password;
        }

        private void ShowMemberRoom(Room room)
        {
            var mConf = Game.Model.Get<MConf>();
            _txtNameMember.text = room.Name;
            _txtModeMember.text = mConf.GetModeName(room.Mode);
            _txtSceneMember.text = mConf.GetSceneTplName(room.SceneTplId);
            _txtMaxPlayersMember.text = room.MaxPlayers.ToString();
        }

        private void OnModifyRoom(string arg0)
        {
            _btn1.gameObject.SetActive(true);
        }

        private void OnModifyRoom1(int arg0)
        {
            _btn1.gameObject.SetActive(true);
        }

        protected override void OnHide()
        {
            _btnEdit.onClick.RemoveAllListeners();
            _btnCancel.onClick.RemoveAllListeners();
            _btnExit.onClick.RemoveAllListeners();
            _btnStart.onClick.RemoveAllListeners();
            _btnExitMember.onClick.RemoveAllListeners();
            _btnReadyMember.onClick.RemoveAllListeners();
            _iptName.onValueChanged.RemoveAllListeners();
            _dpdScene.onValueChanged.RemoveAllListeners();
            _dpdMaxPlayers.onValueChanged.RemoveAllListeners();
            _iptPassword.onValueChanged.RemoveAllListeners();
            UnbindNotice(MRoom.NRoomModify, OnShowRoom);
            base.OnHide();
        }

        private void OnClickReadyMember()
        {
            Game.Ctrl.Get<CRoom>().ReadyRoom(code =>
            {
                switch (code)
                {
                    case 0:
                        var tmp = _btnReadyMember.transform.FindComp<TMP_Text>("Text (TMP)");
                        tmp.text = Game.Model.Get<MRoom>().IsReady ? "取消" : "准备";
                        break;
                    case 1:
                        Game.Log.Debug("房间不存在");
                        break;
                    case 2:
                        Game.Log.Debug("没有进房间");
                        break;
                }
            });
        }

        private void OnClickExitMember()
        {
            Game.Ctrl.Get<CRoom>().ExitRoom(code =>
            {
                if (code > 0) return;
                Hide();
                Game.View.GetView<VRoomList>().Show();
            });
        }

        private void OnClickStart()
        {
            Game.Ctrl.Get<CRoom>().StartRoom(code =>
            {
                switch (code)
                {
                    case 0:
                        //开始成功,不需要处理，收到房间开始通知后再进入后续流程
                        break;
                    case 1:
                        _txtTip.text = "房间不存在或不是房主";
                        break;
                    case 2:
                        _txtTip.text = "没有进房间";
                        break;
                    case 3:
                        _txtTip.text = "有玩家没准备";
                        break;
                    case 4:
                        _txtTip.text = "创建场景失败";
                        break;
                }
            });
        }

        private void OnClickExit()
        {
            Game.Ctrl.Get<CRoom>().ExitRoom(code =>
            {
                if (code > 0) return;
                Hide();
                Game.View.GetView<VRoomList>().Show();
            });
        }

        private void OnClickCancel()
        {
            _btn1.gameObject.SetActive(false);
            OnShowRoom(null);
        }

        private void OnClickEdit()
        {
            _btn1.gameObject.SetActive(false);
            var maxPlayer = Game.Model.Get<MConf>().GetMaxPlayer(_dpdMaxPlayers.value);
            Game.Ctrl.Get<CRoom>().EditRoom(_iptName.text, _dpdScene.value, _iptPassword.text, maxPlayer,
                code =>
                {
                    switch (code)
                    {
                        case 0:
                            _txtTip.text = "";
                            break;
                        case 1:
                            _txtTip.text = "房间不存在";
                            break;
                        case 2:
                            _txtTip.text = "不是管理员";
                            break;
                    }
                });
        }
    }
}