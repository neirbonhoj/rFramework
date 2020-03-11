using CitizenFX.Core;
using System;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;
using static rFrameworkServer.Functions;
namespace rFrameworkServer
{
    public class PickupManager : BaseScript
    {
        private static Dictionary<int, List<object>> MoneyPickups = new Dictionary<int, List<object>>();
        private static Dictionary<int, List<object>> CasePickups = new Dictionary<int, List<object>>();

        //Used to verify events sent from the client
        private static List<Guid> ClientGuids = new List<Guid>();

        public PickupManager()
        {
            EventHandlers.Add("rFramework:CreateMoneyPickup", new Action<Player, int, int>(CreateMoneyPickup));
            EventHandlers.Add("rFramework:CreateCasePickup", new Action<Player, int, int>(CreateCasePickup));
            EventHandlers.Add("rFramework:PickupCash", new Action<Player, int>(PlayerPickupCash));
            EventHandlers.Add("rFramework:PickupCase", new Action<Player, int, int>(PlayerOpenCase));
            EventHandlers.Add("rFramework:VerifyDropMoney", new Action<Player, int, long>(CanPlayerDropMoney));
        }

        private static void CreateMoneyPickup([FromSource] Player player, int PickupNetworkID, int Amount)
        {
            //DebugWrite("Creating Pickup | ID[" + PickupNetworkID + "] Amount[" + Amount + "]");
            MoneyPickups.Add(PickupNetworkID, new List<object>() { "MoneyDrop", Amount });
        }

        private static void CreateCasePickup([FromSource] Player player, int PickupNetworkID, int Amount)
        {
            CasePickups.Add(PickupNetworkID, new List<object>() { "MoneyDrop", Amount });
        }

        private void CanPlayerDropMoney([FromSource] Player player, int moneyAmount, long moneyType)
        {
            rFrameworkPlayer rPlayer;
            PlayerManager.GetOnlinePlayers().TryGetValue(GetPlayerDiscordID(player), out rPlayer);

            if (rPlayer != null)
            {
                long rPlayerCashAmount = rPlayer.CashBalance;
                if (moneyAmount <= rPlayerCashAmount)
                {
                    PlayerManager.ChangePlayerMoney(rPlayer, 0, -moneyAmount);
                    PlayerManager.UpdatePlayerCash(rPlayer);

                    Guid secureClientGuid = new Guid();

                    TriggerClientEvent(player, "rFramework:CreatePickup", moneyAmount, moneyType, secureClientGuid);
                    DebugWrite("Player " + player.Name + " has dropped ^1$" + moneyAmount);

                    ClientGuids.Add(secureClientGuid);
                }
            }
        }

        private async static void PlayerPickupCash([FromSource] Player player, int Amount)
        {
            rFrameworkPlayer rPlayer = PlayerManager.GetOnlinePlayers()[Functions.GetPlayerDiscordID(player)];
            foreach (int Pickup in MoneyPickups.Keys)
            {
                if (!DoesEntityExist(Pickup))
                {
                    //parameter 3 is bank amt, 4 is cash amount
                    PlayerManager.ChangePlayerMoney(rPlayer, 0, Amount);
                    MoneyPickups.Remove(Pickup);
                    DebugWrite("Player " + player.Name + " has picked up ^4$" + Amount);
                    break;
                }
            }
        }

        private async static void PlayerOpenCase([FromSource] Player player, int Amount, int FourDigitCode)
        {
            rFrameworkPlayer rPlayer = PlayerManager.GetOnlinePlayers()[Functions.GetPlayerDiscordID(player)];
            foreach (int Pickup in CasePickups.Keys)
            {
                if (!DoesEntityExist(Pickup))
                {
                    PlayerManager.ChangePlayerMoney(rPlayer, 0, Amount);
                    CasePickups.Remove(Pickup);
                    DebugWrite("Player " + player.Name + " has picked up ^2$" + Amount);
                    break;
                }
            }
        }
    }
}
