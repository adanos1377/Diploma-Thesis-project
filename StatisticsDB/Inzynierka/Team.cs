using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Inzynierka
{
    class Team
    {
        [Key]
        public int TeamID { get; }
        [Required]
        public string PlayersID {get; set;}
    }
}
