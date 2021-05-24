using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Bit.Core.Models;


namespace Bit.Api.Authorization
{
    public class CollectionAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, CollectionAccessProfile>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       CollectionAccessProfile resource)
        {
            var sessionContext = new Core.SessionContext(context.User);
            switch (requirement.Name)
            {
                case "Read":
                    break;
                case "Create":
                    break;
                case "Update":
                    if (sessionContext.HasOrganizationAdminAccess(resource.OrganizationId))
                    {
                        context.Succeed(requirement);
                    }

                    break;
                case "Delete":
                    break;

            }


            return Task.CompletedTask;
        }
    }
    public class CipherAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Core.Models.OrganizationCipher>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       Core.Models.OrganizationCipher resource)
        {
            var sessionContext = new Core.SessionContext(context.User);

            if (requirement.Name == Operations.Read.Name && sessionContext.HasOrganizationAdminAccess(resource.OrganizationId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    public class CrudRequirement : IAuthorizationRequirement
    {
        public string Operation { get; set; }
    }

    public static class Operations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = nameof(Create) };
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = nameof(Update) };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = nameof(Delete) };
    }

    public static class AuthorizationServiceExtensions
    {
        static public void AddCustomAuthorization(this ServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, CollectionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CipherAuthorizationHandler>();
        }
    }


    class Sample
    {
        async private void test()
        {
            IAuthorizationService _authorizationService = null;
            ClaimsPrincipal user = null;
            Core.Models.Collection collection = null;
            var authorizationResult = await _authorizationService
                      .AuthorizeAsync(user, collection, Operations.Read);
        }
    }

}