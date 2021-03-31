using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Bit.Core.Models.EntityFramework;
using TableModel = Bit.Core.Models.Table;

namespace Bit.Core.Repositories.EntityFramework
{
    public class SqliteContextFactory : IDesignTimeDbContextFactory<SqliteDatabaseContext>
    {
        public SqliteDatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDatabaseContext>();
            optionsBuilder.UseSqlite("Data Source=vault.db");

            return new SqliteDatabaseContext(optionsBuilder.Options);
        }
    }

    public class ContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer("server=localhost;database=vault");

            return new DatabaseContext(optionsBuilder.Options);
        }
    }

    public class SqliteDatabaseContext : DatabaseContext<SqliteDatabaseContext>
    {
        public SqliteDatabaseContext(DbContextOptions<SqliteDatabaseContext> options) : base(options) { }
    }

    public partial class DatabaseContext : DatabaseContext<DatabaseContext>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
    public class DatabaseContext<T> : DbContext
    where T : DbContext
    {
        public DatabaseContext(DbContextOptions<T> options)
            : base(options)
        { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=localhost;database=vault");
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Cipher> Ciphers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionCipher> CollectionCiphers { get; set; }
        public DbSet<CollectionUser> CollectionUsers { get; set; }
        public DbSet<TableModel.Device> Devices { get; set; }
        public DbSet<TableModel.Grant> Grants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            /*
            builder.Entity<Cipher>().Ignore(e => e.Data);
            builder.Entity<Cipher>().Property(e => e.DataJson).HasColumnName("Data");
            builder.Entity<Cipher>().Ignore(e => e.Attachments);
            builder.Entity<Cipher>().Property(e => e.AttachmentsJson).HasColumnName("Attachments");
            builder.Entity<Cipher>().Ignore(e => e.Favorites);
            builder.Entity<Cipher>().Property(e => e.FavoritesJson).HasColumnName("Favorites");
            builder.Entity<Cipher>().Ignore(e => e.Folders);
            builder.Entity<Cipher>().Property(e => e.FoldersJson).HasColumnName("Folders");
            builder.Entity<User>().Ignore(e => e.TwoFactorProviders);
            builder.Entity<User>().Property(e => e.TwoFactorProvidersJson).HasColumnName("TwoFactorProviders");
            builder.Entity<Organization>().Ignore(e => e.TwoFactorProviders);
            builder.Entity<Organization>().Property(e => e.TwoFactorProvidersJson).HasColumnName("TwoFactorProviders");
            */

            builder.Entity<User>().ToTable(nameof(User));
            builder.Entity<Cipher>(x =>
            {
                x.ToTable(nameof(Cipher));
            });
            builder.Entity<Organization>().ToTable(nameof(Organization));
            builder.Entity<OrganizationUser>().ToTable(nameof(OrganizationUser));
            builder.Entity<Collection>().ToTable(nameof(Collection));

            builder.Entity<CollectionCipher>(x =>
            {
                //x.HasNoKey();
                x.ToTable(nameof(CollectionCipher));
                x.HasOne(y => y.Cipher)
                    .WithMany()
                    .HasForeignKey(y => y.CipherId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired();                
            });
            builder.Entity<CollectionUser>(x =>
            {
                //x.HasNoKey();
                x.ToTable(nameof(CollectionUser));
                x.HasOne(y => y.OrganizationUser)
                    .WithMany()
                    .HasForeignKey(y => y.OrganizationUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired();
            });

            builder.Entity<TableModel.Device>().ToTable(nameof(TableModel.Device));
            builder.Entity<TableModel.Grant>(x => { x.HasKey(k => k.Key); x.ToTable(nameof(TableModel.Grant)); });
        }
    }
}
