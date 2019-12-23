using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static rPlayerManager_Server_.DiscordDatabase;
using static rPlayerManager_Server_.Functions;
using static rConfig.ConfigManager;
namespace rPlayerManager_Server_
{
    public class EventManager : BaseScript
    {
        private static bool IsSetupComplete = false;
        public EventManager()
        {
            if (!IsSetupComplete)
            {
                InitConfig();

                StartDiscordBotProcess();

                EventHandlers.Add("playerConnecting", new Action<Player, String, dynamic, dynamic>(PlayerConnecting));
                EventHandlers.Add("playerDropped", new Action<Player, string>(PlayerDropped));
                EventHandlers.Add("rFramework:RequestPermissions", new Action<Player>(RequestPermissions));

                Tick += CheckForDiscordRoleChange;

                IsSetupComplete = true;
            }
        }
        
        public PlayerList GetPlayers()
        {
            return Players;
        }

        public static void PlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            HandlePlayerConnectingRoles(player);
        }

        public static void PlayerDropped([FromSource] Player player, string reason)
        {
            HandlePlayerDroppedRoles(player);
        }

        public static void RequestPermissions([FromSource] Player player)
        {
            //Debug.WriteLine("^9Player Discord ID: " + GetPlayerDiscordID(player));
            //foreach(ulong u in PlayerDiscordRoles.Keys)
            //{
            //    Debug.WriteLine("^5" + u);
            //}
            //Debug.WriteLine("^1Printed All Names");
            //Debug.WriteLine("^5" + PlayerDiscordRoles.ContainsKey(GetPlayerDiscordID(player)));
            TriggerClientEvent(player, "rFramework:Permissions", JsonConvert.SerializeObject(PlayerDiscordRoles[GetPlayerDiscordID(player)]));
        }
    }
}
