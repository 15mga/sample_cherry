using Cherry;
using Cherry.Model;
using Pb;

namespace Script.Model
{
    public class MScene : ModelBase
    {
        public const string N_InitTransform = "init_transform";
        public Scene Scene { get; private set; }
        public Vector2 PlayerPosition { get; private set; }
        public SceneConf Conf { get; private set; }
        
        public void SetScene(Scene scene)
        {
            Scene = scene;
            Conf = new SceneConf
            {
                Width = 1024,
                Height = 1024,
            };
        }

        public void InitPlayerPosition(Vector2 position)
        {
            PlayerPosition = position;
            Game.Notice.DispatchNotice(N_InitTransform);
        }

        public void UpdateTransform(SceneTransformEvt evt)
        {
            // var pos = evt.Transform.Position;
            // var pawnId = evt.PawnId;
            // //todo 这里改为逐步移动，后期考虑改为实体组件的形式
            // var movements = evt.Transform.Movements;
            // var l = movements.Count;
            // if (l > 0)
            // {
            //     SceneMovement last = null;
            //     var lastIdx = l - 1;
            //     if (l > 1)
            //     {
            //         for (var i = 0; i < lastIdx; i++)
            //         {
            //             var prev = movements[i];
            //             var next = movements[i+1];
            //             pos = UpdatePosition(next.Timestamp - prev.Timestamp, prev, pos);
            //         }
            //     }
            //     last = movements[lastIdx];
            //     var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //     pos = UpdatePosition(now - last.Timestamp, last, pos);
            // }
            // Game.Log.Debug($"pos:{pos.PbToVec2().ToString()}");
            // OnUpdatePlayerPosition?.Invoke(pawnId, pos.PbToVec2());
        }

        private Vector2 UpdatePosition(long dur, SceneMovement movement, Pb.Vector2 pos)
        {
            if (IsStop(movement))
            {
                return pos;
            }

            var secs = (float)dur / 1000;
            var dir = movement.Direction.PbToVec2();
            var offset = dir * movement.MoveSpeed * secs;
            return (pos.PbToVec2() + offset).Vec2ToPb();
        }

        private bool IsStop(SceneMovement movement)
        {
            return movement.MoveSpeed == 0 || movement.Direction == null;
        }
    }

    public class SceneConf
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}