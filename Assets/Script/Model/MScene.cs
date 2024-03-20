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
    }

    public class SceneConf
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}