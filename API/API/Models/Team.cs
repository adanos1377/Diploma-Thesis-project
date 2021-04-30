using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Team
    {
        [Key]
        public int TeamID { get; }
        [Required]
        public string PlayersID { get; set; }
    }
}
