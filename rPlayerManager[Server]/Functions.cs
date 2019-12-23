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
    }
}
