﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using static rPlayerManager_Client_.Main;
using static rPlayerManager_Client_.Functions;
using System.Threading;

namespace rPlayerManager_Client_
{
    public class EventManager : BaseScript
    {
        public EventManager()
        {
            DisplayCash(true);

            EventHandlers.Add("rFramework:UpdateMoney", new Action<long, long>(UpdateMoney));
            EventHandlers.Add("rFramework:Permissions", new Action<string>(ReceivePermissions));
            EventHandlers.Add("playerSpawned", new Action<dynamic>(PlayerSpawn));

            Tick += ControlCheck;
        }

        private async static Task ControlCheck()
        {
            if(IsControlPressed(0, 20))
            {
                ShowHudComponentThisFrame(3);
                ShowHudComponentThisFrame(4);
            }
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
            StatSetInt((uint)GetHashKey("BANK_BALANCE"), (int)BankAmount, true);
            StatSetInt((uint)GetHashKey("MP0_WALLET_BALANCE"), (int)CashAmount, true);
        }

        public static void PlayerSpawn(dynamic spawnInfo)
        {
            TriggerServerEvent("rFramework:PlayerSpawn");
        }
    }
}
