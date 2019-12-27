using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using static rConfig_Server_.ConfigManager;
namespace rFrameworkPolice_Server_
{
    public static class Functions
    {
        #region SQL Functions
        public async static Task<MySqlDataReader> ExecuteSQLQuery(string SQLQuery)
        {
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            MySqlDataReader SQLDataReader = (MySqlDataReader)(await SQLCommand.ExecuteReaderAsync());
            return SQLDataReader;
        }
        public async static void ExecuteSQL(string SQLQuery)
        {
            MySqlConnection SQLConnection = GetDBConnection();
            await SQLConnection.OpenAsync();
            MySqlCommand SQLCommand = new MySqlCommand(SQLQuery, SQLConnection);
            await SQLCommand.ExecuteNonQueryAsync();
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
    }
}
