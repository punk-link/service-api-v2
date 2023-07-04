using Microsoft.Extensions.Configuration;

namespace Core.Utils;

public static class ConsulHelper
{
    public static string BuildServiceName(IConfiguration configuration, string serviceName)
        => $"{configuration["ASPNETCORE_ENVIRONMENT"]!.ToLower()}/{serviceName}";
}
