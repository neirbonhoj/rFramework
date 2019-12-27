using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static rConfig_Client_.ConfigManager;
namespace rPlayerManager_Client_
{
    public static class Functions
    {
        public static void DebugWrite(string DebugMessage)
        {
            Debug.WriteLine(DebugPrefix + DebugMessage + DebugSuffix);
        }
    }
}
