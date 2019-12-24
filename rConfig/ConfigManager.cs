using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
namespace rConfig
{
    public static class ConfigManager
    {
        public static Dictionary<string, object> config = new Dictionary<string, object>();
        public static readonly string PermissionFilePath = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\rDiscordBot\DiscordPermissionsFile.txt");

        public static Dictionary<string, ulong> DiscordRoleIDs = new Dictionary<string, ulong>()
        {
            ["police"] = 650174452709064714
        };

        public static string DebugPrefix = "^2[rFramework]^5";
        public static string DebugSuffix = "^0";

        public static void InitConfig()
        {
            string ConfigPath = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\rConfig.json");
            string ConfigJson = File.ReadAllText(ConfigPath);

            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigJson);
        }
    }
}
