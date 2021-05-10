using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using EFRepos = Bit.Infrastructure.EntityFramework;
using NoopRepos = Bit.Core.Repositories.Noop;
using Bit.Core.Repositories;
using Bit.Infrastructure.EntityFramework.Migration;


namespace Bit.Core
{
    public static class EntityFramewoakPersistanceExtension
    {
        public static void AddSqlServerRepositories(this IServiceCollection services, GlobalSettings globalSettings)
        {
            services.AddAutoMapper(typeof(Entities.Cipher));
            if (globalSettings.Sqlite.HasConnection)
                services.MigrateSqlite(globalSettings.Sqlite.ConnectionString);
            else if (globalSettings.SqlServer.HasConnection)            
                services.AddDbContext<EFRepos.DatabaseContext>(options => options.UseSqlServer(globalSettings.SqlServer.ConnectionString));

            services.AddScoped<IUserRepository, EFRepos.UserRepository>();
            services.AddScoped<ICipherRepository, EFRepos.CipherRepository>();
            services.AddScoped<IOrganizationCipherRepository, EFRepos.OrganizationCipherRepository>();
            services.AddScoped<IOrganizationRepository, EFRepos.OrganizationRepository>();
            services.AddScoped<IOrganizationUserRepository, EFRepos.OrganizationUserRepository>();
            services.AddScoped<ICollectionRepository, EFRepos.CollectionRepository>();
            services.AddScoped<ICollectionCipherRepository, EFRepos.CollectionCipherRepository>();

            services.AddScoped<IDeviceRepository, EFRepos.DeviceRepository>();
            services.AddScoped<IGrantRepository, EFRepos.GrantRepository>();

            //Noop Repositores
            services.AddSingleton<IPolicyRepository, NoopRepos.PolicyRepository>();
            services.AddSingleton<IInstallationDeviceRepository, NoopRepos.InstallationDeviceRepository>();
            services.AddSingleton<IMetaDataRepository, NoopRepos.MetaDataRepository>();
            services.AddSingleton<IFolderRepository, NoopRepos.FolderRepository>();
            services.AddSingleton<IU2fRepository, NoopRepos.U2fRepository>();
            services.AddSingleton<IInstallationRepository, NoopRepos.InstallationRepository>();
            services.AddSingleton<ISsoConfigRepository, NoopRepos.SsoConfigRepository>();
            services.AddSingleton<ISsoUserRepository, NoopRepos.SsoUserRepository>();
            services.AddSingleton<ITaxRateRepository, NoopRepos.TaxRateRepository>();

#if false                        
            services.AddSingleton<IMaintenanceRepository, SqlServerRepos.MaintenanceRepository>();
            services.AddSingleton<ISendRepository, NoopRepos.SendRepository>();
            services.AddSingleton<ITransactionRepository, SqlServerRepos.TransactionRepository>();                                    
            services.AddSingleton<ISendRepository, SqlServerRepos.SendRepository>();
            services.AddSingleton<ITaxRateRepository, SqlServerRepos.TaxRateRepository>();
            services.AddSingleton<IEmergencyAccessRepository, SqlServerRepos.EmergencyAccessRepository>();
            services.AddSingleton<IEventRepository, SqlServerRepos.EventRepository>();
#endif            
        }
       
    }
}
