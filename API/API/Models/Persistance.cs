using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class LobbyCache
    {
        static LobbyCache _instance = new LobbyCache();

        static LobbyCache()
        {
        }
        LobbyCache()
        {
        }

        public static LobbyCache Instance
        {
            get { return _instance; }
        }

        private List<Lobby> _lobbies = new List<Lobby>();
        public List<Lobby> Lobbies
        {
            get { return _lobbies; }
        }
    }
}
