﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.Functions;
using System.Threading;

namespace rFrameworkClient
{
    public class PlayerManager : BaseScript
    {
        public static Dictionary<ulong, int> Permissions = new Dictionary<ulong, int>();
        private static long PlayerCash;
        private static long PlayerBank;

        public PlayerManager()
        {
            DisplayCash(true);

            EventHandlers.Add("rFramework:UpdateMoney", new Action<long, long>(UpdateMoney));
            EventHandlers.Add("rFramework:UpdateTransactions", new Action<string>(UpdateTransactions));
            EventHandlers.Add("rFramework:Permissions", new Action<string>(ReceivePermissions));
            EventHandlers.Add("playerSpawned", new Action<dynamic>(PlayerSpawn));

            Tick += ControlCheck;
        }

        private async static Task ControlCheck()
        {
            if (IsControlPressed(0, 20))
            {
                ShowHudComponentThisFrame(3);
                ShowHudComponentThisFrame(4);
            }
            await Delay(100);
        }

        public static void ReceivePermissions(string jsonPermissionsList)
        {
            if (jsonPermissionsList != null)
            {
                List<ulong> PermissionsList = JsonConvert.DeserializeObject<List<ulong>>(jsonPermissionsList);
                Permissions = new Dictionary<ulong, int>();
                foreach (ulong Permission in PermissionsList)
                {
                    Permissions.Add(Permission, 1);
                }
            }
        }

        public static void UpdateMoney(long BankAmount, long CashAmount)
        {
            ProfilerEnterScope("SetStatInt - 1");
            PlayerCash = CashAmount;
            PlayerBank = BankAmount;
            ProfilerExitScope();
            ProfilerEnterScope("SetStatInt - UpdateMoney");
            StatSetInt((uint)GetHashKey("BANK_BALANCE"), (int)BankAmount, true);
            StatSetInt((uint)GetHashKey("MP0_WALLET_BALANCE"), (int)CashAmount, true);
            ProfilerExitScope();
            ProfilerEnterScope("SetStatInt - 2");
            if (ATMManager.isUsingATM)
            {
                //ATMManager.UpdateDisplayBalance();
            }
            ProfilerExitScope();
        }

        public static void UpdateTransactions(string transfersJson)
        {
            List<rBankTransfer> transactions = JsonConvert.DeserializeObject<List<rBankTransfer>>(transfersJson);
            ATMManager.transactions = transactions;
        }

        public static void PlayerSpawn(dynamic spawnInfo)
        {
            TriggerServerEvent("rFramework:PlayerSpawn");
        }

        public static long GetPlayerCash()
        {
            return PlayerCash;
        }

        public static long GetPlayerBank()
        {
            return PlayerBank;
        }
    }
}
