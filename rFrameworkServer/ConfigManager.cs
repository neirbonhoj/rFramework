using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Security.Cryptography;

namespace rFrameworkServer
{
    public static class ConfigManager
    {
        public static Dictionary<string, object> config;
        public static readonly string PermissionFilePath = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\rDiscordBot\DiscordPermissionsFile.txt");

        public static Dictionary<string, ulong> DiscordRoleIDs = new Dictionary<string, ulong>();

        public static string DebugPrefix = "^2[rFramework]^5";
        public static string DebugSuffix = "^0";

        public static string ConfigPath = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\rConfig.json");

        public static void InitializeConfig()
        {
            string ConfigJson = File.ReadAllText(ConfigPath);
            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigJson);

            DiscordRoleIDs = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(config["DiscordRoleIDs"].ToString());
        }
    }
}
