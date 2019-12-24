using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static rConfig.ConfigManager;
using static CitizenFX.Core.Native.API;
using static rPlayerManager_Client_.Main;
namespace rPlayerManager_Client_
{
    public class EventManager : BaseScript
    {
        public EventManager()
        {
            RegisterCommand("getpermissions", new Action<int, List<object>, string>((source, args, raw) =>
            {
                foreach(ulong p in Permissions.Keys)
                {
                    Debug.WriteLine("^5[rFramework]" + p);
                }
            }), false);

            EventHandlers.Add("rFramework:Permissions", new Action<string>(ReceivePermissions));
            EventHandlers.Add("playerSpawned", new Action<dynamic>(PlayerSpawn));
            //Tick += UpdateUserPermissions;
        }

        public async static Task UpdateUserPermissions()
        {
            TriggerServerEvent("rFramework:RequestPermissions");
            await Delay(30000);
        }

        public static void ReceivePermissions(string jsonPermissionsList)
        {
            if (jsonPermissionsList != null)
            {
                List<ulong> PermissionsList = JsonConvert.DeserializeObject<List<ulong>>(jsonPermissionsList);
                Permissions = new Dictionary<ulong, int>();
                foreach (ulong Permission in PermissionsList)
                {
                    Permissions.Add(Permission, 1);
                }
            }
        }

        public static void PlayerSpawn(dynamic spawnInfo)
        {
            TriggerServerEvent("rFramework:PlayerSpawn");
        }
    }
}
