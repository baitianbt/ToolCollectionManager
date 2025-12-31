using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using ToolCollectionManager.Models;

namespace ToolCollectionManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SoftwareItem> SoftwareItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Screenshot> Screenshots { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appFolder = Path.Combine(appData, "ToolCollectionManager");
                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }
                string dbPath = Path.Combine(appFolder, "tool_collection.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<SoftwareItem>()
                .HasOne(s => s.Category)
                .WithMany(c => c.SoftwareItems)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading delete for categories

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Software)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Screenshot>()
                .HasOne(s => s.Software)
                .WithMany(soft => soft.Screenshots)
                .HasForeignKey(s => s.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Seed initial categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "开发工具", Color = "#0078D4" },
                new Category { Id = 2, Name = "系统工具", Color = "#107C10" },
                new Category { Id = 3, Name = "媒体工具", Color = "#FF8C00" },
                new Category { Id = 4, Name = "办公工具", Color = "#8E8CD8" },
                new Category { Id = 5, Name = "网络工具", Color = "#00BCF2" }
            );
        }
    }
}