using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Inzynierka
{
    class StatContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<MatchTeam> TeamGames { get; set; }
        public DbSet<MatchSolo> SoloGames { get; set; }
        public DbSet<Team> Teams { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=C:/DataBase/Statistics.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Map table names
            modelBuilder.Entity<Player>().ToTable("Players", "test");
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId);
                entity.HasIndex(e => e.NickName).IsUnique();
            });
            modelBuilder.Entity<MatchTeam>().ToTable("Team Matches", "test");
            modelBuilder.Entity<MatchTeam>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.Property(e => e.GameDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            modelBuilder.Entity<MatchSolo>().ToTable("Solo Matches", "test");
            modelBuilder.Entity<MatchSolo>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.Property(e => e.GameDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            modelBuilder.Entity<Team>().ToTable("Teams", "test");
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.TeamID);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
