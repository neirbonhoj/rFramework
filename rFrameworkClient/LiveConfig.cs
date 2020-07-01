using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
namespace rFrameworkClient
{
    class LiveConfig
    {
        public LiveConfig()
        {
            //EventHandlers.Add("rFramework:AssignConfig", new Action<string>(AssignConfig));
        }

        public static void AssignConfig(string ConfigJson)
        {
            Dictionary<string, object> config = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigJson);
            ConfigManager.config = config;

            VehicleManager.Dealerships = JsonConvert.DeserializeObject<List<rDealership>>(config["Dealerships"].ToString());
            VehicleManager.Vehicles = JsonConvert.DeserializeObject<Dictionary<string, rVehicle>>(config["Vehicles"].ToString());

            //ConfigManager.DiscordRoleIDs = (Dictionary<string, ulong>) config["DiscordRoleIDs"];
        }
    }
}
