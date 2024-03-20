using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf.Collections;
using Pb;
using Script.Model;
using UnityEngine;

namespace Script.Comp
{
    public class PawnTransform : MonoBehaviour
    {
        private Queue<SceneMovement> _movements = new();

        private bool _moving;

        private float _totalMs;

        private float _frameMs = 50f;

        [SerializeField] private float _scale = 3;
        
        public void PushMovement(SceneMovement movement)
        {
            _movements.Enqueue(movement);
            _totalMs += movement.Duration;
            if (!_moving)
            {
                ProcessMovement();
            }
        }

        private void ProcessMovement()
        {
            _moving = false;
            if (_movements.Count == 0)
            {
                return;
            }
            var movement = _movements.Dequeue();
            _moving = true;
            transform.DOKill();
            var pos = movement.Position.PbToVec3();
            var dur = (float)movement.Duration / 1000 * 0.6f;
            if (_totalMs > _frameMs)
            {
                dur *= _frameMs / _totalMs * _scale;
                // Game.Log.Debug($"{_totalMs} {_frameMs}");
            }

            _totalMs -= movement.Duration;
            transform.DOMove(pos, dur).SetEase(Ease.Linear).onComplete += ProcessMovement;
            // Game.Log.Debug($"process dur:{dur} pos:{pos}");
        }
    }
}