using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class SoloScoresDTO
    {
        [Required]
        public List<int> scores { get; set; }
        public int match_id { get; set; }
    } 
}
