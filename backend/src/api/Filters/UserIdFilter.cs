using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UserIdFilter : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
        }
        var userId = context.HttpContext?.Request.UserId();
        if (userId == null)
        {
            context.Result = new UnauthorizedResult();
        }
        var valid = Guid.TryParse(userId, out Guid id);
        if (!valid)
        {
            context.Result = new UnauthorizedResult();
        }
        context.HttpContext!.Items["UserId"] = id;
    }
}
