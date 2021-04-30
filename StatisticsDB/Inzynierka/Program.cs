using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace Inzynierka
{
    class Lobby
    {
        public int LobbyID;
        public List<Player> players;
    }

    class Program
    {
        public static void Create_DataBase(string dbName, int game_id)
        {
            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }
            //create database
            using (var dbContext = new StatContext())
            {
                  dbContext.Database.EnsureCreated();
                  dbContext.SaveChanges();
            }
        }
        public static void Add_Player(string nick, int game_id)
        {
            using (var dbContext = new StatContext())
            {
                 using (var config = new ConfigContext())
                 {
                    var game = config.games.First(g => g.ConfigId == game_id);
                    dbContext.Players.Add(new Player { NickName = nick, SkillRating = game.StartRating, GamesPlayed = 0, GamesWon = 0, GamesTied = 0, GamesLost = 0, WinRate = 0.0, Rank = "Bronze" });
                 }
                 //check if tie games are allowed
                 dbContext.SaveChanges();
            }
        }

        public static int Add_Team(List<int> players)
        {
            int result;
            using (var dbContext = new StatContext())
            {
                dbContext.Teams.Add(new Team { PlayersID = JsonSerializer.Serialize(players) });
                result = dbContext.Teams.Count();
                dbContext.SaveChanges();
            }
            return result;
        }

        public static void Add_Game_Team(List<int> teams, bool ranked)
        {
            using (var dbContext = new StatContext())
            {
                List<int> scores = new List<int>();
                foreach (var p in teams) scores.Add(0);
                dbContext.TeamGames.Add(new MatchTeam { Teams = JsonSerializer.Serialize(teams), GameDate = DateTime.Today, Scores = JsonSerializer.Serialize(scores), RankedGame = ranked, Finished = false });
                dbContext.SaveChanges();
            }
        }

        public static void Add_Game_Solo(List<int> players, bool ranked)
        {
            using (var dbContext = new StatContext())
            {
                List<int> scores = new List<int>();
                foreach (var p in players) scores.Add(0);
                dbContext.SoloGames.Add(new MatchSolo { Players = JsonSerializer.Serialize(players), GameDate = DateTime.Today, Scores = JsonSerializer.Serialize(scores), RankedGame = ranked, Finished = false });
                dbContext.SaveChanges();
            }
        }

        public static void Add_Result_Team(int game_id, List<int>scores, int config_id)
        {
            using (var dbContext = new StatContext())
            {
                var game = dbContext.TeamGames.First(g => g.GameId == game_id);
                game.Add_Result(scores);
                if (game.RankedGame)
                {
                    List<double> skills = new List<double>();
                    List<int> tids = JsonSerializer.Deserialize<List<int>>(game.Teams);
                    //fill skills list

                    foreach(var tid in tids)
                    {
                        var team = dbContext.Teams.First(t => t.TeamID == tid);
                        double avg_skills = 0;
                        List<int> team_players = JsonSerializer.Deserialize <List<int>>(team.PlayersID);
                        foreach(var pid in team_players)
                        {
                            var player = dbContext.Players.First(p => p.PlayerId == pid);
                            avg_skills += player.SkillRating;
                        }
                        avg_skills = avg_skills / team_players.Count;
                        skills.Add(avg_skills);
                    }

                    bool ties = false;

                    List<double> ranking_updates = new List<double>();
                    List<int> ranks = new List<int>();
                    using (var config = new ConfigContext())
                    {
                        var game_config = config.games.First(g => g.ConfigId == config_id);
                        ranking_updates = game.CountRanking(scores, skills, game_config.KValue, game_config.PktsRatio);
                        ranks = JsonSerializer.Deserialize<List<int>>(game_config.RanksLimit);
                        if (game_config.TieGames) ties = true;
                    }
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
                        var team = dbContext.Teams.First(t => t.TeamID == tid);
                        List<int> team_players = JsonSerializer.Deserialize<List<int>>(team.PlayersID);

                        if (ties)
                        {
                            if (highest.Contains(i) & highest.Count < 2)
                            {
                                foreach (var pid in team_players)
                                {
                                    var player = dbContext.Players.First(p => p.PlayerId == pid);
                                    player.Update_Stats(ranking_updates[i], 1);
                                    player.Update_Rank(ranks);
                                }
                                i++;
                            }
                            else if(highest.Contains(i) & highest.Count > 1)
                            {
                                foreach (var pid in team_players)
                                {
                                    var player = dbContext.Players.First(p => p.PlayerId == pid);
                                    player.Update_Stats(ranking_updates[i], 0);
                                    player.Update_Rank(ranks);
                                }
                                i++;
                            }
                            else
                            {
                                foreach (var pid in team_players)
                                {
                                    var player = dbContext.Players.First(p => p.PlayerId == pid);
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
                                    var player = dbContext.Players.First(p => p.PlayerId == pid);
                                    player.Update_Stats(ranking_updates[i], 1);
                                    player.Update_Rank(ranks);
                                }
                                i++;
                            }
                            else
                            {
                                foreach (var pid in team_players)
                                {
                                    var player = dbContext.Players.First(p => p.PlayerId == pid);
                                    player.Update_Stats(ranking_updates[i], 2);
                                    player.Update_Rank(ranks);
                                }
                                i++;
                            }
                        }
                    }
                }
                dbContext.SaveChanges();
            }
        }

        public static void Add_Result_Solo(int game_id, List<int> scores, int config_id)
        {
            using (var dbContext = new StatContext())
            {
                var game = dbContext.SoloGames.First(g => g.GameId == game_id);
                game.Add_Result(scores);
                if (game.RankedGame)
                {
                    List<double> skills = new List<double>();
                    List<int> pids = JsonSerializer.Deserialize<List<int>>(game.Players);
                    //fill skills list
                    foreach (var pid in pids)
                    {
                        var player = dbContext.Players.First(p => p.PlayerId == pid);
                        skills.Add(player.SkillRating);
                    }
                    List<double> ranking_updates = new List<double>();

                    bool ties = false;
                    List<int> ranks = new List<int>();

                    using (var config = new ConfigContext())
                    {
                        var game_config = config.games.First(g => g.ConfigId == config_id);
                        ranking_updates = game.CountRanking(scores, skills, game_config.KValue, game_config.PktsRatio);
                        ranks = JsonSerializer.Deserialize<List<int>>(game_config.RanksLimit);
                        if (game_config.TieGames) ties = true;
                    }

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
                        var player = dbContext.Players.First(p => p.PlayerId == pid);

                        if (ties)
                        {
                            if (highest.Contains(i) & highest.Count < 2) player.Update_Stats(ranking_updates[i++], 1);
                            else if(highest.Contains(i) & highest.Count > 1) player.Update_Stats(ranking_updates[i++], 0);
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
                dbContext.SaveChanges();
            }
        }

        public static int Find_Opponent_Solo(int id, List<Lobby>rooms, int skilllimit)
        {
            int result = -1;
            using (var dbContext = new StatContext())
            {
                var player = dbContext.Players.First(p => p.PlayerId == id);
                foreach(var r in rooms)
                {
                    double skillMean = 0;
                    double skillSum = 0;
                    foreach (var p in r.players) skillSum += p.SkillRating;
                    skillMean = skillSum / r.players.Count;
                    if(skillMean-player.SkillRating < skilllimit)
                    {
                        result = r.LobbyID;
                        break;
                    }
                }
            }
            return result;
        }

        public static int Find_Opponent_Team(int id, List<Lobby> rooms, int skilllimit)
        {
            int result = -1;
            using (var dbContext = new StatContext())
            {
                var player = dbContext.Players.First(p => p.PlayerId == id);
                foreach (var r in rooms)
                {
                    double skillMean = 0;
                    double skillSum = 0;
                    foreach (var p in r.players) skillSum += p.SkillRating;
                    skillMean = skillSum / r.players.Count;
                    if (skillMean - player.SkillRating < skilllimit)
                    {
                        result = r.LobbyID;
                        break;
                    }
                }
            }
            return result;
        }

        public static List<int> Manage_Teams(Lobby lobby, int n_teams)
        {
            List<int> result = new List<int>();

            List<List<Player>> teams = new List<List<Player>>();
            List<Player> sortedPlayers = lobby.players.OrderBy(p => p.SkillRating).ToList();
            int iterations = lobby.players.Count / n_teams;
            bool front = true;

            for (int i = 0; i < n_teams; i++) teams.Add(new List<Player>());

            for(int i = 0; i < iterations; i++)
            {
                if (front)
                {
                    for(int j = 0; j < n_teams; j++)
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
                        sortedPlayers.RemoveAt(sortedPlayers.Count-1);
                    }
                }
            }

            List<int> members = new List<int>();

            foreach (var tTeam in teams)
            {
                foreach(var player in tTeam)
                {
                    members.Add(player.PlayerId);
                }
                result.Add(Add_Team(members));
                members.Clear();
            }

            return result;
        }
        static void Main(string[] args)
        {
            string dbName = "C:/DataBase/Statistics.db";
            Create_DataBase(dbName, 1);
            
            Add_Player("roman", 2);
            Add_Player("arek", 2);
            
            /*
            //solo test
            List<int> players = new List<int>();
            players.Add(1);
            players.Add(2);
            players.Add(3);

            List<int> scores = new List<int>();
            scores.Add(10);
            scores.Add(5);
            scores.Add(5);

            Add_Game_Solo(players, true);
            Add_Result_Solo(1, scores, 1);*/

            //solo test2
            List<int> players = new List<int>();
            players.Add(1);
            players.Add(2);

            List<int> scores = new List<int>();
            scores.Add(10000);
            scores.Add(0);

            Add_Game_Solo(players, true);
            Add_Result_Solo(1, scores, 2);

            //team test
            /*
            List<int> teamA = new List<int>();
            List<int> teamB = new List<int>();

            teamA.Add(1);
            teamA.Add(2);
            Add_Team(teamA);

            teamB.Add(3);
            teamB.Add(4);
            Add_Team(teamB);

            List<int> teams = new List<int>();
            teams.Add(1);
            teams.Add(2);

            Add_Game_Team(teams, true);

            List<int> scores = new List<int>();
            scores.Add(10);
            scores.Add(5);

            Add_Result_Team(1, scores, 1);
            */
            /*
            //manage teams test
            Lobby lobby = new Lobby();
            lobby.players = new List<Player>();

            for(int i = 1; i <= 4; i++)
            {
                using (var dbContext = new StatContext())
                {
                    var game = dbContext.Players.First(p => p.PlayerId == i);
                    lobby.players.Add(game);
                }
            }
            
            List<int> teams = Manage_Teams(lobby, 2);
            */
            Console.WriteLine("OK");
        }
    }
}
