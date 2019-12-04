using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./blog.db");
            base.OnConfiguring(optionsBuilder);           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().ToTable("Blogs", "blog");

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.BlogId);
                entity.Property(e => e.BlogId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Title).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Blog>().OwnsOne(e => e.Settings);

            modelBuilder.Entity<Blog>()
                .HasMany(e => e.Posts)
                .WithOne();

            modelBuilder.Entity<Post>().ToTable("Posts", "blog");

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.PostId);
                entity.Property(e => e.PostId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Title).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}