using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using static rFrameworkServer.DiscordDatabase;
using static rFrameworkServer.Functions;
using static rFrameworkServer.ConfigManager;
using MySql.Data.MySqlClient;

namespace rFrameworkServer
{
    public class PlayerManager : BaseScript
    {
        private static bool IsSetupComplete = false;

        private static Dictionary<ulong, rFrameworkPlayer> OnlinePlayers;
        private static Dictionary<ulong, rFrameworkPlayer> ConnectingPlayers;
        public PlayerManager()
        {
            if (!IsSetupComplete)
            {
                InitializeConfig();

                //StartDiscordBotProcess();

                EventHandlers.Add("playerConnecting", new Action<Player, string, dynamic, dynamic>(PlayerConnecting));
                EventHandlers.Add("playerDropped", new Action<Player, string>(PlayerDropped));
                EventHandlers.Add("rFramework:PlayerSpawn", new Action<Player>(PlayerSpawn));
                EventHandlers.Add("rFramework:ChangePlayerMoney", new Action<ulong, int, int>(ChangePlayerMoney));

                Tick += CheckForDiscordRoleChange;
                Tick += UpdatePlayerDatabase;

                OnlinePlayers = new Dictionary<ulong, rFrameworkPlayer>();
                ConnectingPlayers = new Dictionary<ulong, rFrameworkPlayer>();

                IsSetupComplete = true;

                foreach(Player player in Players)
                {
                    ulong PlayerDiscordID = GetPlayerDiscordID(player);

                    rFrameworkPlayer rPlayer = new rFrameworkPlayer(player, PlayerDiscordID);

                    OnlinePlayers.Add(PlayerDiscordID, rPlayer);
                    InitializeDatabasePlayer(rPlayer);

                    //Fix rPlayer 

                    rPlayer.IsPlayerLoaded = true;

                    TriggerClientEvent(player, "rFramework:AssignConfig", JsonConvert.SerializeObject(config));

                    UpdatePlayerCash(rPlayer);
                }
            }
        }
        
        public PlayerList GetPlayers()
        {
            return Players;
        }

        public static Dictionary<ulong, rFrameworkPlayer> GetOnlinePlayers()
        {
            return OnlinePlayers;
        }

        public async void PlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            ulong PlayerDiscordID = GetPlayerDiscordID(player);

            rFrameworkPlayer rPlayer = new rFrameworkPlayer(player, PlayerDiscordID);
            rPlayer.IsPlayerLoaded = false;
            ConnectingPlayers.Add(PlayerDiscordID, rPlayer);

            await InitializeDatabasePlayer(rPlayer);

            HandlePlayerWhitelist(rPlayer, deferrals);

            return;
        }

        public static void PlayerDropped([FromSource] Player player, string reason)
        {
        }

        public async Task UpdatePlayerDatabase()
        {
            if (OnlinePlayers.Values.Count > 0)
            {
                DatabaseUpdatePlayerMoney(OnlinePlayers.Values.ToList());
            }
            await Delay(5000);
        }

        public async void PlayerSpawn([FromSource] Player player)
        {
            ulong PlayerDiscordID = GetPlayerDiscordID(player);
            rFrameworkPlayer rPlayer;
            ConnectingPlayers.TryGetValue(PlayerDiscordID, out rPlayer);

            if (rPlayer!=null && !rPlayer.IsPlayerLoaded)
            {
                //Fix rPlayer 
                OnlinePlayers.Add(GetPlayerDiscordID(player), rPlayer);
                ConnectingPlayers.Remove(PlayerDiscordID);

                rPlayer.IsPlayerLoaded = true;

                UpdateClientPermissions(player);

                TriggerClientEvent(player, "rFramework:AssignConfig", JsonConvert.SerializeObject(config));

                UpdatePlayerCash(rPlayer);
            }
        }

        public static void UpdateClientPermissions(Player player)
        {
            List<ulong> PlayerRoles;
            PlayerDiscordRoles.TryGetValue(GetPlayerDiscordID(player), out PlayerRoles);
            if (PlayerRoles != null)
            {
                string PlayerRolesJson = JsonConvert.SerializeObject(PlayerRoles);
                player.TriggerEvent("rFramework:Permissions", PlayerRolesJson);
            }
        }

        public static void ChangePlayerMoney(ulong PlayerDiscordID, int BankBalanceChange, int CashBalanceChange)
        {
            rFrameworkPlayer rPlayer = OnlinePlayers[PlayerDiscordID];

            rPlayer.BankBalance += BankBalanceChange;
            rPlayer.CashBalance += CashBalanceChange;

            UpdatePlayerCash(rPlayer);
        }

        public static void ChangePlayerMoney(rFrameworkPlayer rPlayer, int BankBalanceChange, int CashBalanceChange)
        {
            rPlayer.BankBalance += BankBalanceChange;
            rPlayer.CashBalance += CashBalanceChange;

            UpdatePlayerCash(rPlayer);
        }

        public static void UpdatePlayerCash(rFrameworkPlayer rPlayer)
        {
            TriggerClientEvent(rPlayer.CorePlayer, "rFramework:UpdateMoney", rPlayer.BankBalance, rPlayer.CashBalance);
        }

        public async Task HandlePlayerWhitelist(rFrameworkPlayer rPlayer, dynamic deferrals)
        {
            if (config["ActivateDiscordWhitelist"].ToString().Equals("true"))
            {
                deferrals.defer();
                deferrals.update("Checking Whitelist...");
                bool IsWhitelisted = await CheckWhitelist(rPlayer.DiscordID);
                if (!IsWhitelisted)
                {
                    deferrals.done("Not Discord Whitelisted");
                }
                else
                {
                    deferrals.done();
                }
            }
        }

        public static void DropPlayerFromDatabaseUpdates(rFrameworkPlayer rPlayer)
        {
            OnlinePlayers.Remove(rPlayer.DiscordID);
        }
    }
}
