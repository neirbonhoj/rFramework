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

        public static void InitConfig()
        {
            string ConfigPath = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\rConfig.json");
            string ConfigJson = File.ReadAllText(ConfigPath);

            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigJson);
        }
    }
}
