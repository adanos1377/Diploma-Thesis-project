using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Server
    {
        public string adress { get; set; }// = "127.0.0.1";
        public int port { get; set; }
        public string lobby_ID { get; set; }
        public int max_size { get; set; }
        public Server(string lobby_id,GameConfigs config)
        {
            lobby_ID = lobby_id;
            max_size = config.NumberOfPlayers;
            string[] subs = config.Server.Split(':');
            adress = subs[0];
            port = Int32.Parse(subs[1]);
        }
    }
}
