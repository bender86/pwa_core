using PWA.Auth.Models;

namespace PWA.Auth.Services
{
    /// <summary>
    /// Service de configuration pour le mapping des applications
    /// </summary>
    public interface IApplicationConfigService
    {
        ApplicationInfo GetApplicationInfo(string appName);
        List<ApplicationInfo> GetApplicationInfos(List<string> appNames);
    }
}