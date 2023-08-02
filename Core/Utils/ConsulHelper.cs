using Microsoft.Extensions.Configuration;

namespace Core.Utils;

public static class ConsulHelper
{
    public static string BuildServiceName(IConfiguration configuration)
        => $"{configuration["ASPNETCORE_ENVIRONMENT"]}/{configuration["ServiceName"]}".ToLower();
}
