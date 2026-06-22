using Microsoft.EntityFrameworkCore;
using AMD_Course_Work.Models;

namespace AMD_Course_Work.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Poll> Polls { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Poll>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Poll>()
                .HasMany(p => p.Options)
                .WithOne(o => o.Poll)
                .HasForeignKey(o => o.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Poll>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Option>()
                .Property(o => o.VoteCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.PollId, v.VoterToken })
                .IsUnique();
        }
    }
}