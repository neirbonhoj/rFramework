using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static rFrameworkPolice_Server_.Functions;
using static rFrameworkPolice_Server_.DiscordDatabase;
namespace rFrameworkPolice_Server_
{
    class EventManager : BaseScript
    {
        private byte[] ExistingPermissionFileHash;

        public EventManager()
        {
            Tick += CheckForRoleChange;
        }
    }
}
