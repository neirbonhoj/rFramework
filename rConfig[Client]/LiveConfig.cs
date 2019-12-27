using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
namespace rConfig_Client_
{
    class LiveConfig : BaseScript
    {
        public LiveConfig()
        {
            EventHandlers.Add("rFramework:AssignConfig", new Action<string>(AssignConfig));
        }

        public static void AssignConfig(string ConfigJson)
        {
            Dictionary<string, object> config = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigJson);
            ConfigManager.config = config;

            //ConfigManager.DiscordRoleIDs = (Dictionary<string, ulong>) config["DiscordRoleIDs"];
        }
    }
}
