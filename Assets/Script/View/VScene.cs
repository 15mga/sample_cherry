using System;
using Cherry;
using Cherry.Attr;
using Cherry.View;
using Cherry.View.Event;
using Pb;
using Script.Ctrl;
using Script.SceneObj;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace Script.View
{
    public class VScene : ViewBase
    {
        [AChild("Center")] private Transform _ctrlCenter;
        [AChild("BtnAddRobot100")] private Button _btnAddRobot100;
        [AChild("BtnAddRobot1000")] private Button _btnAddRobot1000;
        [AChild("BtnClearRobot")] private Button _btnClearRobot;
        [AChild("TxtCurr")] private TMP_Text _txtCurr;
        [AChild("TxtTotal")] private TMP_Text _txtTotal;
        private Vector2 _centerPos;
        private VDrag _centerDrag;
        private int _currAngle;
        private const float _ctrlMaxDis = 120;
        private int _totalPawn;
        protected override void OnLoad()
        {
            base.OnLoad();

            _centerDrag = _ctrlCenter.gameObject.AddComponent<VDrag>();
            _centerPos = _ctrlCenter.position;
            _centerDrag.OnDragBegin.AddListener(OnDragBegin);
            _centerDrag.OnDragEnd.AddListener(OnDragEnd);
            _centerDrag.OnDragging.AddListener(OnDragging);
            _btnAddRobot100.onClick.AddListener(OnClickAddRobot100);
            _btnAddRobot1000.onClick.AddListener(OnClickAddRobot1000);
            _btnClearRobot.onClick.AddListener(OnClickClearRobot);
            ClearCurrAngle();
            BindNotice(SScenePawn.N_Pawn_Curr, OnPawnCurr);
        }

        private void OnPawnCurr(object obj)
        {
            _txtCurr.text = obj.ToString();
        }

        private void UpdatePawnTotal()
        {
            _txtTotal.text = _totalPawn.ToString();
        }

        private void ClearCurrAngle()
        {
            _currAngle = 1000;
        }

        protected override void OnUnload()
        {
            _centerDrag.OnDragBegin.RemoveAllListeners();
            _centerDrag.OnDragEnd.RemoveAllListeners();
            _centerDrag.OnDragging.RemoveAllListeners();
            _btnAddRobot100.onClick.RemoveAllListeners();
            _btnAddRobot1000.onClick.RemoveAllListeners();
            _btnClearRobot.onClick.RemoveAllListeners();
            base.OnUnload();
        }

        private void OnDragBegin(PointerEventData data)
        {
            _currAngle = 0;
        }

        private void OnDragEnd(PointerEventData data)
        {
            _ctrlCenter.localPosition = Vector3.zero;
            ClearCurrAngle();
            Game.Ctrl.Get<CScene>().Movement();
        }

        private void OnDragging(PointerEventData data)
        {
            var dis = Vector2.Distance(data.position, _centerPos);
            var dir = data.position - _centerPos;
            var normalized = dir.normalized;
            if (dis < _ctrlMaxDis)
            {
                _ctrlCenter.position = data.position;
            }
            else
            {
                _ctrlCenter.position = _centerPos + normalized * _ctrlMaxDis;
            }

            // 使用 Atan2 函数计算角度（以弧度表示）
            var angleRadians = Mathf.Atan2(dir.y, dir.x);
            // 将弧度转换为角度（以度数表示）
            var angle = (int)(angleRadians * Mathf.Rad2Deg);
            // if (angle % 2 == 1)
            // {
            //     angle -= 1;
            // }
            
            if (angle == _currAngle) return;
            _currAngle = angle;
            Game.Ctrl.Get<CScene>().Movement(20, new Pb.Vector2
            {
                X = normalized.x,
                Y = normalized.y,
            });
        }

        private void OnClickAddRobot100()
        {
            _totalPawn += 100;
            UpdatePawnTotal();
            Game.Ctrl.Get<CScene>().AddRobot(100, count =>
            {
                _totalPawn = count;
                UpdatePawnTotal();
            });
        }

        private void OnClickAddRobot1000()
        {
            _totalPawn += 5000;
            UpdatePawnTotal();
            Game.Ctrl.Get<CScene>().AddRobot(5000, count =>
            {
                _totalPawn = count;
                UpdatePawnTotal();
            });
        }

        private void OnClickClearRobot()
        {
            _totalPawn = 0;
            UpdatePawnTotal();
            Game.Ctrl.Get<CScene>().ClearRobot();
        }
    }
}