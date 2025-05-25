using Microsoft.AspNetCore.Authorization;

namespace FlashSale.Infrastructure.Identity
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var hasClaim = context.User?.Claims.Any(c =>
                c.Type == "permission" && c.Value == requirement.Permission) ?? false;

            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
