using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Configuration;

using Bit.Core.Enums;
using Bit.Core.Identity;
using Bit.Core.IdentityServer;
using Bit.Core.Models;
using Bit.Core.Utilities;

namespace Bit.Core
{
    public static class IdentityServerExtension
    {
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
            Action<AuthorizationOptions> addAuthorization = null)
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
   
    }
}
