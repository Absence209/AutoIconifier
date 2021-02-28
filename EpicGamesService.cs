using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Gameloop.Vdf;



namespace TileIconifier2._0
{
    class EpicGamesService
    {
        public readonly static string EpicGamesLauncherManifest = @"C:\ProgramData\Epic\EpicGamesLauncher\Data\Manifests";

        public static List<EpicGamesLauncherVideoGame> GetInstalledVideoGames()
        {
            List<EpicGamesLauncherVideoGame> videoGames = new List<EpicGamesLauncherVideoGame>();
            foreach (var file in Directory.EnumerateFiles(EpicGamesLauncherManifest, "*.item", SearchOption.TopDirectoryOnly))
            {
                videoGames.Add(JsonConvert.DeserializeObject<EpicGamesLauncherVideoGame>(File.ReadAllText(file)));
            }
            return videoGames;
        } 
    }
    
    public class EpicGamesLauncherVideoGame
    {
        public int FormatVersion { get; set; }
        public bool bIsIncompleteInstall { get; set; }
        public string AppVersionString { get; set; }
        public string LaunchCommand { get; set; }
        public string LaunchExecutable { get; set; }
        public string ManifestLocation { get; set; }
        public bool bIsApplication { get; set; }
        public bool bIsExecutable { get; set; }
        public bool bIsManaged { get; set; }
        public bool bNeedsValidation { get; set; }
        public bool bRequiresAuth { get; set; }
        public bool bCanRunOffline { get; set; }
        public string AppName { get; set; }
        public List<string> BaseURLs { get; set; }
        public string BuildLabel { get; set; }
        public string CatalogItemId { get; set; }
        public string CatalogNamespace { get; set; }
        public List<string> AppCategories { get; set; }
        public List<object> ChunkDbs { get; set; }
        public List<object> CompatibleApps { get; set; }
        public string DisplayName { get; set; }
        public string FullAppName { get; set; }
        public string InstallationGuid { get; set; }
        public string InstallLocation { get; set; }
        public string InstallSessionId { get; set; }
        public List<object> InstallTags { get; set; }
        public List<object> InstallComponents { get; set; }
        public string HostInstallationGuid { get; set; }
        public List<object> PrereqIds { get; set; }
        public string StagingLocation { get; set; }
        public string TechnicalType { get; set; }
        public string VaultThumbnailUrl { get; set; }
        public string VaultTitleText { get; set; }
        public long InstallSize { get; set; }
        public string MainWindowProcessName { get; set; }
        public List<object> ProcessNames { get; set; }
        public string MainGameAppName { get; set; }
        public string MandatoryAppFolderName { get; set; }
        public string OwnershipToken { get; set; }

        // AttatchedProperties
        public string GetLaunchUri()
        {
            if (AppName == null) return null;
            return $"com.epicgames.launcher://apps/{AppName}?action=launch&silent=true";
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
