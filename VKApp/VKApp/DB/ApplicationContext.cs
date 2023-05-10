using Microsoft.EntityFrameworkCore;
using VKApp.models;

namespace VKApp.DB
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserState> UserStates { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>().HasData(
                new UserGroup { Id = 1, Code = "Admin", Description = "some"},
                new UserGroup { Id = 2, Code = "User", Description = "some" }
            );

            modelBuilder.Entity<UserState>().HasData(
                new UserState { Id = 1, Code = "Active", Description = "some" },
                new UserState { Id = 2, Code = "Blocked", Description = "some" }
            );

            modelBuilder.Entity<User>()
             .HasOne(u => u.UserGroup)
             .WithMany(g => g.Users)
             .HasForeignKey(u => u.UserGroupId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserState)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.UserStateId);
        }
    }
}
