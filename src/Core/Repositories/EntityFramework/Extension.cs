using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using Bit.Core.Models.EntityFramework;
using TableModel = Bit.Core.Models.Table;

namespace Bit.Core.Repositories.EntityFramework.Migration
{
    static public partial class MigrationExtension
    {
        static public void MigrateSqlite(this IServiceCollection services, string ConnectionString)
        {
            IServiceProvider sp = null;
            services.AddDbContext<SqliteDatabaseContext>(builder => builder.UseSqlite(ConnectionString));
            sp = services.BuildServiceProvider().CreateScope().ServiceProvider;
            sp.GetService<SqliteDatabaseContext>().Database.Migrate();
            services.AddDbContext<DatabaseContext>(builder => builder.UseSqlite(ConnectionString));
        }

        static public void MigrateSql(this IServiceCollection services, string ConnectionString)
        {
            IServiceProvider sp = null;
            services.AddDbContext<DatabaseContext>(builder => builder.UseSqlServer(ConnectionString));
            sp = services.BuildServiceProvider().CreateScope().ServiceProvider;
            sp.GetService<DatabaseContext>().Database.Migrate();
        }
    }
}