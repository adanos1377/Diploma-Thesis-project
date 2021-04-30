using API.DTO;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public interface IPoczekalnia
    {
        Lobby find_lobby(Player player, GameConfigs config);
        bool remove_player_from_lobby(string nick,string lobby_id);
        void add_lobby_to_pool(Lobby lobby);
        void remove_lobby_from_pool(string lobby_id);
    }
}
