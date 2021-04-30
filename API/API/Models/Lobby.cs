using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Lobby
    {
        public string lobby_ID { get; set; }
        public int max_size { get; set; }
        public double AvgSkill { get; set; }
        public List<Player> queue { get; set; }
        public Lobby(Player player, string id,int size,double skill)
        {
            lobby_ID = id+ DateTime.Now;
            max_size = size;
            queue=new List<Player> { player };
            AvgSkill = skill;
        }
    }
}
