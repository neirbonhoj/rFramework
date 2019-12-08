using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace rFrameworkPolice_Server_
{
    public static class DiscordDatabase
    {
        public static Dictionary<Player, List<ulong>> PlayerDiscordRoles = new Dictionary<Player, List<ulong>>();

        public async static Task CheckForRoleChange()
        {

        }
    }
}
