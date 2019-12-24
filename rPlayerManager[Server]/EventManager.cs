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
                TriggerClientEvent("");
                InitConfig();

                StartDiscordBotProcess();

                ExecuteSQLQuery("SELECT * FROM `users`");

                EventHandlers.Add("playerConnecting", new Action<Player, String, dynamic, dynamic>(PlayerConnecting));
                EventHandlers.Add("playerDropped", new Action<Player, string>(PlayerDropped));
                EventHandlers.Add("rFramework:PlayerSpawn", new Action<Player>(PlayerSpawn));

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
            Task.Run(() =>
            {
                InitializeDatabasePlayer(player);
            });
        }

        public static void PlayerDropped([FromSource] Player player, string reason)
        {
            HandlePlayerDroppedRoles(player);
        }

        public async static void PlayerSpawn([FromSource] Player player)
        {
            UpdateClientPermissions(player);
        }

        public static void UpdateClientPermissions(Player player)
        {
            List<ulong> PlayerRoles = PlayerDiscordRoles[GetPlayerDiscordID(player)];
            string PlayerRolesJson = JsonConvert.SerializeObject(PlayerRoles);
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            player.TriggerEvent("rFramework:Permissions", PlayerRolesJson);
            long milliseconds2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            DebugWrite("TCE Runtime: " + (milliseconds2 - milliseconds));
        }
    }
}
