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
            EventHandlers.Add("rFramework:RegisterMoneyPickup", new Action<Player, int, int, string>(RegisterMoneyPickup));
            EventHandlers.Add("rFramework:RegisterCasePickup", new Action<Player, int, int, string>(RegisterCasePickup));
            EventHandlers.Add("rFramework:PickupCash", new Action<Player, int>(PlayerPickupCash));
            EventHandlers.Add("rFramework:PickupCase", new Action<Player, int, int>(PlayerOpenCase));
            EventHandlers.Add("rFramework:VerifyDropMoney", new Action<Player, int, long>(CanPlayerDropMoney));
        }

        private static void RegisterMoneyPickup([FromSource] Player player, int PickupNetworkID, int Amount, string guid)
        {
            Guid clientGuid = Guid.Parse(guid);
            if (ClientGuids.Contains(clientGuid))
            {
                DebugWrite("Registering Pickup | ID[" + PickupNetworkID + "] Amount[" + Amount + "]");
                MoneyPickups.Add(PickupNetworkID, new List<object>() { "MoneyDrop", Amount });
                ClientGuids.Remove(clientGuid);
            } else
            {
                player.Drop("Unauthorized client event");
                DebugWrite("Kicking player: " + player.Name + " | Discord ID: " + GetPlayerDiscordID(player) + " for an unauthorized client event");
            }
        }

        private static void RegisterCasePickup([FromSource] Player player, int PickupNetworkID, int Amount, string guid)
        {
            Guid clientGuid = Guid.Parse(guid);
            if (ClientGuids.Contains(clientGuid))
            {
                DebugWrite("Registering Pickup | ID[" + PickupNetworkID + "] Amount[" + Amount + "]");
                CasePickups.Add(PickupNetworkID, new List<object>() { "MoneyDrop", Amount });
                ClientGuids.Remove(clientGuid);
            } else
            {
                player.Drop("Unauthorized client event");
                DebugWrite("Kicking player: " + player.Name + " | Discord ID: " + GetPlayerDiscordID(player) + " for an unauthorized client event");
            }
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

                    Guid secureClientGuid = Guid.NewGuid();

                    TriggerClientEvent(player, "rFramework:CreatePickup", moneyAmount, moneyType, secureClientGuid.ToString());
                    DebugWrite("Player " + player.Name + " has dropped ^1$" + moneyAmount);
                    DebugWrite("Generated GUID " + secureClientGuid);
                    ClientGuids.Add(secureClientGuid);
                }
            }
        }

        private async static void PlayerPickupCash([FromSource] Player player, int Amount)
        {
            rFrameworkPlayer rPlayer = PlayerManager.GetOnlinePlayers()[Functions.GetPlayerDiscordID(player)];
            foreach (int Pickup in MoneyPickups.Keys)
            {
                //if the entity no longer exists, it was the one picked up
                //this is not 100% secure. a better solution must be found
                //although cheating money through this would prove very difficult without the server code...
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
