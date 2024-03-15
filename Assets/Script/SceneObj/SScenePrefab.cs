using System;
using System.Collections.Generic;
using Cherry;
using Cherry.Attr;
using Cherry.Extend;
using Cherry.SceneObj;
using Script.Model;
using UnityEngine;

namespace Script.SceneObj
{
    public class SScenePrefab : SceneObjBase
    {
        public const string N_Prefab_Complete = "prefab_complete";
        [AChild("SPrefab")] private Transform _prefabRoot;
        private readonly List<Transform> _pawns = new();
        protected override void OnLoaded()
        {
            base.OnLoaded();

            var mConf = Game.Model.Get<MConf>();
            var count = 0;
            foreach (var hero in mConf.Heros())
            {
                foreach (var level in mConf.HeroLevels())
                {
                    count++;
                    var name = $"{hero}_{level}";
                    Game.Asset.Spawn(name, o =>
                    {
                        o.name = name;
                        o.transform.SetParent(_prefabRoot);
                        BindPool(o.name);
                        count--;
                        if (count == 0)
                        {
                            Game.Notice.DispatchNotice(N_Prefab_Complete);
                        }
                    });
                }
            }
        }

        protected override void OnUnloaded()
        {
            foreach (var pawn in _pawns)
            {
                Recycle(pawn);
            }
            _pawns.Clear();
            base.OnUnloaded();
        }

        public GameObject InstantiatePlayer(int hero)
        {
            var name = Game.Model.Get<MConf>().GetHeroName(hero);
            return Spawn(name + "_Small");
        }

        public GameObject InstantiateMonster(string tplId)
        {
            //todo 暂时使用玩家模型
            return Spawn(tplId);
        }
    }
}