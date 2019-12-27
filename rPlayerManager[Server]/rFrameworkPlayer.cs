using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace rPlayerManager_Server_
{
    public class rFrameworkPlayer
    {
        public Player CorePlayer; 
        public ulong DiscordID;
        public long BankBalance;
        public long CashBalance;
        public bool IsPlayerLoaded;

        public rFrameworkPlayer(Player CorePlayer, ulong DiscordID)
        {
            this.CorePlayer = CorePlayer;
            this.DiscordID = DiscordID;
            BankBalance = 0;
            CashBalance = 0;
            IsPlayerLoaded = false;
        }
    }
}
