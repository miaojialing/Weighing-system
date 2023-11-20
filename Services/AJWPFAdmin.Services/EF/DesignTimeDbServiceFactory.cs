using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.EF
{
    public class DesignTimeDbServiceFactory : IDesignTimeDbContextFactory<DbService>
    {
        public DbService CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DbService>();
            var version = new MySqlServerVersion(ServerVersion.Parse("8.0.34-mysql"));
            builder.UseMySql("server=localhost;user=root;password=123456;database=shipping_system", version);
            return new DbService(builder.Options);
        }
    }
}
