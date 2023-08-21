using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Utils.HelthChecks;

public class DbContextHealthCheck<TContext> : IHealthCheck where TContext : DbContext
{
    public DbContextHealthCheck(TContext dbContext) 
    {
        _dbContext = dbContext;
    }


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("DbContext connection test failed", ex);
        }
    }


    private readonly TContext _dbContext;
}
