﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bit.Api.Utilities;
using Bit.Core;
using Bit.Core.Identity;
using Newtonsoft.Json.Serialization;
using AspNetCoreRateLimit;
using Stripe;
using Bit.Core.Utilities;
using IdentityModel;
using System.Globalization;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Bit.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; private set; }
        public IWebHostEnvironment Environment { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Options
            services.AddOptions();

            // Settings
            var globalSettings = services.AddGlobalSettingsServices(Configuration);
            if (!globalSettings.SelfHosted)
            {
                services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimitOptions"));
                services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            }

            // Data Protection
            services.AddCustomDataProtectionServices(Environment, globalSettings);

            // Stripe Billing
            StripeConfiguration.ApiKey = globalSettings.StripeApiKey;

            // Repositories
            //services.AddSqlServerRepositories(globalSettings);
            services.AddEFSqlServerRepositories(globalSettings);

            // Context
            services.AddScoped<CurrentContext>();

            // Caching
            services.AddMemoryCache();

            // BitPay
            services.AddSingleton<BitPayClient>();

            if (!globalSettings.SelfHosted)
            {
                // Rate limiting
                services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            }

            services.AddCustomCookiePolicy();
            services.AddCustomSingleSignOn(globalSettings);
            // Identity
            services.AddCustomIdentityServices(globalSettings);
            services.AddIdentityAuthenticationServices(globalSettings, Environment);
            services.AddCustomAuthorizationPolicy();

            services.AddScoped<AuthenticatorTokenProvider>();

            // IdentityServer
            services.AddCustomIdentityServerServices(Environment, globalSettings);

            // Identity
            services.AddCustomIdentityServices(globalSettings);
            // Services
            services.AddBaseServices();

            //services.AddDefaultServices(globalSettings);
            services.AddNoopServices();

            services.AddCoreLocalizationServices();

            // MVC
            services.AddMvc(config =>
            {
                config.Conventions.Add(new ApiExplorerGroupConvention());
                config.Conventions.Add(new PublicApiControllersModelConvention());
            }).AddNewtonsoftJson(options =>
            {
                if (Environment.IsProduction() && Configuration["swaggerGen"] != "true")
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                }
            });

            services.AddSwagger(globalSettings);
            //Jobs.JobsHostedService.AddJobsServices(services);
            //services.AddHostedService<Jobs.JobsHostedService>();

            if (globalSettings.SelfHosted)
            {
                // Jobs service
                //Jobs.JobsHostedService.AddJobsServices(services);
                //services.AddHostedService<Jobs.JobsHostedService>();
            }
            if (CoreHelpers.SettingHasValue(globalSettings.ServiceBus.ConnectionString) &&
                CoreHelpers.SettingHasValue(globalSettings.ServiceBus.ApplicationCacheTopicName))
            {
                services.AddHostedService<Core.HostedServices.ApplicationCacheHostedService>();
            }
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            GlobalSettings globalSettings,
            ILogger<Startup> logger)
        {
            IdentityModelEventSource.ShowPII = true;
            app.UseSerilog(env, appLifetime, globalSettings);

            // Default Middleware
            app.UseDefaultMiddleware(env, globalSettings);

            if (!globalSettings.SelfHosted)
            {
                // Rate limiting
                app.UseMiddleware<CustomIpRateLimitMiddleware>();
            }
            else
            {
                app.UseForwardedHeaders(globalSettings);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCookiePolicy();
            }            

            // Add localization
            app.UseCoreLocalization();

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add routing
            app.UseRouting();

            // Add Cors
            app.UseCors(policy => policy.SetIsOriginAllowed(o => CoreHelpers.IsCorsOriginAllowed(o, globalSettings))
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            // Add authentication and authorization to the request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            // Add current context
            app.UseMiddleware<CurrentContextMiddleware>();

            // Add IdentityServer to the request pipeline.
            app.UseIdentityServer();

            // Add endpoints to the request pipeline.
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

            // Add Swagger
            if (Environment.IsDevelopment() || globalSettings.SelfHosted)
            {
                app.UseSwagger(config =>
                {
                    config.RouteTemplate = "specs/{documentName}/swagger.json";
                    config.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                        swaggerDoc.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer { Url = globalSettings.BaseServiceUri.Api }
                        });
                });
                app.UseSwaggerUI(config =>
                {
                    config.DocumentTitle = "Bitwarden API Documentation";
                    config.RoutePrefix = "docs";
                    config.SwaggerEndpoint($"{globalSettings.BaseServiceUri.Api}/specs/public/swagger.json",
                        "Bitwarden Public API");
                    config.OAuthClientId("accountType.id");
                    config.OAuthClientSecret("secretKey");
                });
            }
            // TODO : merge with identity
            // 33656 : identity, 51822 : sso
            /*
            app.Map("/signin-oidc",_app=>_app.RunProxy(new ProxyOptions(){Host="localhost",Port="33656", Scheme="http"}));
            app.Map("/authorize",_app=>_app.RunProxy(new ProxyOptions(){Host="localhost",Port="33656", Scheme="http"}));
            app.Map("/connect",_app=>_app.RunProxy(new ProxyOptions(){Host="localhost",Port="33656", Scheme="http"}));
            app.Map("/account",_app=>_app.RunProxy(new ProxyOptions(){Host="localhost",Port="33656", Scheme="http"}));
            app.Map("/.well-known",_app=>_app.RunProxy(new ProxyOptions(){Host="localhost",Port="33656", Scheme="http"}));
            */
            app.RunProxy(new ProxyOptions(){Host="localhost",Port="8080", Scheme="http"});

            // Log startup
            logger.LogInformation(Constants.BypassFiltersEventId, globalSettings.ProjectName + " started.");
        }
    }
}
