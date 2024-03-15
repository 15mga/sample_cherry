using System.Collections.Generic;
using Cherry.Model;
using Pb;

namespace Script.Model
{
    public class MConf : ModelBase
    {
        private static readonly List<string> _sceneTplNames = new()
        {
            new string("场景1"),
            new string("场景2"),
            new string("场景3"),
            new string("场景4"),
        };
        private static readonly List<string> _modeNames = new()
        {
            new string("PVE"),
            new string("PVP"),
            new string("人机"),
            new string("战争")
        };
        private static readonly List<int> _maxPlayers = new()
        {
            1,
            3,
            5,
            8
        };

        private static readonly List<string> _heros = new()
        {
            "Catcher",
            "Fishguard",
            "Imp",
            "Knight",
            "Monkeydong",
            "Nosedman",
            "Pitboy",
            "Spike",
            "Treestor",
            "Wedger",
        };

        private static readonly List<string> _heroLevels = new()
        {
            "Small",
            "Medium",
            "Big",
        };
        
        public string GetSceneTplName(int idx)
        {
            return _sceneTplNames[idx];
        }

        public List<string> GetSceneTplNames()
        {
            return _sceneTplNames;
        }
        public string GetModeName(ESceneMode idx)
        {
            return _modeNames[(int)idx];
        }

        public List<string> GetModeNames()
        {
            return _modeNames;
        }
        public int GetMaxPlayer(int idx)
        {
            return _maxPlayers[idx];
        }
        public List<int> GetMaxPlayers()
        {
            return _maxPlayers;
        }

        public string GetHeroName(int idx)
        {
            return _heros[idx];
        }

        public List<string> Heros()
        {
            return _heros;
        }

        public List<string> HeroLevels()
        {
            return _heroLevels;
        }
    }
}