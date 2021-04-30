using API.DTO;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class ConfigRepo : IConfig
    {
        private readonly ConfigContext config;

        public ConfigRepo(ConfigContext Config)
        {
            config = Config;
        }

        public async Task<GameConfigs> GetConfig(int id)
        {
            return await config.games.FirstAsync(c=>c.ConfigId==id);
        }
    }
}