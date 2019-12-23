using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static rConfig.ConfigManager;
using static CitizenFX.Core.Native.API;

namespace rPlayerManager_Client_
{
    public class EventManager : BaseScript
    {
        public EventManager()
        {
            RegisterCommand("roles", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("rFramework:RequestPermissions");
            }), false);

            EventHandlers.Add("rFramework:Permissions", new Action<string>(ReceivePermissions));
        }

        public static void ReceivePermissions(string jsonPermissionsList)
        {
            List<ulong> PermissionsList = JsonConvert.DeserializeObject<List<ulong>>(jsonPermissionsList);
            foreach(ulong Permission in PermissionsList)
            {
                Debug.WriteLine("^2" + Permission);
            }
        }
    }
}
