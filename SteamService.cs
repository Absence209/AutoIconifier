using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace TileIconifier2._0
{
    class SteamService
    {
        public static readonly string SteamRootLibraryPath = @"C:\Program Files (x86)\Steam\steamapps\libraryfolders.vdf";
        public static readonly string ChildFolder = @"steamapps";

        public static List<SteamAppState> GetSteamVideoGames()
        {
            if (!File.Exists(SteamRootLibraryPath)) return null;
            VProperty vdf = VdfConvert.Deserialize(File.ReadAllText(SteamRootLibraryPath));
            JToken jToken = VTokenExtensions.ToJson(vdf);
            string rawJson = JsonConvert.SerializeObject(jToken);
            //rawJson = rawJson.TrimStart("\\\"");
            //rawJson.
            rawJson = $"{{{rawJson}}}";
            SteamVDFRoot folders = JsonConvert.DeserializeObject<SteamVDFRoot>(rawJson);
            List<string> ValidLibraries = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._1)) ValidLibraries.Add(folders.LibraryFolders._1);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._2)) ValidLibraries.Add(folders.LibraryFolders._2);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._3)) ValidLibraries.Add(folders.LibraryFolders._3);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._4)) ValidLibraries.Add(folders.LibraryFolders._4);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._5)) ValidLibraries.Add(folders.LibraryFolders._5);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._6)) ValidLibraries.Add(folders.LibraryFolders._6);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._7)) ValidLibraries.Add(folders.LibraryFolders._7);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._8)) ValidLibraries.Add(folders.LibraryFolders._8);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._9)) ValidLibraries.Add(folders.LibraryFolders._9);
            if (!string.IsNullOrWhiteSpace(folders.LibraryFolders._10)) ValidLibraries.Add(folders.LibraryFolders._10);
            
            /*
            if (!string.IsNullOrWhiteSpace(folders._1)) ValidLibraries.Add(folders._1);
            if (!string.IsNullOrWhiteSpace(folders._2)) ValidLibraries.Add(folders._2);
            if (!string.IsNullOrWhiteSpace(folders._3)) ValidLibraries.Add(folders._3);
            if (!string.IsNullOrWhiteSpace(folders._4)) ValidLibraries.Add(folders._4);
            if (!string.IsNullOrWhiteSpace(folders._5)) ValidLibraries.Add(folders._5);
            if (!string.IsNullOrWhiteSpace(folders._6)) ValidLibraries.Add(folders._6);
            if (!string.IsNullOrWhiteSpace(folders._7)) ValidLibraries.Add(folders._7);
            if (!string.IsNullOrWhiteSpace(folders._8)) ValidLibraries.Add(folders._8);
            if (!string.IsNullOrWhiteSpace(folders._9)) ValidLibraries.Add(folders._9);
            if (!string.IsNullOrWhiteSpace(folders._10)) ValidLibraries.Add(folders._10);
            */
            List<SteamAppState> steamApps = new List<SteamAppState>();
            foreach (var lib in ValidLibraries)
            {
                string searchpath = Path.Combine(lib, ChildFolder);
                foreach (var acf in Directory.EnumerateFiles(searchpath, "*.acf", SearchOption.TopDirectoryOnly))
                {
                    VProperty acfVdf = VdfConvert.Deserialize(File.ReadAllText(acf));
                    JToken _acf = VTokenExtensions.ToJson(acfVdf);
                    string rawAcfJson = JsonConvert.SerializeObject(_acf);
                    rawAcfJson = $"{{{rawAcfJson}}}";
                    SteamACFRoot acfRoot = JsonConvert.DeserializeObject<SteamACFRoot>(rawAcfJson);
                    steamApps.Add(acfRoot.AppState);
                }
            }
            return steamApps;
        }

    }
    public class SteamLibraryFolders
    {
        public string TimeNextStatsReport { get; set; }
        public string ContentStatsID { get; set; }
        [JsonProperty(PropertyName = "1")]
        public string _1 { get; set; }
        [JsonProperty(PropertyName = "2")]
        public string _2 { get; set; }
        [JsonProperty(PropertyName = "3")]
        public string _3 { get; set; }
        [JsonProperty(PropertyName = "4")]
        public string _4 { get; set; }
        [JsonProperty(PropertyName = "5")]
        public string _5 { get; set; }
        [JsonProperty(PropertyName = "6")]
        public string _6 { get; set; }
        [JsonProperty(PropertyName = "7")]
        public string _7 { get; set; }
        [JsonProperty(PropertyName = "8")]
        public string _8 { get; set; }
        [JsonProperty(PropertyName = "9")]
        public string _9 { get; set; }
        [JsonProperty(PropertyName = "10")]
        public string _10 { get; set; }
    }

    public class SteamVDFRoot
    {
        public SteamLibraryFolders LibraryFolders { get; set; }
    }

    public class SteamAppState
    {
        public string appid { get; set; }
        public string Universe { get; set; }
        public string LauncherPath { get; set; }
        public string name { get; set; }
        public string StateFlags { get; set; }
        public string installdir { get; set; }
        public string LastUpdated { get; set; }
        public string UpdateResult { get; set; }
        public string SizeOnDisk { get; set; }
        public string buildid { get; set; }
        public string LastOwner { get; set; }
        public string BytesToDownload { get; set; }
        public string BytesDownloaded { get; set; }
        public string BytesToStage { get; set; }
        public string BytesStaged { get; set; }
        public string AutoUpdateBehavior { get; set; }
        public string AllowOtherDownloadsWhileRunning { get; set; }
        public string ScheduledAutoUpdate { get; set; }

        public string GetLaunchUri()
        {
            return $"steam://rungameid/{appid}";
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class SteamACFRoot
    {
        public SteamAppState AppState { get; set; }
    }
}
