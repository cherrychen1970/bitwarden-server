using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Bit.Core.Services;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Serilog.Context;


namespace Bit.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ServiceCollectionExtensions));
            services.AddScoped<ICipherService, CipherService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<ICollectionService, CollectionService>();
            //services.AddScoped<IGroupService, GroupService>();
            //services.AddScoped<IPolicyService, PolicyService>();
            
            //services.AddScoped<IEmergencyAccessService, EmergencyAccessService>();

            services.AddScoped<IDeviceService, DeviceService>();
            //services.AddSingleton<IAppleIapService, AppleIapService>();
            //services.AddSingleton<ISsoConfigService, SsoConfigService>();
            //services.AddScoped<ISendService, SendService>();
            services.AddScoped<IApplicationCacheService, InMemoryApplicationCacheService>();

            // Noop Services
            services.AddScoped<IReferenceEventService, NoopReferenceEventService>();
            services.AddScoped<ISendFileStorageService, NoopSendFileStorageService>();
            services.AddScoped<IPushRegistrationService, NoopPushRegistrationService>();
            services.AddScoped<IEventWriteService, NoopEventWriteService>();            
            services.AddScoped<IMailService, NoopMailService>();
            services.AddScoped<IEventService, NoopEventService>();
            services.AddScoped<IMailDeliveryService, NoopMailDeliveryService>();
            services.AddScoped<IPushNotificationService, NoopPushNotificationService>();
            services.AddScoped<IBlockIpService, NoopBlockIpService>();
            services.AddScoped<IPushRegistrationService, NoopPushRegistrationService>();
            services.AddScoped<IAttachmentStorageService, NoopAttachmentStorageService>();            
            services.AddScoped<IEventWriteService, NoopEventWriteService>();
            services.AddScoped<IEmergencyAccessService, NoopEmergencyAccessService>();
            services.AddScoped<IPolicyService, NoopPolicyService>();            
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
    }
}