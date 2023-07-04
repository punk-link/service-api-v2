using Core.Utils.ConfigurationProviders;
using Microsoft.Extensions.Configuration;

namespace Core.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder builder, string address, string token, string storageName)
    {
        return builder.Add(new ConsulConfigurationSource(address, token, storageName));
    }
}
