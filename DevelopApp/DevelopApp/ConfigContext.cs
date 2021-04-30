using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DevelopApp
{
    class ConfigContext : DbContext
    {
        public DbSet<GameConfig> games { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=C:/DataBase/Games.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameConfig>().ToTable("Games", "test");
            modelBuilder.Entity<GameConfig>(entity =>
            {
                entity.HasKey(e => e.ConfigId);
                entity.HasIndex(e => e.Name).IsUnique();
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
