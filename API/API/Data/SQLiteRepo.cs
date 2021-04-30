using API.DTO;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class SQLiteRepo : ILiteRepo
    {
        private readonly SQLiteContext _context;
        private readonly ConfigContext config;

        public SQLiteRepo(SQLiteContext context, ConfigContext Config)
        {
            _context = context;
            config = Config;
        }

        public async Task<Player> AddPlayer(Player player, int game_id)
        {
            Player p;
            var game = config.games.First(g => g.ConfigId == game_id);
            if (game.TieGames)
            {
                p = new Player { NickName = player.NickName, Rank = "Bronze", SkillRating = game.StartRating, GamesPlayed = 0, GamesWon = 0, GamesLost = 0, GamesTied = 0, WinRate = 0.0 };
                await _context.PlayersDB.AddAsync(p);
            }
            else
            {
                p = new Player { NickName = player.NickName, Rank = "Bronze", SkillRating = game.StartRating, GamesPlayed = 0, GamesWon = 0, GamesLost = 0, WinRate = 0.0 };
                await _context.PlayersDB.AddAsync(p);
            }
            
            await _context.SaveChangesAsync();
            return p;
            
        }

        public async Task<Player> GetPlayerInfo(string nick)
        {
            return await _context.PlayersDB.FirstOrDefaultAsync(p => p.NickName == nick);
        }

        public async Task<int> Add_Game_Solo(List<string> players, bool ranked)
        {
            List<int> scores = new List<int>();
            foreach (var p in players) scores.Add(0);
            var m = new MatchSolo { Players = JsonSerializer.Serialize(players), GameDate = DateTime.Now, Scores = JsonSerializer.Serialize(scores), RankedGame = ranked, Finished = false };
            await _context.SoloGamesDB.AddAsync(m);
            await _context.SaveChangesAsync();
            return m.GameId;
        }

        public async Task<int> Add_Game_Team(List<int> teams, bool ranked)
        {
            List<int> scores = new List<int>();
            foreach (var p in teams) scores.Add(0);
            var m = new MatchTeam { Teams = JsonSerializer.Serialize(teams), GameDate = DateTime.Today, Scores = JsonSerializer.Serialize(scores), RankedGame = ranked, Finished = false };
            await _context.TeamGamesDB.AddAsync(m);
            await _context.SaveChangesAsync();
            return m.GameId;
        }
        public async Task<int> Add_Team(List<string> players)
        {
            var m = new Team { PlayersID = JsonSerializer.Serialize(players) };
            await _context.Teams.AddAsync(m);
            await _context.SaveChangesAsync();
            return m.TeamID;
        }

        public async Task<int> Add_Result_Team(int game_id, List<int> scores, int config_id)
        {
             var game = await _context.TeamGamesDB.FirstAsync(g => g.GameId == game_id);
             game.Add_Result(scores);
             if (game.RankedGame)
             {
                    List<double> skills = new List<double>();
                    List<int> tids = JsonSerializer.Deserialize<List<int>>(game.Teams);
                    //fill skills list

                    foreach (var tid in tids)
                    {
                        var team = await _context.Teams.FirstAsync(t => t.TeamID == tid);
                        double avg_skills = 0;
                        List<string> team_players = JsonSerializer.Deserialize<List<string>>(team.PlayersID);
                        foreach (var pid in team_players)
                        {
                            var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                            avg_skills += player.SkillRating;
                        }
                        avg_skills = avg_skills / team_players.Count;
                        skills.Add(avg_skills);
                    }

                    bool ties = false;

                    List<double> ranking_updates = new List<double>();
                    List<int> ranks = new List<int>();

                    var game_config = config.games.First(g => g.ConfigId == config_id);
                    ranking_updates = game.CountRanking(scores, skills, game_config.KValue, game_config.PktsRatio);
                    ranks = JsonSerializer.Deserialize<List<int>>(game_config.RanksLimit);
                    if (game_config.TieGames) ties = true;

                //count new skills

                    int i = 0;
                    List<int> highest = new List<int>();
                    int hresult = scores[0];
                    highest.Add(0);

                    for (int j = 1; j < scores.Count; j++)
                    {
                        if (scores[j] > hresult)
                        {
                            highest.Clear();
                            highest.Add(j);
                            hresult = scores[j];
                        }
                        else if (scores[j] == hresult) highest.Add(j);
                    }
                    //update all stats
                    foreach (var tid in tids)
                    {
                        var team = await _context.Teams.FirstAsync(t => t.TeamID == tid);
                        List<string> team_players = JsonSerializer.Deserialize<List<string>>(team.PlayersID);
                    if (ties)
                    {
                        if (highest.Contains(i) & highest.Count < 2)
                        {
                            foreach (var pid in team_players)
                            {
                                var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                                player.Update_Stats(ranking_updates[i], 1);
                                player.Update_Rank(ranks);
                            }
                            i++;
                        }
                        else if (highest.Contains(i) & highest.Count > 1)
                        {
                            foreach (var pid in team_players)
                            {
                                var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                                player.Update_Stats(ranking_updates[i], 0);
                                player.Update_Rank(ranks);
                            }
                            i++;
                        }
                        else
                        {
                            foreach (var pid in team_players)
                            {
                                var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                                player.Update_Stats(ranking_updates[i], 2);
                                player.Update_Rank(ranks);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        if (highest.Contains(i))
                        {
                            foreach (var pid in team_players)
                            {
                                var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                                player.Update_Stats(ranking_updates[i], 1);
                                player.Update_Rank(ranks);
                            }
                            i++;
                        }
                        else
                        {
                            foreach (var pid in team_players)
                            {
                                var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                                player.Update_Stats(ranking_updates[i], 2);
                                player.Update_Rank(ranks);
                            }
                            i++;
                        }
                    }
                        
                    }
             }
            var result=await _context.SaveChangesAsync();
            if (result > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public async Task<int> Add_Result_Solo(int game_id, List<int> scores, int config_id)
        {
                var game = await _context.SoloGamesDB.FirstAsync(g => g.GameId == game_id);
                game.Add_Result(scores);
                if (game.RankedGame)
                {
                    List<double> skills = new List<double>();
                    List<string> pids = JsonSerializer.Deserialize<List<string>>(game.Players);
                    //fill skills list
                    foreach (var pid in pids)
                    {
                        var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                        skills.Add(player.SkillRating);
                    }
                    List<double> ranking_updates = new List<double>();
                    bool ties = false;
                    List<int> ranks = new List<int>();

                    var game_config = config.games.First(g => g.ConfigId == config_id);
                    ranking_updates = game.CountRanking(scores, skills, game_config.KValue, game_config.PktsRatio);
                    ranks = JsonSerializer.Deserialize<List<int>>(game_config.RanksLimit);
                    if (game_config.TieGames) ties = true;

                //count new skills
                int i = 0;

                    //update ratings and stats
                    List<int> highest = new List<int>();
                    int hresult = scores[0];
                    highest.Add(0);

                    for (int j = 1; j < scores.Count; j++)
                    {
                        if (scores[j] > hresult)
                        {
                            highest.Clear();
                            highest.Add(j);
                            hresult = scores[j];
                        }
                        else if (scores[j] == hresult) highest.Add(j);
                    }

                    foreach (var pid in pids)
                    {
                        var player = await _context.PlayersDB.FirstAsync(p => p.NickName == pid);
                        if (game_config.TieGames)
                        {
                            if (highest.Contains(i) & highest.Count < 2) player.Update_Stats(ranking_updates[i++], 1);
                            else if (highest.Contains(i) & highest.Count > 1) player.Update_Stats(ranking_updates[i++], 0);
                            else player.Update_Stats(ranking_updates[i++], 2);
                            player.Update_Rank(ranks);
                        }
                        else
                        {
                            if (highest.Contains(i)) player.Update_Stats(ranking_updates[i++], 1);
                            else player.Update_Stats(ranking_updates[i++], 2);
                            player.Update_Rank(ranks);
                        }

                    }
                }
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public async Task<List<int>> Manage_Teams(Lobby lobby, int n_teams)
        {
            List<int> result = new List<int>();

            List<List<Player>> teams = new List<List<Player>>();
            List<Player> sortedPlayers = lobby.queue.OrderBy(p => p.SkillRating).ToList();
            int iterations = lobby.queue.Count / n_teams;
            bool front = true;

            for (int i = 0; i < n_teams; i++) teams.Add(new List<Player>());

            for (int i = 0; i < iterations; i++)
            {
                if (front)
                {
                    for (int j = 0; j < n_teams; j++)
                    {
                        teams[j].Add(sortedPlayers.First());
                        sortedPlayers.RemoveAt(0);
                    }
                }
                else
                {
                    for (int j = 0; j < n_teams; j++)
                    {
                        teams[j].Add(sortedPlayers.Last());
                        sortedPlayers.RemoveAt(sortedPlayers.Count - 1);
                    }
                }
            }

            List<string> members = new List<string>();

            foreach (var tTeam in teams)
            {
                foreach (var player in tTeam)
                {
                    members.Add(player.NickName);
                }
                var x = await Add_Team(members);
                result.Add(x);
                members.Clear();
            }

            return result;
        }
        public void RemovePlayer(Player player)
        {
            throw new NotImplementedException();
        }


        public void UpdatePlayer(Player player)
        {
            throw new NotImplementedException();
        }
    }
}