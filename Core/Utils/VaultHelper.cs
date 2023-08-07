using Vault.Client;
using Vault;
using Microsoft.Extensions.Configuration;

namespace Core.Utils;

public static class VaultHelper
{
    public static dynamic GetSecrets(IConfiguration configuration)
    {
        var vaultClient = GetVaultClient(configuration);
        var response = vaultClient.Secrets.KVv2Read(configuration["ServiceName"], StorageName);

        dynamic data = response.Data;
        return data["data"]!;
    }


    private static VaultClient GetVaultClient(IConfiguration configuration)
    {
        var vaultAddress = configuration["PNKL_VAULT_ADDR"]!;
        var vaultConfig = new VaultConfiguration(vaultAddress);

        var vaultClient = new VaultClient(vaultConfig);
        vaultClient.SetToken(configuration["PNKL_VAULT_TOKEN"]);

        return vaultClient;
    }
     

    private const string StorageName = "secrets";
}
