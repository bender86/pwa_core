using PWA.Auth.Models;

namespace PWA.Auth.Services
{
    public class ApplicationConfigService : IApplicationConfigService
    {
        private readonly Dictionary<string, ApplicationInfo> _appConfigs;

        public ApplicationConfigService()
        {
            // Configuration centralis�e de toutes les applications
            _appConfigs = new Dictionary<string, ApplicationInfo>(StringComparer.OrdinalIgnoreCase)
            {
                ["MaesAuth"] = new ApplicationInfo
                {
                    Name = "MaesAuth",
                    DisplayName = "Administration IAM",
                    Description = "Manage users, roles, and permissions",
                    IconUrl = "/icons/maesauth.svg",
                    Route = "/admin",
                    Category = "Administration",
                    DisplayOrder = 1
                },
                ["WorldBet"] = new ApplicationInfo
                {
                    Name = "WorldBet",
                    DisplayName = "World Cup Betting",
                    Description = "Bet on matches with your friends",
                    IconUrl = "/icons/worldbet.svg",
                    Route = "https://maeshome.ddns.net/worldbet",
                    Category = "Entertainment",
                    DisplayOrder = 2
                }
                // Ajoute d'autres applications ici au fur et � mesure
            };
        }

        /// <summary>
        /// R�cup�re les infos d'une application par son nom
        /// </summary>
        public ApplicationInfo GetApplicationInfo(string appName)
        {
            if (_appConfigs.TryGetValue(appName, out var config))
            {
                return config;
            }

            // Fallback pour les apps non configur�es
            return new ApplicationInfo
            {
                Name = appName,
                DisplayName = appName,
                Description = "Application",
                IconUrl = "/icons/default-app.svg",
                Route = "/",
                DisplayOrder = 999
            };
        }

        /// <summary>
        /// R�cup�re les infos de plusieurs applications
        /// </summary>
        public List<ApplicationInfo> GetApplicationInfos(List<string> appNames)
        {
            return appNames
                .Select(GetApplicationInfo)
                .OrderBy(app => app.DisplayOrder)
                .ToList();
        }
    }
}