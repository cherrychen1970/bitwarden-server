﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Bit.Core.Enums;
using Bit.Core.Identity;
using Bit.Core.IdentityServer;
using Bit.Core.Models.Table;
using Bit.Core.Repositories;
using Bit.Core.Resources;
using Bit.Core.Services;
using Bit.Core.Utilities;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Azure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog.Context;
using EntityFrameworkRepos = Bit.Core.Repositories.EntityFramework;
using Bit.Core.Repositories.EntityFramework.Migration;
using NoopRepos = Bit.Core.Repositories.Noop;
using SqlServerRepos = Bit.Core.Repositories.SqlServer;
using TableStorageRepos = Bit.Core.Repositories.TableStorage;

namespace Bit.Core.Utilities
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEFSqlServerRepositories(this IServiceCollection services, GlobalSettings globalSettings)
        {
            
            services.AddAutoMapper(typeof(EntityFrameworkRepos.UserRepository));
            services.MigrateSql(globalSettings.SqlServer.ConnectionString);
            //services.AddDbContext<EntityFrameworkRepos.DatabaseContext>(options => options.UseSqlServer(globalSettings.SqlServer.ConnectionString));

            services.AddScoped<IUserRepository, EntityFrameworkRepos.UserRepository>();
            services.AddScoped<ICipherRepository, EntityFrameworkRepos.CipherRepository>();
            services.AddScoped<IOrganizationRepository, EntityFrameworkRepos.OrganizationRepository>();
            services.AddScoped<IOrganizationUserRepository, EntityFrameworkRepos.OrganizationUserRepository>();
            services.AddScoped<ICollectionRepository, EntityFrameworkRepos.CollectionRepository>();
            services.AddScoped<ICollectionCipherRepository, EntityFrameworkRepos.CollectionCipherRepository>();
            
            services.AddScoped<IDeviceRepository, EntityFrameworkRepos.DeviceRepository>();
            services.AddScoped<IGrantRepository, EntityFrameworkRepos.GrantRepository>();
            
            //Noop Repositores
            services.AddSingleton<IPolicyRepository, NoopRepos.PolicyRepository>();
            services.AddSingleton<IInstallationDeviceRepository, NoopRepos.InstallationDeviceRepository>();
            services.AddSingleton<IMetaDataRepository, NoopRepos.MetaDataRepository>();
            services.AddSingleton<IFolderRepository, NoopRepos.FolderRepository>();
            services.AddSingleton<IU2fRepository, NoopRepos.U2fRepository>();
            services.AddSingleton<IGroupRepository, NoopRepos.GroupRepository>();
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
        public static void AddSqlServerRepositories(this IServiceCollection services, GlobalSettings globalSettings)
        {
            var usePostgreSql = CoreHelpers.SettingHasValue(globalSettings.PostgreSql?.ConnectionString);
            var useEf = usePostgreSql;

            if (useEf)
            {
                services.AddAutoMapper(typeof(EntityFrameworkRepos.UserRepository));
                services.AddDbContext<EntityFrameworkRepos.DatabaseContext>(options =>
                {
                    if (usePostgreSql)
                    {
                        options.UseNpgsql(globalSettings.PostgreSql.ConnectionString);
                    }
                });
                services.AddSingleton<IUserRepository, EntityFrameworkRepos.UserRepository>();
                services.AddSingleton<ICipherRepository, EntityFrameworkRepos.CipherRepository>();
                services.AddSingleton<IOrganizationRepository, EntityFrameworkRepos.OrganizationRepository>();
            }
            else
            {
                services.AddSingleton<IUserRepository, SqlServerRepos.UserRepository>();
                services.AddSingleton<ICipherRepository, SqlServerRepos.CipherRepository>();
                services.AddSingleton<IDeviceRepository, SqlServerRepos.DeviceRepository>();
                services.AddSingleton<IGrantRepository, SqlServerRepos.GrantRepository>();
                services.AddSingleton<IOrganizationRepository, SqlServerRepos.OrganizationRepository>();
                services.AddSingleton<IOrganizationUserRepository, SqlServerRepos.OrganizationUserRepository>();
                services.AddSingleton<ICollectionRepository, SqlServerRepos.CollectionRepository>();
                services.AddSingleton<IFolderRepository, SqlServerRepos.FolderRepository>();
                services.AddSingleton<ICollectionCipherRepository, SqlServerRepos.CollectionCipherRepository>();
                services.AddSingleton<IGroupRepository, SqlServerRepos.GroupRepository>();
                services.AddSingleton<IU2fRepository, SqlServerRepos.U2fRepository>();
                services.AddSingleton<IInstallationRepository, SqlServerRepos.InstallationRepository>();
                services.AddSingleton<IMaintenanceRepository, SqlServerRepos.MaintenanceRepository>();
                services.AddSingleton<ITransactionRepository, SqlServerRepos.TransactionRepository>();
                services.AddSingleton<IPolicyRepository, SqlServerRepos.PolicyRepository>();
                services.AddSingleton<ISsoConfigRepository, SqlServerRepos.SsoConfigRepository>();
                services.AddSingleton<ISsoUserRepository, SqlServerRepos.SsoUserRepository>();
                services.AddSingleton<ISendRepository, SqlServerRepos.SendRepository>();
                services.AddSingleton<ITaxRateRepository, SqlServerRepos.TaxRateRepository>();
                services.AddSingleton<IEmergencyAccessRepository, SqlServerRepos.EmergencyAccessRepository>();
            }

            if (globalSettings.SelfHosted)
            {
                if (useEf)
                {
                    // TODO
                }
                else
                {
                    services.AddSingleton<IEventRepository, SqlServerRepos.EventRepository>();
                }
                services.AddSingleton<IInstallationDeviceRepository, NoopRepos.InstallationDeviceRepository>();
                services.AddSingleton<IMetaDataRepository, NoopRepos.MetaDataRepository>();
            }
            else
            {
                services.AddSingleton<IEventRepository, TableStorageRepos.EventRepository>();
                services.AddSingleton<IInstallationDeviceRepository, TableStorageRepos.InstallationDeviceRepository>();
                services.AddSingleton<IMetaDataRepository, TableStorageRepos.MetaDataRepository>();
            }
        }

        public static void AddBaseServices(this IServiceCollection services)
        {
            services.AddScoped<ICipherService, CipherService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<ICollectionService, CollectionService>();
            //services.AddScoped<IGroupService, GroupService>();
            //services.AddScoped<IPolicyService, PolicyService>();
            services.AddScoped<IEventService, EventService>();
            //services.AddScoped<IEmergencyAccessService, EmergencyAccessService>();
            
            services.AddScoped<IDeviceService, DeviceService>();
            //services.AddSingleton<IAppleIapService, AppleIapService>();
            //services.AddSingleton<ISsoConfigService, SsoConfigService>();
            //services.AddScoped<ISendService, SendService>();
            services.AddScoped<IApplicationCacheService, InMemoryApplicationCacheService>();            
        }

        public static void AddNoopServices(this IServiceCollection services)
        {
            services.AddScoped<IReferenceEventService, NoopReferenceEventService>();
            services.AddScoped<ISendFileStorageService, NoopSendFileStorageService>();
            services.AddScoped<IPushRegistrationService, NoopPushRegistrationService>();
            services.AddScoped<IEventWriteService, NoopEventWriteService>();
            services.AddScoped<IPaymentService, NoopPaymentService>();            
            services.AddScoped<IMailService, NoopMailService>();
            services.AddScoped<IMailDeliveryService, NoopMailDeliveryService>();
            services.AddScoped<IPushNotificationService, NoopPushNotificationService>();
            services.AddScoped<IBlockIpService, NoopBlockIpService>();
            services.AddScoped<IPushRegistrationService, NoopPushRegistrationService>();
            services.AddScoped<IAttachmentStorageService, NoopAttachmentStorageService>();
            services.AddScoped<ILicensingService, NoopLicensingService>();
            services.AddScoped<IEventWriteService, NoopEventWriteService>();

            services.AddScoped<IGroupService, NoopGroupService>();
            services.AddScoped<IEmergencyAccessService, NoopEmergencyAccessService>();
            services.AddScoped<IPolicyService, NoopPolicyService>();


        }

        public static void AddDefaultServices(this IServiceCollection services, GlobalSettings globalSettings)
        {
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddScoped<IEmergencyAccessService, EmergencyAccessService>();

            services.AddSingleton<IAppleIapService, AppleIapService>();
            services.AddSingleton<ISsoConfigService, SsoConfigService>();
            services.AddScoped<ISendService, SendService>();

            services.AddSingleton<IPaymentService, StripePaymentService>();
            services.AddSingleton<IMailService, HandlebarsMailService>();
            services.AddSingleton<ILicensingService, NoopLicensingService>();

            if (CoreHelpers.SettingHasValue(globalSettings.ServiceBus.ConnectionString) &&
                CoreHelpers.SettingHasValue(globalSettings.ServiceBus.ApplicationCacheTopicName))
            {
                services.AddSingleton<IApplicationCacheService, InMemoryServiceBusApplicationCacheService>();
            }
            else
            {
                services.AddSingleton<IApplicationCacheService, InMemoryApplicationCacheService>();
            }

            if (CoreHelpers.SettingHasValue(globalSettings.Amazon?.AccessKeySecret))
            {
                services.AddSingleton<IMailDeliveryService, AmazonSesMailDeliveryService>();
            }
            else if (CoreHelpers.SettingHasValue(globalSettings.Mail?.Smtp?.Host))
            {
                services.AddSingleton<IMailDeliveryService, MailKitSmtpMailDeliveryService>();
            }
            else
            {
                services.AddSingleton<IMailDeliveryService, NoopMailDeliveryService>();
            }

            services.AddSingleton<IPushNotificationService, MultiServicePushNotificationService>();
            if (globalSettings.SelfHosted &&
                CoreHelpers.SettingHasValue(globalSettings.PushRelayBaseUri) &&
                globalSettings.Installation?.Id != null &&
                CoreHelpers.SettingHasValue(globalSettings.Installation?.Key))
            {
                services.AddSingleton<IPushRegistrationService, RelayPushRegistrationService>();
            }
            else if (!globalSettings.SelfHosted)
            {
                services.AddSingleton<IPushRegistrationService, NotificationHubPushRegistrationService>();
            }
            else
            {
                services.AddSingleton<IPushRegistrationService, NoopPushRegistrationService>();
            }

            if (!globalSettings.SelfHosted && CoreHelpers.SettingHasValue(globalSettings.Storage?.ConnectionString))
            {
                services.AddSingleton<IBlockIpService, AzureQueueBlockIpService>();
            }
            else if (!globalSettings.SelfHosted && CoreHelpers.SettingHasValue(globalSettings.Amazon?.AccessKeySecret))
            {
                services.AddSingleton<IBlockIpService, AmazonSqsBlockIpService>();
            }
            else
            {
                services.AddSingleton<IBlockIpService, NoopBlockIpService>();
            }

            if (!globalSettings.SelfHosted && CoreHelpers.SettingHasValue(globalSettings.Events.ConnectionString))
            {
                services.AddSingleton<IEventWriteService, AzureQueueEventWriteService>();
            }
            else if (globalSettings.SelfHosted)
            {
                services.AddSingleton<IEventWriteService, RepositoryEventWriteService>();
            }
            else
            {
                services.AddSingleton<IEventWriteService, NoopEventWriteService>();
            }

            if (CoreHelpers.SettingHasValue(globalSettings.Attachment.ConnectionString))
            {
                services.AddSingleton<IAttachmentStorageService, AzureAttachmentStorageService>();
            }
            else if (CoreHelpers.SettingHasValue(globalSettings.Attachment.BaseDirectory))
            {
                services.AddSingleton<IAttachmentStorageService, LocalAttachmentStorageService>();
            }
            else
            {
                services.AddSingleton<IAttachmentStorageService, NoopAttachmentStorageService>();
            }

            if (CoreHelpers.SettingHasValue(globalSettings.Send.ConnectionString))
            {
                services.AddSingleton<ISendFileStorageService, AzureSendFileStorageService>();
            }
            else if (CoreHelpers.SettingHasValue(globalSettings.Send.BaseDirectory))
            {
                services.AddSingleton<ISendFileStorageService, LocalSendStorageService>();
            }
            else
            {
                services.AddSingleton<ISendFileStorageService, NoopSendFileStorageService>();
            }

            if (globalSettings.SelfHosted)
            {
                services.AddSingleton<IReferenceEventService, NoopReferenceEventService>();
            }
            else
            {
                services.AddSingleton<IReferenceEventService, AzureQueueReferenceEventService>();
            }
        }


        public static IdentityBuilder AddCustomIdentityServices(
            this IServiceCollection services, GlobalSettings globalSettings)
        {
            services.AddSingleton<IOrganizationDuoWebTokenProvider, OrganizationDuoWebTokenProvider>();
            services.Configure<PasswordHasherOptions>(options => options.IterationCount = 100000);
            services.Configure<TwoFactorRememberTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(30);
            });

            var identityBuilder = services.AddIdentityWithoutCookieAuth<User, Role>(options =>
            {
                options.User = new UserOptions
                {
                    RequireUniqueEmail = true,
                    AllowedUserNameCharacters = null // all
                };
                options.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequiredLength = 8,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
                options.ClaimsIdentity = new ClaimsIdentityOptions
                {
                    SecurityStampClaimType = "sstamp",
                    UserNameClaimType = JwtClaimTypes.Email,
                    UserIdClaimType = JwtClaimTypes.Subject
                };
                options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
            });

            identityBuilder
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
                .AddTokenProvider<AuthenticatorTokenProvider>(
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.Authenticator))
                .AddTokenProvider<EmailTokenProvider>(
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.Email))
                .AddTokenProvider<YubicoOtpTokenProvider>(
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.YubiKey))
                .AddTokenProvider<DuoWebTokenProvider>(
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.Duo))
                .AddTokenProvider<U2fTokenProvider>(
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.U2f))
                .AddTokenProvider<TwoFactorRememberTokenProvider>(
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.Remember))
                .AddTokenProvider<EmailTokenProvider<User>>(TokenOptions.DefaultEmailProvider);

            return identityBuilder;
        }

        public static Tuple<IdentityBuilder, IdentityBuilder> AddPasswordlessIdentityServices<TUserStore>(
            this IServiceCollection services, GlobalSettings globalSettings) where TUserStore : class
        {
            services.TryAddTransient<ILookupNormalizer, LowerInvariantLookupNormalizer>();
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            var passwordlessIdentityBuilder = services.AddIdentity<IdentityUser, Role>()
                .AddUserStore<TUserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

            var regularIdentityBuilder = services.AddIdentityCore<User>()
                .AddUserStore<UserStore>();

            services.TryAddScoped<PasswordlessSignInManager<IdentityUser>, PasswordlessSignInManager<IdentityUser>>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/";
                options.AccessDeniedPath = "/login?accessDenied=true";
                options.Cookie.Name = $"Bitwarden_{globalSettings.ProjectName}";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(2);
                options.ReturnUrlParameter = "returnUrl";
                options.SlidingExpiration = true;
            });

            return new Tuple<IdentityBuilder, IdentityBuilder>(passwordlessIdentityBuilder, regularIdentityBuilder);
        }

        public static void AddIdentityAuthenticationServices(
            this IServiceCollection services, GlobalSettings globalSettings, IWebHostEnvironment environment,
            Action<AuthorizationOptions> addAuthorization=null)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = globalSettings.BaseServiceUri.InternalIdentity;
                    options.RequireHttpsMetadata = !environment.IsDevelopment() &&
                        globalSettings.BaseServiceUri.InternalIdentity.StartsWith("https");
                    options.TokenRetriever = TokenRetrieval.FromAuthorizationHeaderOrQueryString();
                    options.NameClaimType = ClaimTypes.Email;
                    options.SupportedTokens = SupportedTokens.Jwt;
                });

            if (addAuthorization != null)
            {
                services.AddAuthorization(config =>
                {
                    addAuthorization.Invoke(config);
                });
            }

            if (environment.IsDevelopment())
            {
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            }
        }

        public static void AddCustomDataProtectionServices(
            this IServiceCollection services, IWebHostEnvironment env, GlobalSettings globalSettings)
        {
            var builder = services.AddDataProtection().SetApplicationName("Bitwarden");
            if (env.IsDevelopment())
            {
                return;
            }

            if (globalSettings.SelfHosted && CoreHelpers.SettingHasValue(globalSettings.DataProtection.Directory))
            {
                builder.PersistKeysToFileSystem(new DirectoryInfo(globalSettings.DataProtection.Directory));
            }

            if (!globalSettings.SelfHosted && CoreHelpers.SettingHasValue(globalSettings.Storage?.ConnectionString))
            {
                var storageAccount = CloudStorageAccount.Parse(globalSettings.Storage.ConnectionString);
                X509Certificate2 dataProtectionCert = null;
                if (CoreHelpers.SettingHasValue(globalSettings.DataProtection.CertificateThumbprint))
                {
                    dataProtectionCert = CoreHelpers.GetCertificate(
                        globalSettings.DataProtection.CertificateThumbprint);
                }
                else if (CoreHelpers.SettingHasValue(globalSettings.DataProtection.CertificatePassword))
                {
                    dataProtectionCert = CoreHelpers.GetBlobCertificateAsync(storageAccount, "certificates",
                        "dataprotection.pfx", globalSettings.DataProtection.CertificatePassword)
                        .GetAwaiter().GetResult();
                }
                builder
                    .PersistKeysToAzureBlobStorage(storageAccount, "aspnet-dataprotection/keys.xml")
                    .ProtectKeysWithCertificate(dataProtectionCert);
            }
        }

        public static IIdentityServerBuilder AddIdentityServerCertificate(
            this IIdentityServerBuilder identityServerBuilder, IWebHostEnvironment env, GlobalSettings globalSettings)
        {
            var certificate = CoreHelpers.GetIdentityServerCertificate(globalSettings);
            if (certificate != null)
            {
                identityServerBuilder.AddSigningCredential(certificate);
            }
            else if (env.IsDevelopment())
            {
                identityServerBuilder.AddDeveloperSigningCredential(false);
            }
            else
            {
                throw new Exception("No identity certificate to use.");
            }
            return identityServerBuilder;
        }

        public static GlobalSettings AddGlobalSettingsServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            var globalSettings = new GlobalSettings();
            ConfigurationBinder.Bind(configuration.GetSection("GlobalSettings"), globalSettings);
            services.AddSingleton(s => globalSettings);
            return globalSettings;
        }

        public static void UseDefaultMiddleware(this IApplicationBuilder app,
            IWebHostEnvironment env, GlobalSettings globalSettings)
        {
            string GetHeaderValue(HttpContext httpContext, string header)
            {
                if (httpContext.Request.Headers.ContainsKey(header))
                {
                    return httpContext.Request.Headers[header];
                }
                return null;
            }

            // Add version information to response headers
            app.Use(async (httpContext, next) =>
            {
                using (LogContext.PushProperty("IPAddress", httpContext.GetIpAddress(globalSettings)))
                using (LogContext.PushProperty("UserAgent", GetHeaderValue(httpContext, "user-agent")))
                using (LogContext.PushProperty("DeviceType", GetHeaderValue(httpContext, "device-type")))
                using (LogContext.PushProperty("Origin", GetHeaderValue(httpContext, "origin")))
                {
                    httpContext.Response.OnStarting((state) =>
                    {
                        httpContext.Response.Headers.Append("Server-Version", CoreHelpers.GetVersion());
                        return Task.FromResult(0);
                    }, null);
                    await next.Invoke();
                }
            });
        }

        public static void UseForwardedHeaders(this IApplicationBuilder app, GlobalSettings globalSettings)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            if (!string.IsNullOrWhiteSpace(globalSettings.KnownProxies))
            {
                var proxies = globalSettings.KnownProxies.Split(',');
                foreach (var proxy in proxies)
                {
                    if (System.Net.IPAddress.TryParse(proxy.Trim(), out var ip))
                    {
                        options.KnownProxies.Add(ip);
                    }
                }
            }
            if (options.KnownProxies.Count > 1)
            {
                options.ForwardLimit = null;
            }
            app.UseForwardedHeaders(options);
        }

        public static void AddCoreLocalizationServices(this IServiceCollection services)
        {
            services.AddTransient<II18nService, I18nService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        public static IApplicationBuilder UseCoreLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[] { "en" };
            return app.UseRequestLocalization(options => options
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures));
        }

        public static IMvcBuilder AddViewAndDataAnnotationLocalization(this IMvcBuilder mvc)
        {
            mvc.Services.AddTransient<IViewLocalizer, I18nViewLocalizer>();
            return mvc.AddViewLocalization(options => options.ResourcesPath = "Resources")
                .AddDataAnnotationsLocalization(options =>
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResources).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResources", assemblyName.Name);
                    });
        }

        public static IServiceCollection AddDistributedIdentityServices(this IServiceCollection services, GlobalSettings globalSettings)
        {
            if (string.IsNullOrWhiteSpace(globalSettings.IdentityServer?.RedisConnectionString))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddDistributedRedisCache(options =>
                    options.Configuration = globalSettings.IdentityServer.RedisConnectionString);
            }

            services.AddOidcStateDataFormatterCache();
            services.AddSession();
            services.ConfigureApplicationCookie(configure => configure.CookieManager = new DistributedCacheCookieManager());
            services.ConfigureExternalCookie(configure => configure.CookieManager = new DistributedCacheCookieManager());
            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>>(
                svcs => new ConfigureOpenIdConnectDistributedOptions(
                    svcs.GetRequiredService<IHttpContextAccessor>(),
                    globalSettings,
                    svcs.GetRequiredService<IdentityServerOptions>())
            );

            return services;
        }
    }
}
