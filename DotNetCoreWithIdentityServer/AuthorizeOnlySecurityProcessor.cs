using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace DotNetCoreWithIdentityServer
{
    public class AuthorizeOnlySecurityProcessor : IOperationProcessor
    {
        private readonly string _schemeName;

        public AuthorizeOnlySecurityProcessor(string schemeName)
        {
            _schemeName = schemeName;
        }

        public bool Process(OperationProcessorContext context)
        {
            // Check if endpoint has [Authorize]
            var hasAuthorize = context.ControllerType?.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any() == true
                || context.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any();

            // Check if [AllowAnonymous] exists
            var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any()
                || context.ControllerType?.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() == true;

            if (hasAuthorize && !hasAllowAnonymous)
            {
                // Initialize Security if null
                if (context.OperationDescription.Operation.Security == null)
                {
                    context.OperationDescription.Operation.Security = new System.Collections.Generic.List<NSwag.OpenApiSecurityRequirement>();
                }

                // Add security requirement
                context.OperationDescription.Operation.Security.Add(new NSwag.OpenApiSecurityRequirement
            {
                { _schemeName, new string[] { } }
            });
            }

            return true;
        }
    }
}
