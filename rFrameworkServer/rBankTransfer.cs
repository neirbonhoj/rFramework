using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace rFrameworkServer
{
    public class rBankTransfer
    {
        public String sender_steamid;
        public String recipient_steamid;
        public string reason;
        public bool isPlayerToPlayer;
        public bool isWithdrawal;
        public int amount;
        public DateTime time;

        [JsonConstructor]
        public rBankTransfer(String sender_steamid, String recipient_steamid, string reason, bool isPlayerToPlayer, bool isWithdrawal, int amount, DateTime time)
        {
            this.sender_steamid = sender_steamid;
            this.recipient_steamid = recipient_steamid;
            this.reason = reason;
            this.isPlayerToPlayer = isPlayerToPlayer;
            this.isWithdrawal = isWithdrawal;
            this.amount = amount;
            this.time = time;
        }
    }
}
