using API.DTO;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public interface ILiteRepo
    {

        Task<Player> GetPlayerInfo(string nick);
        Task<Player> AddPlayer(Player player, int game_id);
        Task<int> Add_Game_Solo(List<string> players, bool ranked);
        Task<int> Add_Game_Team(List<int> teams, bool ranked);
        Task<int> Add_Team(List<string> players);
        Task<int> Add_Result_Team(int game_id, List<int> scores, int config_id);
        Task<int> Add_Result_Solo(int game_id, List<int> scores, int config_id);
        Task<List<int>> Manage_Teams(Lobby lobby, int n_teams);
        void UpdatePlayer(Player player);
        void RemovePlayer(Player player);
    }
}
