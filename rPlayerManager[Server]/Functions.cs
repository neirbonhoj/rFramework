using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static rConfig.ConfigManager;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace rPlayerManager_Server_
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
            Process.Start(startInfo);
        }
        #endregion

        #region SQL Functions
        public async static Task<MySqlDataReader> ExecuteSQLQuery(string SQLQuery)
        {
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            MySqlDataReader SQLDataReader = (MySqlDataReader)(await SQLCommand.ExecuteReaderAsync());
            return SQLDataReader;
        }

        public async static Task<bool> ExecuteSQL(string SQLQuery)
        {
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            await SQLCommand.ExecuteNonQueryAsync();

            return true;
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

        public async static void InitializeDatabasePlayer(Player player)
        {
            ulong DiscordID = GetPlayerDiscordID(player);
            using (MySqlDataReader reader = await ExecuteSQLQuery("SELECT * FROM `users` WHERE `discordID` = " + DiscordID))
            {
                if (!reader.Read())
                {
                    await ExecuteSQL("INSERT INTO `users` (discordID, name, money, vehicles) VALUES ('" + DiscordID + "', '" + player.Name + "', '0', NULL)");
                    await ExecuteSQL("UPDATE `users` SET `vehicles` = '' WHERE `discordID` = " + DiscordID);
                }
                else
                {
                    long PlayerMoney = reader.GetInt64(2);
                    string OwnedVehiclesJson = reader.GetString(3);
                }
            }
        }


        public static byte[] GetFileHash(string fileName)
        {
            HashAlgorithm sha1 = HashAlgorithm.Create();
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                return sha1.ComputeHash(stream);
        }
        public static Player GetPlayerFromDiscordID(ulong DiscordID)
        {
            foreach (Player p in new EventManager().GetPlayers())
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
            ulong DiscordID = ulong.Parse(player.Identifiers["discord"]);
            return DiscordID;
        }

        public static async Task WriteAsync(string data)
        {
            using (var sw = new StreamWriter(PermissionFilePath))
            {
                await sw.WriteAsync(data);
            }
        }

        public static void DebugWrite(string DebugMessage)
        {
            CitizenFX.Core.Debug.WriteLine(DebugPrefix + DebugMessage + DebugSuffix);
        }
    }
}
