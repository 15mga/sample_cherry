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
            var hero = player.Hero;
            var go = Game.Scene.GetObj<SScenePrefab>().InstantiatePlayer(hero);
            var pawnTnf = go.AddComponent<PawnTransform>();
            _player = go.transform;
            _player.SetParent(_pawnRoot);
            Game.Camera.CreatePlayerCamera(0, 45, _player, 48);
            go.SetActive(false);
            _idToPawn.Add(player.Id, pawnTnf);
            
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
            foreach (var pawnEvt in evt.Visible)
            {
                NewPawn(pawnEvt);
            }
            
            foreach (var transformEvt in evt.Transform)
            {
                UpdateTransform(transformEvt);
            }

            foreach (var invisibleEvt in evt.Invisible)
            {
                DisposePawn(invisibleEvt);
            }

            Game.Notice.DispatchNotice(N_Pawn_Curr, _currPawn);
        }

        private void UpdateTransform(SceneTransformEvt transformEvt)
        {
            if (_idToPawn.TryGetValue(transformEvt.PawnId, out var pawn))
            {
                pawn.PushMovement(transformEvt.Movement);
            }
        }

        private void NewPawn(ScenePawnEvt pawnEvt)
        {
            if (_idToPawn.TryGetValue(pawnEvt.PawnId, out _)) return;
            _currPawn++;
            switch (pawnEvt.PawnTypeCase)
            {
                case ScenePawnEvt.PawnTypeOneofCase.Player:
                    SpawnPlayer(pawnEvt);
                    break;
                case ScenePawnEvt.PawnTypeOneofCase.Monster:
                    SpawnMonster(pawnEvt);
                    break;
            }
        }

        private void SpawnPlayer(ScenePawnEvt pawnEvt)
        {
            var hero = pawnEvt.Player.Hero;
            var go = Game.Scene.GetObj<SScenePrefab>().InstantiatePlayer(hero);
            go.transform.SetParent(_pawnRoot);
            var pawn = go.AddComponent<PawnTransform>();
            pawn.transform.position = pawnEvt.Position.PbToVec3();
            _idToPawn.Add(pawnEvt.PawnId, pawn);
        }

        private void SpawnMonster(ScenePawnEvt pawnEvt)
        {
            var tplId = pawnEvt.Monster.TplId;
            Game.Log.Debug($"spawn monster: {tplId}");
            var go = Game.Scene.GetObj<SScenePrefab>().InstantiateMonster(tplId);
            go.transform.SetParent(_pawnRoot);
            var pawn = go.AddComponent<PawnTransform>();
            pawn.transform.position = pawnEvt.Position.PbToVec3();
            _idToPawn.Add(pawnEvt.PawnId, pawn);
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