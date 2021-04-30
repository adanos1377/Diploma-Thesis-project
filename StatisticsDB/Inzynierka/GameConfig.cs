using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Inzynierka
{
    class GameConfig
    {
        [Key]
        public int ConfigId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Server { get; set; }
        public int NumberOfPlayers { get; set; }
        public int AvgTime { get; set; }
        public bool TieGames { get; set; }
        public int NumberOfRanks { get; set; }
        public int KValue { get; set; }
        public bool PktsRatio { get; set; }
        public int StartRating { get; set; }
        public int MatchmakingLimit { get; set; }
        public string RanksLimit { get; set; }
    }
}
