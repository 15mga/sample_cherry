using System;
using Cherry.Model;
using Pb;

namespace Script.Model
{
    public class MPlayer : ModelBase
    {
        public Player Player { get; private set; }
        public ReconnectData Reconnect { get; private set; }

        public void SetPlayer(Player player)
        {
            Player = player;
        }

        public void SetReconnect(ReconnectData data)
        {
            Reconnect = data;
        }
        
        public class ReconnectData
        {
            public string RoomId;
            public string SceneId;
        }
    }
}