using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MobileShopBilling.Domain.Interfaces;
using MobileShopBilling.Infrastructure.Data;

namespace MobileShopBilling.Infrastructure.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, ApplicationDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("TenantId");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
            {
                var tenant = await dbContext.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

                if (tenant != null)
                {
                    tenantProvider.SetTenantId(tenantId);
                }
            }
        }

        await _next(context);
    }
}
