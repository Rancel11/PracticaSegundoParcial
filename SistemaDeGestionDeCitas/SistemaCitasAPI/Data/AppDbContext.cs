using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<ScheduleException> ScheduleExceptions { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Role Configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // ServiceCategory Configuration
            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.ToTable("ServiceCategories");
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            // Service Configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Services");
                entity.HasKey(e => e.ServiceId);
                entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Services)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Schedule Configuration
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedules");
                entity.HasKey(e => e.ScheduleId);
                entity.Property(e => e.DayOfWeek).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.Service)
                    .WithMany(s => s.Schedules)
                    .HasForeignKey(e => e.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment Configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointments");
                entity.HasKey(e => e.AppointmentId);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Appointments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Service)
                    .WithMany(s => s.Appointments)
                    .HasForeignKey(e => e.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                    .WithMany(s => s.Appointments)
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.Appointment)
                    .WithMany(a => a.Payments)
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(e => e.NotificationId);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Appointment)
                    .WithMany(a => a.Notifications)
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Type)
                    .WithMany(t => t.Notifications)
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // AuditLog Configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.AuditId);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TableName).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AppointmentStatus>(entity =>
            {
                entity.ToTable("AppointmentStatuses");
                entity.HasKey(e => e.StatusId);
                entity.Property(e => e.StatusName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ColorCode).HasMaxLength(7);
                entity.HasIndex(e => e.StatusName).IsUnique();
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.ToTable("NotificationTypes");
                entity.HasKey(e => e.TypeId);
                entity.Property(e => e.TypeName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.TypeName).IsUnique();
            });

            modelBuilder.Entity<ScheduleException>(entity =>
            {
                entity.ToTable("ScheduleExceptions");
                entity.HasKey(e => e.ExceptionId);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ExceptionType).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.Service)
                    .WithMany(s => s.ScheduleExceptions)
                    .HasForeignKey(e => e.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.ToTable("SystemSettings");
                entity.HasKey(e => e.SettingId);
                entity.Property(e => e.SettingKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SettingValue).IsRequired();
                entity.Property(e => e.DataType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.SettingKey).IsUnique();

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
