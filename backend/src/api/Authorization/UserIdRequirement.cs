using Microsoft.AspNetCore.Authorization;

namespace api.Authorization;

public class UserIdRequirement : IAuthorizationRequirement
{
    public static string PolicyName = "UserIdPolicy";
}

public class UserIdRequirementHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<UserIdRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private HttpContext _httpContext => _httpContextAccessor.HttpContext!;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIdRequirement requirement)
    {
        if (!_httpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            context.Fail();
        }
        var userId = _httpContext?.Request.UserId();
        if (userId == null)
        {
            context.Fail();
        }
        var valid = Guid.TryParse(userId, out Guid id);
        if (!valid)
        {
            context.Fail();
        }
        _httpContext!.Items["UserId"] = id;
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}