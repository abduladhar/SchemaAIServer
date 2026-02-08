using SchemaAI.Persistence;

namespace SchemaAI
{
    public sealed class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
     HttpContext context,
     SchemaAIDbContext dbContext)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path!.StartsWith("/openapi") ||
                path.StartsWith("/health") ||
                path.StartsWith("/auth"))
            {
                await _next(context);
                return;
            }

            var tenantHeader = context.Request.Headers["X-Tenant-Guid"];

            //if(tenantHeader == string.Empty)
            //{
            //    tenantHeader = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            //}

            //if (!Guid.TryParse(tenantHeader, out Guid tenantGuid))
            //{
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    await context.Response.WriteAsync("Tenant header missing");
            //    return;
            //}

            //dbContext.CurrentTenantGuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            await _next(context);
        }
    }

}
