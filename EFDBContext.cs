using ArtShareServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer
{
    public sealed class EFDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        
        // TODO: потім змінити на інтерфейс, і весь контент зберігати в одній таблиці(?)
        public DbSet<Content> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<ContentLike> Likes { get; set; }
        public DbSet<ContentDislike> Dislikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<CommentDislike> CommentDislikes { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<ContentReport> ContentReports { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }

        public EFDBContext(DbContextOptions<EFDBContext> options): base(options) {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Following>().HasOne(f => f.FollowUser).WithMany(u => u.Followers).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Following>().HasOne(f => f.FollowingUser).WithMany(u => u.Followings).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.ContentReports).WithOne(r => r.User)
                    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.CommentReports).WithOne(r => r.User)
                    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.CommentDislikes).WithOne(d => d.User)
                    .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasMany(u => u.CommentLikes).WithOne(d => d.User)
                    .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasOne(u => u.Avatar).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasMany(u => u.Likes).WithOne(l => l.User).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.Dislikes).WithOne(l => l.User).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.Votes).WithOne(l => l.User).OnDelete(DeleteBehavior.Restrict);
        }
    }
}