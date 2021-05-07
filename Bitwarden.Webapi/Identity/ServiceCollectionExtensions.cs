using System;
using Bit.Core;
using Bit.Core.Services;
using Bit.Core.IdentityServer;
using Bit.Core.Utilities;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Bit.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IIdentityServerBuilder AddCustomIdentityServerServices(this IServiceCollection services,
            IWebHostEnvironment env, GlobalSettings globalSettings)
        {
                        var rsa = new RsaKeyService(env, TimeSpan.FromDays(globalSettings.SingingKeyRefreshDays));
            services.AddTransient<RsaKeyService>(provider => rsa);

            SigningCredentials signingCredentials = new SigningCredentials(rsa.GetKey(), SecurityAlgorithms.RsaSha256);
            services.AddTransient<IDiscoveryResponseGenerator, DiscoveryResponseGenerator>();

            services.AddSingleton<StaticClientStore>();
            services.AddTransient<IAuthorizationCodeStore, AuthorizationCodeStore>();

            var issuerUri = new Uri(globalSettings.BaseServiceUri.InternalIdentity);
            var identityServerBuilder = services
                .AddIdentityServer(options =>
                {
                    options.Endpoints.EnableIntrospectionEndpoint = false;
                    options.Endpoints.EnableEndSessionEndpoint = false;
                    options.Endpoints.EnableUserInfoEndpoint = false;
                    options.Endpoints.EnableCheckSessionEndpoint = false;
                    options.Endpoints.EnableTokenRevocationEndpoint = false;
                    options.IssuerUri = $"{issuerUri.Scheme}://{issuerUri.Host}";
                    options.Caching.ClientStoreExpiration = new TimeSpan(0, 5, 0);
                    if (env.IsDevelopment())
                    {
                        options.Authentication.CookieSameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                    }
                })
                .AddSigningCredential(signingCredentials)
                .AddInMemoryCaching()
                .AddInMemoryApiResources(ApiResources.GetApiResources())
                .AddInMemoryApiScopes(ApiScopes.GetApiScopes())
                .AddClientStoreCache<ClientStore>()
                .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddPersistedGrantStore<PersistedGrantStore>()
                .AddClientStore<ClientStore>()
                .AddIdentityServerCertificate(env, globalSettings);

            services.AddTransient<ICorsPolicyService, CustomCorsPolicyService>();
            return identityServerBuilder;
        }
    }
}
