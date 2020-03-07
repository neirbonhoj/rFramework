using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace rFrameworkClient
{
    public class rBankTransfer
    {
        public string senderName;
        public ulong recipientDiscordID;
        public string recipientName;
        public string reason;
        public bool isPlayerToPlayer;
        public bool isWithdrawal;
        public int amount;
        public DateTime time;

        [JsonConstructor]
        public rBankTransfer(string senderName, ulong recipientDiscordID, string recipientName, string reason, bool isPlayerToPlayer, bool isWithdrawal, int amount, DateTime time)
        {
            this.senderName = senderName;
            this.recipientDiscordID = recipientDiscordID;
            this.recipientName = recipientName;
            this.reason = reason;
            this.isPlayerToPlayer = isPlayerToPlayer;
            this.isWithdrawal = isWithdrawal;
            this.amount = amount;
            this.time = time;
        }
    }
}
