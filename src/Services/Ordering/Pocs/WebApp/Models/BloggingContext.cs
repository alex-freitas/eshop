using System;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        
        public DbSet<Post> Posts { get; set; }

        public DbSet<Owner> Owner { get; set; }

        public DbSet<BlogStatus> BlogStatus { get; set; }

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
                
                entity.Property<int?>("OwnerId").IsRequired(false);                
            });

            modelBuilder.Entity<Blog>().OwnsOne(e => e.Settings);

            modelBuilder.Entity<Blog>()
                .HasMany(e => e.Posts)
                .WithOne();

            modelBuilder.Entity<Blog>()
                .HasOne<Owner>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("OwnerId");

            modelBuilder.Entity<Blog>()
                .HasOne(e => e.BlogStatus)
                .WithMany()
                .HasForeignKey("BlogStatusId");

            modelBuilder.Entity<Post>().ToTable("Posts", "blog");

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.PostId);
                entity.Property(e => e.PostId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Title).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });




            modelBuilder.Entity<Owner>().ToTable("Owner", "blog");

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name);
            });





            base.OnModelCreating(modelBuilder);
        }
    }
}