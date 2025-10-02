using IoTFarmSystem.DeviceManagement.Domain.Aggregates;
using IoTFarmSystem.DeviceManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Infrastructure.Persistance
{
    public class DeviceManagementDbContext : DbContext
    {
        public DeviceManagementDbContext(DbContextOptions<DeviceManagementDbContext> options)
            : base(options) { }

        // Aggregates
        public DbSet<Device> Devices { get; set; }

        // Entities
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<ConnectionLog> ConnectionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================
            // Device (Aggregate Root)
            // ========================
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(d => d.Id);

                // Required properties
                entity.Property(d => d.DeviceName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(d => d.SerialNumber)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(d => d.SerialNumber)
                      .IsUnique();

                entity.Property(d => d.TenantId)
                      .IsRequired();

                entity.HasIndex(d => d.TenantId);

                entity.Property(d => d.AzureIotDeviceId)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(d => d.ConnectionString)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(d => d.DeviceType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(d => d.Status)
                      .IsRequired()
                      .HasMaxLength(50);

                // Optional properties
                entity.Property(d => d.Location)
                      .HasMaxLength(300);

                entity.Property(d => d.Description)
                      .HasMaxLength(1000);

                // Specifications
                entity.Property(d => d.LightSpectrum)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(d => d.Manufacturer)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(d => d.ModelNumber)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(d => d.FirmwareVersion)
                      .IsRequired()
                      .HasMaxLength(50);

                // Timestamps
                entity.Property(d => d.RegisteredAt)
                      .IsRequired();

                // Child entities - MaintenanceRecords
                entity.HasMany<MaintenanceRecord>("_maintenanceRecords")
                      .WithOne()
                      .HasForeignKey(m => m.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation("_maintenanceRecords")
                      .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.Ignore(d => d.MaintenanceRecords);

                // Child entities - ConnectionLogs
                entity.HasMany<ConnectionLog>("_connectionLogs")
                      .WithOne()
                      .HasForeignKey(c => c.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation("_connectionLogs")
                      .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.Ignore(d => d.ConnectionLogs);
            });

            // ========================
            // MaintenanceRecord (Entity)
            // ========================
            modelBuilder.Entity<MaintenanceRecord>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.DeviceId)
                      .IsRequired();

                entity.HasIndex(m => m.DeviceId);

                entity.Property(m => m.MaintenanceType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(m => m.Description)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(m => m.PerformedBy)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(m => m.PerformedAt)
                      .IsRequired();

                entity.Property(m => m.Notes)
                      .HasMaxLength(2000);

                entity.HasIndex(m => m.PerformedAt);
            });

            // ========================
            // ConnectionLog (Entity)
            // ========================
            modelBuilder.Entity<ConnectionLog>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.DeviceId)
                      .IsRequired();

                entity.HasIndex(c => c.DeviceId);

                entity.Property(c => c.EventType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(c => c.Message)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(c => c.Timestamp)
                      .IsRequired();

                entity.HasIndex(c => c.Timestamp);

                // Composite index for querying by device and time
                entity.HasIndex(c => new { c.DeviceId, c.Timestamp });
            });
        }
    }
}
