﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.ConfigManager;
namespace rFrameworkClient
{
    public static class Functions
    {
        public static void DebugWrite(object DebugObject)
        {
            CitizenFX.Core.Debug.WriteLine(DebugPrefix + DebugObject.ToString() + DebugSuffix);
        }
    }
}
