using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class PlayerCreateDTO
    {
        [Required]
        [MaxLength(16)]
        public string NickName { get; set; }



    }
}
