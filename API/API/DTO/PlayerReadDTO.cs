using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class PlayerReadDTO
    {
        public string Nickname { get; set; }
       // public string Rank { get; set; }
        //new
        public double SkillRating { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesTied { get; set; }
        public int GamesLost { get; set; }
        public double WinRate { get; set; }
        public string Rank { get; set; }
        public string ApiName { get; set; }
    }
}
