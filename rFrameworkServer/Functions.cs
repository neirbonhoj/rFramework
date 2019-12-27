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
            MySqlDataReader SQLDataReader = (MySqlDataReader)(await SQLCommand.ExecuteReaderAsync());
            if (!SQLDataReader.Read())
            {
                SQLQuery = "INSERT INTO `users` (discordID, name) VALUES ('" + rPlayer.DiscordID + "', '" + rPlayer.CorePlayer.Name + "')";
                SQLConnection = GetDBConnection();
                SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
                await SQLConnection.OpenAsync();
                await SQLCommand.ExecuteNonQueryAsync();

                rPlayer.BankBalance = 0;
                rPlayer.CashBalance = 0;
            } else
            {
                rPlayer.BankBalance = SQLDataReader.GetInt64(2);
                rPlayer.CashBalance = SQLDataReader.GetInt64(3);
            }

            SQLCommand.Dispose();
            SQLDataReader.Dispose();
            SQLConnection.Dispose();
            return;
        }

        public static void DatabaseUpdatePlayerMoney(List<rFrameworkPlayer> players)
        {
            string SQLQuery = "INSERT INTO users (discordID, bank, cash) VALUES ";
            foreach (rFrameworkPlayer rPlayer in players)
            {
                SQLQuery += "(" + rPlayer.DiscordID + ", " + rPlayer.BankBalance + ", " + rPlayer.CashBalance + "), ";

                if(rPlayer.IsPlayerLoaded && !(rPlayer.CorePlayer.Ping>0))
                {
                    PlayerManager.DropPlayerFromDatabaseUpdates(rPlayer);
                }
            }
            SQLQuery = SQLQuery.Substring(0, SQLQuery.Length - 2) + " ";
            SQLQuery += "ON DUPLICATE KEY UPDATE bank = VALUES(bank), cash = VALUES(cash);";

            MySqlConnection SQLConnection = GetDBConnection();
            SQLConnection.Open();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            SQLCommand.ExecuteNonQuery();

            SQLCommand.Dispose();
            SQLConnection.Dispose();

            return;
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
    }
}
