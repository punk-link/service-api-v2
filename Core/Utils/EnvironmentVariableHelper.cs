using Microsoft.Extensions.Hosting;

namespace Core.Utils;

public static class EnvironmentVariableHelper
{
    public static bool IsLocal(this IHostEnvironment hostingEnvironment) 
        => hostingEnvironment.IsEnvironment(LocalEnvironment);    
        

    private const string LocalEnvironment = "Local";
}
