using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Masuit.Tools.Security;
using AJWPFAdmin.Core.Properties;
using System.Diagnostics;
using AJWPFAdmin.Services.EF.Tables;

namespace AJWPFAdmin.Services.EF
{
    public partial class DbService : DbContext
    {

        public DbService(DbContextOptions<DbService> options)
            : base(options)
        {
        }


        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<PlatformCustomer> PlatformCustomers { get; set; }
        public DbSet<ShippingRecord> ShippingRecords { get; set; } 
        public DbSet<MaterialDictionary> MaterialDictionaries { get; set; }
        public DbSet<DatabaseBackupRecord> DatabaseBackupRecords { get; set; }
        public DbSet<Watchhouse> Watchhouses { get; set; }
        public DbSet<Passageway> Passageways { get; set; }
        public DbSet<Device> Devices { get; set; } 
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<SystemConfigDictionary> SystemConfigDictionaries { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehousePassageRecord> WarehousePassageRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var version = new MySqlServerVersion(ServerVersion.Parse("8.0.34-mysql"));
                optionsBuilder.UseMySql("server=localhost;user=akira;password=mysql2014!@akira;database=shipping_system;AllowLoadLocalInfile=true", version, builder =>
                {
                    builder.EnableRetryOnFailure();
                }).EnableDetailedErrors()
                .LogTo(log => Debug.WriteLine(log),
                     new[] {
                         DbLoggerCategory.Database.Command.Name,
                         DbLoggerCategory.Update.Name,
                         DbLoggerCategory.Query.Name
                     }
                    );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var now = DateTime.Now;

            modelBuilder.Entity<PlatformCustomer>()
                .HasData(new PlatformCustomer
                {
                    Id = 480306,
                    Name = "阿吉",
                    Director = "阿吉",
                    DirectorPhone = "15155555551",
                    Contact = "15155555551",
                    Address = "",
                    CreateDate = now,
                    ExpireDate = now.AddYears(1),
                    Enable = true
                });

            modelBuilder.Entity<SystemRole>()
                .HasData(new SystemRole
                {
                    Id = 480306,
                    PId = 480306,
                    Name = "管理员",
                    CreateDate = now,
                    UpdateDate = now,
                    Permission = "all"
                });

            modelBuilder.Entity<SystemUser>()
                .HasData(new SystemUser
                {
                    Id = 480306,
                    PId = 480306,
                    AccountName = "admin",
                    Phone = "15155555551",
                    Password = "123456".AESEncrypt(Resources.AESKey),
                    NickName = "admin",
                    Avatar = "",
                    Enable = true,
                    RoleId = 480306,
                    RoleName = "管理员",
                    CreateDate = now,
                    UpdateDate = now,
                });

            modelBuilder.Entity<CarType>()
                .HasData(new CarType
                {
                    Id = 480306,
                    PId = 480306,
                    Name = "固定车",
                    CreateDate = now,
                    SysRequired = true,
                    AutoPass = true
                });
            modelBuilder.Entity<CarType>()
                .HasData(new CarType
                {
                    Id = 480307,
                    PId = 480306,
                    Name = "临时车",
                    CreateDate = now,
                    SysRequired =  true
                });
            modelBuilder.Entity<CarType>()
                .HasData(new CarType
                {
                    Id = 480308,
                    PId = 480306,
                    Name = "台账车",
                    CreateDate = now,
                    SysRequired = true,
                    AutoPass = true
                });


        }
    }
}
