using Microsoft.Extensions.Configuration;

namespace Core.Utils;

public static class DatabaseHelper
{
    public static string BuildConnectionString(IConfiguration configuration, string userId, string password)
        => $"Server={configuration["DatabaseSettings:Host"]};Port={configuration["DatabaseSettings:Port"]};" +
        $"User Id={userId};Password={password};Database=punklink2;Pooling=true;";
}
