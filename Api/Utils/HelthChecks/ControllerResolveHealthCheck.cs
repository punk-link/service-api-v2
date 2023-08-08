using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Utils.HelthChecks;

public class ControllerResolveHealthCheck : IHealthCheck
{
    public ControllerResolveHealthCheck(IServiceProvider provider)
    {
        _provider = provider;
    }


    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        foreach (var controllerType in ControllerTypes)
            _provider.GetRequiredService(controllerType);

        return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
    }


    private static readonly Type[] ControllerTypes = typeof(ControllerResolveHealthCheck).Assembly
        .GetTypes()
        .Where(t => Attribute.GetCustomAttribute(t, typeof(ApiControllerAttribute)) is not null)
        .Where(t=> !t.IsAbstract && t.IsPublic)
        .ToArray();


    private readonly IServiceProvider  _provider;
}
