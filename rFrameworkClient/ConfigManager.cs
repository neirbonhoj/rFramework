using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
namespace rFrameworkClient
{
    public static class ConfigManager
    {
        public static Dictionary<string, object> config = new Dictionary<string, object>();

        public static Dictionary<string, ulong> DiscordRoleIDs = new Dictionary<string, ulong>();

        public static string DebugPrefix = "^2[rFramework]^5";
        public static string DebugSuffix = "^0";
    }
}
