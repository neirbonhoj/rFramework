using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace rConfig
{
    class InitConfig : BaseScript
    {
        public InitConfig()
        {
            ConfigManager.InitConfig();
        }
    }
}
