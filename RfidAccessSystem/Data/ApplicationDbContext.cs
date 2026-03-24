using Microsoft.EntityFrameworkCore;
using RfidAccessSystem.Models.Entities;

namespace RfidAccessSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<AccessRule> AccessRules { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users Table
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UID).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.HasIndex(e => e.UID).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // AccessLog Table
            modelBuilder.Entity<AccessLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.UID).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccessType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(255);
                entity.Property(e => e.IPAddress).HasMaxLength(50);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.AccessLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.UID);
                entity.HasIndex(e => e.AccessTime);
                entity.HasIndex(e => e.Status);
            });

            // ApiKey Table
            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.HasKey(e => e.ApiKeyId);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.KeyValue).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SecretKey).HasMaxLength(500);
                entity.HasIndex(e => e.KeyValue).IsUnique();
            });

            // AccessRule Table
            modelBuilder.Entity<AccessRule>(entity =>
            {
                entity.HasKey(e => e.RuleId);
                entity.Property(e => e.AllowedDays).HasMaxLength(100);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.AccessRules)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AuditLog Table
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditId);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ChangedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IPAddress).HasMaxLength(50);
                entity.HasIndex(e => e.ChangedAt);
            });
        }
    }
}
