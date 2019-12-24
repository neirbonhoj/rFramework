using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static rPlayerManager_Server_.Functions;
using static rConfig.ConfigManager;
using System.IO;

namespace rPlayerManager_Server_
{
    public static class DiscordDatabase
    {
        public static Dictionary<ulong, List<ulong>> PlayerDiscordRoles = new Dictionary<ulong, List<ulong>>();
        private static byte[] ExistingDiscordPermissionHash = new byte[1];
        public async static Task CheckForDiscordRoleChange()
        {
            try
            {
                byte[] NewHash = GetFileHash(PermissionFilePath);
                if (!ExistingDiscordPermissionHash.SequenceEqual(NewHash))
                {
                    string PermissionJson = System.IO.File.ReadAllText(PermissionFilePath);
                    Dictionary<ulong, List<ulong>> DiscordRoles = JsonConvert.DeserializeObject<Dictionary<ulong, List<ulong>>>(PermissionJson);
                    if (DiscordRoles == null)
                    {
                        ExistingDiscordPermissionHash = NewHash;
                        return;
                    }
                    foreach (ulong PlayerDiscordID in DiscordRoles.Keys)
                    {
                        PlayerDiscordRoles[PlayerDiscordID] = DiscordRoles[PlayerDiscordID];
                        Player UpdatedPlayer = GetPlayerFromDiscordID(PlayerDiscordID);
                        if (UpdatedPlayer != null)
                        {
                            EventManager.UpdateClientPermissions(UpdatedPlayer);
                        }
                        if (GetPlayerFromDiscordID(PlayerDiscordID) != null)
                        {
                            DebugWrite("Updated Player Permissions for " + GetPlayerFromDiscordID(PlayerDiscordID).Name);
                        }
                        else
                        {
                            DebugWrite("Updated Player Permissions for " + PlayerDiscordID + "[DiscordID]");
                        }
                    }
                    await WriteAsync("");
                    ExistingDiscordPermissionHash = GetFileHash(PermissionFilePath);
                }
            } catch(Exception e)
            {
                DebugWrite("^3File Reading Error - Noncritical (permissions must be refreshed)");
            }
        }

        public static void HandlePlayerDroppedRoles(Player ply)
        {
        }
    }
}
