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
        public static bool UsingDiscordIntegration = false;

        private static Dictionary<String, rFrameworkPlayer> OnlinePlayers;
        private static Dictionary<String, rFrameworkPlayer> ConnectingPlayers;
        public PlayerManager()
        {
            if (!IsSetupComplete)
            {
                InitializeConfig();

                //StartDiscordBotProcess();
                object ConfigUseDiscordIntegration;
                config.TryGetValue("DiscordIntegration", out ConfigUseDiscordIntegration);

                EventHandlers.Add("playerConnecting", new Action<Player, string, dynamic, dynamic>(PlayerConnecting));
                EventHandlers.Add("playerDropped", new Action<Player, string>(PlayerDropped));
                EventHandlers.Add("rFramework:PlayerSpawn", new Action<Player>(PlayerSpawn));
                EventHandlers.Add("rFramework:MoneyTransaction", new Action<Player, int, bool>(PlayerMoneyTransaction));

                if (ConfigUseDiscordIntegration.ToString().ToLower().Equals("true"))
                {
                    UsingDiscordIntegration = true;
                }

                OnlinePlayers = new Dictionary<String, rFrameworkPlayer>();
                ConnectingPlayers = new Dictionary<String, rFrameworkPlayer>();

                IsSetupComplete = true;

                foreach(Player player in Players)
                {
                    ulong PlayerDiscordID;
                    if (UsingDiscordIntegration)
                    {
                        PlayerDiscordID = GetPlayerDiscordID(player);
                    } else
                    {
                        PlayerDiscordID = 0;

                    }

                    String PlayerSteamID = GetPlayerSteamID(player);

                    rFrameworkPlayer rPlayer = new rFrameworkPlayer(player, PlayerDiscordID, PlayerSteamID);

                    OnlinePlayers.Add(PlayerSteamID, rPlayer);
                    InitializeDatabasePlayer(rPlayer);

                    //Fix rPlayer 

                    rPlayer.IsPlayerLoaded = true;

                    TriggerClientEvent(player, "rFramework:AssignConfig", JsonConvert.SerializeObject(config));

                    UpdatePlayerCash(rPlayer);
                    UpdatePlayerTransactions(rPlayer);
                }

                if (UsingDiscordIntegration)
                {
                    Tick += CheckForDiscordRoleChange;
                }
                Tick += UpdatePlayerDatabase;
            }
        }
        
        public PlayerList GetPlayers()
        {
            return Players;
        }

        public static Dictionary<String, rFrameworkPlayer> GetOnlinePlayers()
        {
            return OnlinePlayers;
        }

        public async void PlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            ulong PlayerDiscordID;
            if (UsingDiscordIntegration)
            {
                PlayerDiscordID = GetPlayerDiscordID(player);
            }
            else
            {
                PlayerDiscordID = 0;

            }

            String PlayerSteamID = GetPlayerSteamID(player);

            rFrameworkPlayer rPlayer = new rFrameworkPlayer(player, PlayerDiscordID, PlayerSteamID);
            rPlayer.IsPlayerLoaded = false;
            //Account for player disconnected while connecting
            ConnectingPlayers.Remove(PlayerSteamID);
            ConnectingPlayers.Add(PlayerSteamID, rPlayer);

            await InitializeDatabasePlayer(rPlayer);

            HandlePlayerWhitelist(rPlayer, deferrals);

            return;
        }

        public static void PlayerDropped([FromSource] Player player, string reason)
        {
            DebugWrite("Player " + player.Name + " leaving - updating database");
            DebugWrite("    Steam ID: " + GetPlayerSteamID(player));
            rFrameworkPlayer rPlayer = new rFrameworkPlayer(player, GetPlayerDiscordID(player), GetPlayerSteamID(player));
            OnlinePlayers.Remove(rPlayer.SteamID);

            //DatabaseUpdatePlayer(new List<rFrameworkPlayer>() { rPlayer });
        }

        public async Task UpdatePlayerDatabase()
        {
            if (OnlinePlayers.Values.Count > 0)
            {
                DatabaseUpdatePlayer(OnlinePlayers.Values.ToList());
            }
            await Delay(1000);
        }

        public void PlayerSpawn([FromSource] Player player)
        {
            String PlayerSteamID = GetPlayerSteamID(player);
            rFrameworkPlayer rPlayer;
            ConnectingPlayers.TryGetValue(PlayerSteamID, out rPlayer);

            if (rPlayer!=null && !rPlayer.IsPlayerLoaded)
            {
                //Fix rPlayer 
                OnlinePlayers.Add(GetPlayerSteamID(player), rPlayer);
                ConnectingPlayers.Remove(PlayerSteamID);
                rPlayer.IsPlayerLoaded = true;

                UpdateClientPermissions(player);
                UpdatePlayerCash(rPlayer);
                VehicleManager.UpdatePlayerVehicles(rPlayer);

                TriggerClientEvent(player, "rFramework:AssignConfig", JsonConvert.SerializeObject(config));
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

        public static void ChangePlayerMoney(String PlayerSteamID, int BankBalanceChange, int CashBalanceChange)
        {
            rFrameworkPlayer rPlayer = OnlinePlayers[PlayerSteamID];

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

        public static void UpdatePlayerTransactions(rFrameworkPlayer rPlayer)
        {
            TriggerClientEvent(rPlayer.CorePlayer, "rFramework:UpdateTransactions", JsonConvert.SerializeObject(rPlayer.Transfers));
        }

        public static void PlayerMoneyTransaction([FromSource] Player player, int amount, bool isWithdrawal)
        {
            rFrameworkPlayer rPlayer = GetrFrameworkPlayer(player);
            amount = Math.Abs(amount);

            //Determine if transaction is a withdrawal (amount less than zero) or deposit (amount greater than zero)
            if (isWithdrawal)
            {
                if(rPlayer.BankBalance - amount >= 0)
                {
                    //Transaction allowed
                    ChangePlayerMoney(rPlayer, -amount, amount);
                    DebugWrite("Player " + player.Name + " withdrew ^2$" + Math.Abs(amount));
                    TriggerClientEvent(player, "rFramework:ATMTransactionSuccess");

                    rBankTransfer transfer = new rBankTransfer(rPlayer.SteamID, rPlayer.SteamID, "Cash Withdrawn", false, true, amount, DateTime.Now);
                    rPlayer.Transfers.Add(transfer);
                    DatabaseUpdateTransaction(transfer);
                    UpdatePlayerTransactions(rPlayer);
                } else
                {
                    //Transaction invalid
                }
            } else
            {
                if (rPlayer.CashBalance - amount >= 0)
                {
                    //Transaction allowed
                    ChangePlayerMoney(rPlayer, amount, -amount);
                    DebugWrite("Player " + player.Name + " deposited ^1$" + Math.Abs(amount));
                    TriggerClientEvent(player, "rFramework:ATMTransactionSuccess");

                    rBankTransfer transfer = new rBankTransfer(rPlayer.SteamID, rPlayer.SteamID, "Cash Deposited", false, false, amount, DateTime.Now);
                    rPlayer.Transfers.Add(transfer);
                    DatabaseUpdateTransaction(transfer);
                    UpdatePlayerTransactions(rPlayer);
                }
                else
                {
                    //Transaction invalid
                }
            }
        }

        public async Task HandlePlayerWhitelist(rFrameworkPlayer rPlayer, dynamic deferrals)
        {
            if (config["ActivateDiscordWhitelist"].ToString().Equals("true"))
            {
                deferrals.defer();
                deferrals.update("Checking Whitelist...");
                bool IsWhitelisted = await CheckWhitelist(rPlayer.DiscordID);
                await Delay(0);
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
            OnlinePlayers.Remove(rPlayer.SteamID);
        }

        public static rFrameworkPlayer GetrFrameworkPlayer(Player player)
        {
            rFrameworkPlayer rPlayer = null;
            OnlinePlayers.TryGetValue(GetPlayerSteamID(player), out rPlayer);
            return rPlayer;
        }
    }
}
