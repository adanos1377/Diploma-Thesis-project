using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class MatchSoloCreate
    {
        [Required]
        public List<string> players { get; set; }
        [Required]
        public bool ranked { get; set; }
        [Required]
        public string lobby_id { get; set; }
    } 
}
