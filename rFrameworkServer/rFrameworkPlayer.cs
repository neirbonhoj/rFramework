using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace rFrameworkServer
{
    public class rFrameworkPlayer
    {
        public Player CorePlayer; 
        public ulong DiscordID;
        public String SteamID;
        public long BankBalance;
        public long CashBalance;
        public string Vehicles;
        public bool IsPlayerLoaded;
        public List<rBankTransfer> Transfers;

        public rFrameworkPlayer(Player CorePlayer, ulong DiscordID, String SteamID)
        {
            this.CorePlayer = CorePlayer;
            this.DiscordID = DiscordID;
            this.SteamID = SteamID;
            BankBalance = 0;
            CashBalance = 0;
            Vehicles = "";
            IsPlayerLoaded = false;
            Transfers = new List<rBankTransfer>();
        }
    }
}
