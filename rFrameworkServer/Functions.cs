using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

using static rFrameworkServer.ConfigManager;

namespace rFrameworkServer
{
    public static class Functions 
    {
        #region Discord Functions
        public static void StartDiscordBotProcess()
        {
            File.WriteAllText(PermissionFilePath, "");
            string FilePath = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\rDiscordBot\rDiscordBot.exe");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.FileName = FilePath;
            //startInfo.UseShellExecute = false;
            //startInfo.CreateNoWindow = true;
            Process.Start(startInfo);
        }
        #endregion

        #region SQL Functions
        private async static Task<MySqlDataReader> ExecuteSQLQuery(string SQLQuery)
        {
            //This function is an example - in practice dont use this. Makes closing the connection difficult
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            MySqlDataReader SQLDataReader = (MySqlDataReader)(await SQLCommand.ExecuteReaderAsync());

            return SQLDataReader;
        }

        public async static Task ExecuteSQL(string SQLQuery)
        {
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            await SQLCommand.ExecuteNonQueryAsync();

            return;
        }
        public static MySqlConnection GetDBConnection(string host, int port, string database, string username, string password)
        {
            string connString = "Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + password;
            MySqlConnection conn = new MySqlConnection(connString);
            return conn;
        }
        public static MySqlConnection GetDBConnection()
        {
            object host;
            config.TryGetValue("DatabaseServer", out host);
            object port;
            config.TryGetValue("DatabasePort", out port);
            object database;
            config.TryGetValue("DatabaseName", out database);
            object username;
            config.TryGetValue("DatabaseLoginUsername", out username);
            object password;
            config.TryGetValue("DatabaseLoginPassword", out password);
            return GetDBConnection(host.ToString(), int.Parse(port.ToString()), database.ToString(), username.ToString(), password.ToString());
        }
        #endregion

        public async static Task InitializeDatabasePlayer(rFrameworkPlayer rPlayer)
        {
            string SQLQuery = "SELECT * FROM `users` WHERE `discordID` = " + rPlayer.DiscordID;
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            MySqlDataReader SQLDataReader = (MySqlDataReader)await SQLCommand.ExecuteReaderAsync();
            if (!SQLDataReader.Read())
            {
                SQLQuery = "INSERT INTO `users` (discordID, name) VALUES ('" + rPlayer.DiscordID + "', '" + rPlayer.CorePlayer.Name + "')";
                SQLConnection = GetDBConnection();
                SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
                await SQLConnection.OpenAsync();
                await SQLCommand.ExecuteNonQueryAsync();

                rPlayer.BankBalance = 0;
                rPlayer.CashBalance = 0;
                rPlayer.Vehicles = "";
            } else
            {
                rPlayer.BankBalance = SQLDataReader.GetInt64(2);
                rPlayer.CashBalance = SQLDataReader.GetInt64(3);
                rPlayer.Vehicles = SQLDataReader.GetString(4);

                SQLCommand.Dispose();
                SQLDataReader.Dispose();

                //Get transactions
                SQLQuery = "SELECT * FROM `transactions` WHERE `player_discordid` = " + rPlayer.DiscordID;
                SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
                SQLDataReader = (MySqlDataReader)await SQLCommand.ExecuteReaderAsync();

                List<rBankTransfer> transactions = new List<rBankTransfer>();

                while(SQLDataReader.Read())
                {
                    rBankTransfer transaction = new rBankTransfer(SQLDataReader.GetString(2), (ulong)SQLDataReader.GetInt64(0), 
                        SQLDataReader.GetString(1), SQLDataReader.GetString(3), (SQLDataReader.GetString(2)!=null) ? false : true, 
                        (SQLDataReader.GetString(3).Equals("Cash Withdrawn")) ? true : false, (int)SQLDataReader.GetInt64(4), DateTime.Parse(SQLDataReader.GetString(5)));
                    transactions.Add(transaction);
                }

                rPlayer.Transfers = transactions;
            }

            SQLCommand.Dispose();
            SQLDataReader.Dispose();
            SQLConnection.Dispose();
            return;
        }

        public static void DatabaseUpdatePlayer(List<rFrameworkPlayer> players)
        {
            string SQLQuery = "INSERT INTO users (discordID, bank, cash, vehicles) VALUES ";
            foreach (rFrameworkPlayer rPlayer in players)
            {
                SQLQuery += "(" + rPlayer.DiscordID + ", " + rPlayer.BankBalance + ", " + rPlayer.CashBalance + ", '" + rPlayer.Vehicles+"'), ";

                if(rPlayer.IsPlayerLoaded && !(rPlayer.CorePlayer.Ping>0))
                {
                    PlayerManager.DropPlayerFromDatabaseUpdates(rPlayer);
                }

                PlayerManager.UpdatePlayerCash(rPlayer);
                PlayerManager.UpdatePlayerTransactions(rPlayer);
            }
            SQLQuery = SQLQuery.Substring(0, SQLQuery.Length - 2) + " ";
            SQLQuery += "ON DUPLICATE KEY UPDATE bank = VALUES(bank), cash = VALUES(cash), vehicles = VALUES(vehicles);";

            MySqlConnection SQLConnection = GetDBConnection();
            SQLConnection.Open();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            SQLCommand.ExecuteNonQuery();

            SQLCommand.Dispose();
            SQLConnection.Dispose();

            return;
        }

        public static void DatabaseUpdateTransaction(rBankTransfer transfer)
        {
            string SQLQuery = "INSERT INTO transactions (player_discordid, player_name, sender_name, reason, amount, time) VALUES ";

            SQLQuery += "(" + transfer.recipientDiscordID + ", '" + transfer.recipientName 
                + "', '" + transfer.senderName + "', '" + transfer.reason + "', '"+transfer.amount+"', '"+transfer.time+"')";

            MySqlConnection SQLConnection = GetDBConnection();
            SQLConnection.Open();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            SQLCommand.ExecuteNonQuery();

            SQLCommand.Dispose();
            SQLConnection.Dispose();

            return;
        }

        public static void DatabaseAddVehicle(string VehicleUniqueID, string VehicleJSON)
        {

        }

        public static byte[] GetFileHash(string fileName)
        {
            HashAlgorithm sha1 = HashAlgorithm.Create();
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                return sha1.ComputeHash(stream);
        }

        public static Player GetPlayerFromDiscordID(ulong DiscordID)
        {
            foreach (Player p in new PlayerManager().GetPlayers())
            {
                if (ulong.Parse(p.Identifiers["discord"]).Equals(DiscordID))
                {
                    return p;
                }
            }

            return null;
        }

        public static ulong GetPlayerDiscordID(Player player)
        {
            try
            {
                if (player.Identifiers["discord"] == null)
                {
                    player.Drop("Discord ID Not Found - try opening Discord and restarting FiveM.");
                    return 0;
                }
                ulong DiscordID = ulong.Parse(player.Identifiers["discord"]);
                return DiscordID;
            } catch(Exception e)
            {
                DebugWrite("Critical Error With [GetPlayerDiscordID]");
                DebugWrite(e.Message);
                return 0;
            }
        }

        public static async Task WriteAsync(string data)
        {
            using (var sw = new StreamWriter(PermissionFilePath))
            {
                await sw.WriteAsync(data);
            }
        }

        public async static Task<bool> CheckWhitelist(ulong DiscordID)
        {
            if (DiscordID == 0)
            {
                return false;
            }

            bool IsWhitelist;
            IsWhitelist = await Task.Run(() =>
            {
                List<ulong> PlayerRoles = DiscordDatabase.PlayerDiscordRoles[DiscordID];
                ulong WhitelistRoleID = DiscordRoleIDs["Whitelist"];

                if (PlayerRoles != null && PlayerRoles.Contains(WhitelistRoleID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            return IsWhitelist;
        }

        public static void DebugWrite(object DebugObject)
        {
            CitizenFX.Core.Debug.WriteLine(DebugPrefix + DebugObject.ToString() + DebugSuffix);
        }
    }
}
