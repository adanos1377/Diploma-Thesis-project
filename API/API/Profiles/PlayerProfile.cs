using API.DTO;
using API.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            //Source to target
            CreateMap<Player, PlayerReadDTO>();
            CreateMap<PlayerCreateDTO, Player>();
        }
    }
}
