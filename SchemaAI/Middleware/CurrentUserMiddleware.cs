using SchemaAI.Entities;
using SchemaAI.Persistence;
using System.Security.Claims;

namespace SchemaAI.Middleware
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // ✅ HttpContext MUST be first
        public async Task InvokeAsync(
            HttpContext context,
            ICurrentUser currentUser)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                currentUser.IsAuthenticated = true;

                currentUser.UserGuid = Guid.TryParse(
                    context.User.FindFirst("UserGuid")?.Value,
                    out var userGuid)
                    ? userGuid
                    : Guid.Empty;

                currentUser.UserName =
                    context.User.FindFirst("Name")?.Value ?? string.Empty;

                currentUser.TenantGuid = Guid.TryParse(
                    context.User.FindFirst("TenantGuid")?.Value,
                    out var tenantGuid)
                    ? tenantGuid
                    : Guid.Empty;
            }

            await _next(context);
        }
    }


    //public class CurrentUserMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public CurrentUserMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    //public async Task InvokeAsync(HttpContext context)
    //    //{
    //    //    if (context.User.Identity?.IsAuthenticated ?? false)
    //    //    {
    //    //        var claims = context.User.Claims;

    //    //        // Map claims to your User class
    //    //        var currentUser = new User
    //    //        {
    //    //            UserGuid = Guid.Parse(claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value ?? "0"),
    //    //            UserName = claims.FirstOrDefault(c => c.Type == "Name")?.Value ?? string.Empty,
    //    //            Phone = (claims.FirstOrDefault(c => c.Type == "Phone")?.Value ?? " "),
    //    //            TenantGuid = Guid.Parse(claims.FirstOrDefault(c => c.Type == "TenantGuid")?.Value ?? "0")
    //    //        };

    //    //        // Store in HttpContext.Items so it can be accessed anywhere
    //    //        context.Items["CurrentUser"] = currentUser;
    //    //    }

    //    //    await _next(context);
    //    //}

    //    public async Task InvokeAsync(
    //        ICurrentUser currentUser,
    //        HttpContext context,
    //        SchemaAIDbContext dbContext)
    //    {
    //        var path = context.Request.Path.Value?.ToLower();

    //        if (path!.StartsWith("/openapi") ||
    //            path.StartsWith("/health") ||
    //            path.StartsWith("/auth"))
    //        {
    //            await _next(context);
    //            return;
    //        }

    //        if (context.User.Identity?.IsAuthenticated ?? false)
    //        {
    //            var claims = context.User.Claims;

    //            // Map claims to your User class
    //            currentUser.IsAuthenticated = true;

    //            currentUser.UserGuid = GetGuidClaim(context.User, "UserGuid");
    //            currentUser.UserName =
    //                context.User.FindFirst("Name")?.Value ?? string.Empty;
    //            currentUser.Phone =
    //                context.User.FindFirst("Phone")?.Value ?? string.Empty;
    //            currentUser.TenantGuid = GetGuidClaim(context.User, "TenantGuid");


    //            // Store in HttpContext.Items so it can be accessed anywhere
    //            context.Items["CurrentUser"] = currentUser;

    //        }
    //        await _next(context);
    //    }

    //    private static Guid GetGuidClaim(ClaimsPrincipal user, string claimType)
    //    {
    //        var value = user.FindFirst(claimType)?.Value;
    //        return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
    //    }
    //}
}
