using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static rConfig_Server_.ConfigManager;
using Newtonsoft.Json;
using rPlayerManager_Server_;
namespace rPickupManager_Server_
{
    public class PickupManager : BaseScript
    {
        private static Dictionary<int, List<object>> MoneyPickups = new Dictionary<int, List<object>>();
        private static Dictionary<int, List<object>> CasePickups = new Dictionary<int, List<object>>();
        public PickupManager()
        {
            EventHandlers.Add("rFramework:CreateMoneyPickup", new Action<Player, int, int>(CreateMoneyPickup));
            EventHandlers.Add("rFramework:CreateCasePickup", new Action<Player, int, int, int>(CreateCasePickup));
            EventHandlers.Add("rFramework:PickupCash", new Action<Player, int>(PlayerPickupCash));
            EventHandlers.Add("rFramework:PickupCase", new Action<Player, int, int>(PlayerOpenCase));
            EventHandlers.Add("rFramework:VerifyDropMoney", new Action<Player, int, long, int>(CanPlayerDropMoney));
        }

        private static void CreateMoneyPickup([FromSource] Player player, int PickupNetworkID, int Amount)
        {
            //DebugWrite("Creating Pickup | ID[" + PickupNetworkID + "] Amount[" + Amount + "]");
            MoneyPickups.Add(PickupNetworkID, new List<object>() { "MoneyDrop", Amount });
        }

        private static void CreateCasePickup([FromSource] Player player, int PickupNetworkID, int Amount, int FourDigitCode)
        {
            CasePickups.Add(PickupNetworkID, new List<object>() { "MoneyDrop", Amount, FourDigitCode });
        }

        private void CanPlayerDropMoney([FromSource] Player player, int moneyAmount, long moneyType, int fourDigitCode)
        {
            rFrameworkPlayer rPlayer;
            EventManager.GetOnlinePlayers().TryGetValue(Functions.GetPlayerDiscordID(player), out rPlayer);


            if (rPlayer != null)
            {
                long rPlayerCashAmount = rPlayer.CashBalance;
                if (moneyAmount <= rPlayerCashAmount)
                {
                    EventManager.ChangePlayerMoney(rPlayer, 0, -moneyAmount);
                    EventManager.UpdatePlayerCash(rPlayer);
                    TriggerClientEvent(player, "rFramework:CreatePickup", moneyAmount, moneyType, fourDigitCode);
                }
            }
        }

        private async static void PlayerPickupCash([FromSource] Player player, int Amount)
        {
            rFrameworkPlayer rPlayer = EventManager.GetOnlinePlayers()[Functions.GetPlayerDiscordID(player)];
            foreach (int Pickup in MoneyPickups.Keys)
            {
                if (!DoesEntityExist(Pickup))
                {
                    //parameter 3 is bank amt, 4 is cash amount
                    EventManager.ChangePlayerMoney(rPlayer, 0, Amount);
                    MoneyPickups.Remove(Pickup);
                    break;
                }
            }
        }

        private async static void PlayerOpenCase([FromSource] Player player, int Amount, int FourDigitCode)
        {
            rFrameworkPlayer rPlayer = EventManager.GetOnlinePlayers()[Functions.GetPlayerDiscordID(player)];
            foreach (int Pickup in CasePickups.Keys)
            {
                if (!DoesEntityExist(Pickup))
                {
                    EventManager.ChangePlayerMoney(rPlayer, 0, Amount);
                    CasePickups.Remove(Pickup);
                    break;
                }
            }
        }
    }
}
