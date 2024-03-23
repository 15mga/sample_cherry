using System.Collections.Generic;
using Cherry;
using Cherry.Attr;
using Cherry.SceneObj;
using Pb;
using Script.Comp;
using Script.Ctrl;
using Script.Model;
using UnityEngine;
using MScene = Script.Model.MScene;
using Object = UnityEngine.Object;

namespace Script.SceneObj
{
    public class SScenePawn : SceneObjBase
    {
        public const string N_Pawn_Curr = "pawn_curr";
        
        [AChild("SPawn")] private Transform _pawnRoot;
        
        private Transform _player;
        private string _playerId;

        private Dictionary<string, PawnTransform> _idToPawn = new();
        private int _currPawn;
        protected override void OnLoaded()
        {
            base.OnLoaded();

            BindNotice(MScene.N_InitTransform, OnInitTransform);
            BindNotice(SScenePrefab.N_Prefab_Complete, OnPrefabComplete);
            Game.Ctrl.Get<CConn>().BindPus<SceneEventPus>(OnEventPus);
        }

        protected override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        private void OnPrefabComplete(object obj)
        {
            var player = Game.Model.Get<MPlayer>().Player;
            _playerId = player.Id;
            var hero = player.Hero;
            var go = Game.Scene.GetObj<SScenePrefab>().InstantiatePlayer(hero);
            var pawnTnf = go.AddComponent<PawnTransform>();
            _player = go.transform;
            _player.SetParent(_pawnRoot);
            Game.Camera.CreatePlayerCamera(0, 45, _player, 48);
            go.SetActive(false);
            _idToPawn.Add(_playerId, pawnTnf);
            
            Game.Ctrl.Get<CScene>().EntryScene();
        }

        private void OnInitTransform(object obj)
        {
            var pos = Game.Model.Get<MScene>().PlayerPosition;
            _player.transform.position = pos.PbToVec3();
            _player.gameObject.SetActive(true);
        }

        private void OnEventPus(SceneEventPus evt)
        {
            var visible = new HashSet<string>();
            var invisible = new HashSet<string>();
            foreach (var sceneEvent in evt.Events)
            {
                switch (sceneEvent.EventCase)
                {
                    case SceneEvent.EventOneofCase.Invisible:
                        if (sceneEvent.Id == _playerId) continue;
                        invisible.Add(sceneEvent.Id);
                        break;
                    case SceneEvent.EventOneofCase.Visible:
                        if (sceneEvent.Id == _playerId) continue;
                        NewPawn(sceneEvent.Id,sceneEvent.Visible);
                        visible.Add(sceneEvent.Id);
                        break;
                    case SceneEvent.EventOneofCase.Movement:
                        UpdateMovement(sceneEvent.Id, sceneEvent.Movement);
                        break;
                }
            }

            foreach (var id in invisible)
            {
                //todo 服务端为节省性能会发出id重复的visible和invisible,需要处理，
                //todo 但有网络延迟的时候这里也不可靠，后期再另行处理
                if (visible.Contains(id) && IgnoreDispose(id)) continue;
                DisposePawn(id);
            }

            Game.Notice.DispatchNotice(N_Pawn_Curr, _currPawn);
        }

        private bool IgnoreDispose(string id)
        {
            if (!_idToPawn.TryGetValue(id, out var pawn)) return true;
            var pp = _player.position;
            var px = Mathf.FloorToInt(pp.x/64);
            var py = Mathf.FloorToInt(pp.y/64);
            var tp = pawn.transform.position;
            var tx = Mathf.FloorToInt(tp.x/64);
            var ty = Mathf.FloorToInt(tp.y/64);
            return Mathf.Abs(tx - px) <= 1 && Mathf.Abs(ty - py) <= 1;

        }

        private void UpdateMovement(string id, SceneMovement movement)
        {
            if (_idToPawn.TryGetValue(id, out var pawn))
            {
                pawn.PushMovement(movement);
            }
        }

        private void NewPawn(string id, SceneVisible visible)
        {
            if (_idToPawn.TryGetValue(id, out _)) return;
            _currPawn++;
            switch (visible.PawnTypeCase)
            {
                case SceneVisible.PawnTypeOneofCase.Player:
                    SpawnPlayer(id, visible);
                    break;
                case SceneVisible.PawnTypeOneofCase.Monster:
                    SpawnMonster(id, visible);
                    break;
            }
        }

        private void SpawnPlayer(string id, SceneVisible pawnEvt)
        {
            var hero = pawnEvt.Player.Hero;
            var go = Game.Scene.GetObj<SScenePrefab>().InstantiatePlayer(hero);
            go.transform.SetParent(_pawnRoot);
            var pawn = go.AddComponent<PawnTransform>();
            pawn.transform.position = pawnEvt.Position.PbToVec3();
            _idToPawn.Add(id, pawn);
        }

        private void SpawnMonster(string id, SceneVisible pawnEvt)
        {
            var tplId = pawnEvt.Monster.TplId;
            Game.Log.Debug($"spawn monster: {tplId}");
            var go = Game.Scene.GetObj<SScenePrefab>().InstantiateMonster(tplId);
            go.transform.SetParent(_pawnRoot);
            var pawn = go.AddComponent<PawnTransform>();
            pawn.transform.position = pawnEvt.Position.PbToVec3();
            _idToPawn.Add(id, pawn);
        }

        private void DisposePawn(string id)
        {
            if (_idToPawn.Remove(id, out var pawn))
            {
                _currPawn--;
                Object.Destroy(pawn.gameObject);
            }
        }
    }
}