using System.Collections.Generic;
using Cherry;
using Cherry.Attr;
using Cherry.Extend;
using Cherry.View;
using Cherry.View.Event;
using Pb;
using Script.Ctrl;
using Script.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    [APool("Item", 16, 0)]
    public class VRoomList : ViewBase
    {
        [AChild("IptName")] private TMP_InputField _iptName;
        [AChild("DpdMode")] private TMP_Dropdown _dpdMode;
        [AChild("DpdScene")] private TMP_Dropdown _dpdScene;
        [AChild("BtnSearch")] private Button _btnSearch;
        [AChild("BtnPrev")] private Button _btnPrev;
        [AChild("BtnNext")] private Button _btnNext;
        [AChild("BtnNew")] private Button _btnNew;

        private int _currPage;
        private int _currMode;
        private int _currScene;
        private string _currName;
        private readonly List<Transform> _roomList = new ();
        protected override void OnLoad()
        {
            base.OnLoad();

            _dpdMode.options = new List<TMP_Dropdown.OptionData>
            {
                new("全部"),
            };
            foreach (var name in Game.Model.Get<MConf>().GetModeNames())
            {
                _dpdMode.options.Add(new TMP_Dropdown.OptionData(name));
            }
            _dpdScene.options = new List<TMP_Dropdown.OptionData>
            {
                new("全部"),
            };
            foreach (var name in Game.Model.Get<MConf>().GetSceneTplNames())
            {
                _dpdScene.options.Add(new TMP_Dropdown.OptionData(name));
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            _btnSearch.onClick.AddListener(OnClickSearch);
            _btnPrev.onClick.AddListener(OnClickPrev);
            _btnNext.onClick.AddListener(OnClickNext);
            _btnNew.onClick.AddListener(OnClickNew);
            
            GetRoomList(_currPage);
        }

        protected override void OnHide()
        {
            _btnSearch.onClick.RemoveAllListeners();
            _btnPrev.onClick.RemoveAllListeners();
            _btnNext.onClick.RemoveAllListeners();
            _btnNew.onClick.RemoveAllListeners();
            base.OnHide();
        }

        private void OnClickSearch()
        {
            _currName = _iptName.text;
            _currMode = _dpdMode.value;
            _currScene = _dpdScene.value;
            GetRoomList(_currPage);
        }

        private void OnClickNew()
        {
            Game.View.GetView<VRoomList>().Hide();
            Game.View.GetView<VRoomNew>().Show();
        }

        private void OnClickNext()
        {
            GetRoomList(_currPage+1);
        }

        private void OnClickPrev()
        {
            if (_currMode == 0)
            {
                return;
            }

            GetRoomList(_currPage-1);
        }

        private void GetRoomList(int page)
        {
            Game.Ctrl.Get<CRoom>().GetRoomList(_currName, _currMode, _currScene,
                _currPage, 16, list =>
                {
                    if (list.Count == 0)
                    {
                        return;
                    }

                    _currPage = page;
                    ShowRoomList(list);
                });
        }

        private void ShowRoomList(List<Room> list)
        {
            foreach (var tnf in _roomList)
            {
                var gameObject = tnf.gameObject;
                gameObject.GetComponent<VDoubleClick>().OnDoubleClick.RemoveAllListeners();
                Recycle(gameObject);
            }
            
            var model = Game.Model.Get<MConf>();
            foreach (var room in list)
            {
                var tnf = Spawn<RectTransform>("Item");
                tnf.FindComp<TMP_Text>("TxtPlayers").text = $"{room.CurrPlayers}/{room.MaxPlayers}";
                tnf.FindComp<TMP_Text>("TxtName").text = room.Name;
                tnf.FindComp<TMP_Text>("TxtScene").text = model.GetSceneTplName(room.SceneTplId);
                tnf.FindComp<TMP_Text>("TxtMode").text = model.GetModeName(room.Mode);
                var roomId = room.Id;
                tnf.gameObject.AddComponent<VDoubleClick>().OnDoubleClick.AddListener(data =>
                {
                    Game.Ctrl.Get<CRoom>().EntryRoom(roomId);
                });
                _roomList.Add(tnf);
            }
        }
    }
}