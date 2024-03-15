using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Cherry;
using Cherry.Attr;
using Cherry.SceneObj;
using Script.Ctrl;
using Script.Model;
using UnityEngine;

namespace Script.SceneObj
{
    public class SPlayerSelect : SceneObjBase
    {
        [AChild("SPlayerSelect")] private Transform _playerRoot;
        private readonly Dictionary<string, Transform> _nameToPlayer = new();
        protected override void OnLoaded()
        {
            base.OnLoaded();

            // var list = new List<Transform>();
            // for (var i = 0; i < _playerRoot.childCount; i++)
            // {
            //     var tnf = _playerRoot.GetChild(i);
            //     list.Add(tnf);
            //     _nameToPlayer.Add(tnf.name, tnf);
            // }
            // list.Sort((tnf1, tnf2) => 
            //     tnf1.localRotation.eulerAngles.y.CompareTo(tnf2.localRotation.eulerAngles.y));
            // var names = list.Select(item => item.name).ToList();

            
            var mConf = Game.Model.Get<MConf>();
            var heros = mConf.Heros();
            var level = mConf.HeroLevels()[0];
            var angle = 360 / heros.Count;
            for (var i = 0; i < heros.Count; i++)
            {
                var hero = heros[i];
                var root = new GameObject
                {
                    name = hero
                };
                root.transform.SetParent(_playerRoot);
                root.transform.localRotation = Quaternion.Euler(new Vector3(0, angle * i, 0));
                _nameToPlayer[hero] = root.transform;
                Game.Asset.Spawn($"{hero}_{level}", o =>
                {
                    o.transform.SetParent(root.transform);
                    o.transform.localPosition = new Vector3(0, 0, -6);
                    o.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                });
            }

            BindNotice(CPlayer.N_PlayerShow, OnShow);
        }

        protected override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        private void OnShow(object obj)
        {
            var name = (string)obj;
            var euler = _nameToPlayer[name].localRotation.eulerAngles * -1;
            _playerRoot.DOLocalRotate(euler, 1f);
        }
    }
}