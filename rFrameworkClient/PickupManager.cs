using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.Functions;
using Newtonsoft.Json;
using static rFrameworkClient.ConfigManager;
namespace rFrameworkClient
{
    class PickupManager : BaseScript
    {
        private static readonly Model CashModel = new Model("PICKUP_MONEY_VARIABLE");
        private static readonly long CashPickup = 4263048111; //PickupType.MoneyVariable
        private static readonly Model CaseModel = new Model("PICKUP_MONEY_SECURITY_CASE");
        private static readonly long CasePickup = 3732468094; //PickupType.MoneySecurityCase
        private static readonly Model CasePropModel = new Model("prop_security_case_01");
        private static List<Prop> ActivePickups = new List<Prop>();

        public PickupManager()
        {
            RegisterCommand("dropmoney", new Action<int, List<object>, string>((source, args, raw) =>
            {
                int MaxPickups;
                int MoneyAmount = int.Parse(args[0].ToString());
                try
                {
                    MaxPickups = int.Parse(config["MaxPickupCount"].ToString());
                } catch(Exception e)
                {
                    MaxPickups = 3;
                }
                if (ActivePickups.Count < MaxPickups)
                {
                    if (MoneyAmount < 20000)
                    {
                        TriggerServerEvent("rFramework:VerifyDropMoney", MoneyAmount, CashPickup, 0);
                    } else
                    {
                        TriggerServerEvent("rFramework:VerifyDropMoney", MoneyAmount, CasePickup, 0);
                    }
                } else
                {
                    bool removed = false;
                    foreach(Prop p in ActivePickups)
                    {
                        if (!p.Exists())
                        {
                            ActivePickups.Remove(p);
                            removed = true;
                            break;
                        }
                    }
                    if (removed)
                    {
                        if (MoneyAmount < 20000)
                        {
                            TriggerServerEvent("rFramework:VerifyDropMoney", MoneyAmount, CashPickup, 0);
                        }
                        else
                        {
                            TriggerServerEvent("rFramework:VerifyDropMoney", MoneyAmount, CasePickup, 0);
                        }
                    } else
                    {
                        SetNotificationTextEntry("STRING");
                        AddTextComponentString("Max Pickup Count Met");
                        DrawNotification(true, false);
                    }
                }
            }), false);

            //RegisterCommand("dropmoneycase", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    int MaxPickups;
            //    try
            //    {
            //        MaxPickups = int.Parse(config["MaxPickupCount"].ToString());
            //    }
            //    catch (Exception e)
            //    {
            //        MaxPickups = 3;
            //    }
            //    if (ActivePickups.Count < MaxPickups)
            //    {
            //        TriggerServerEvent("rFramework:VerifyDropMoney", int.Parse(args[0].ToString()), CasePickup, 1234);
            //    }
            //    else
            //    {
            //        bool removed = false;
            //        foreach (Prop p in ActivePickups)
            //        {
            //            if (!p.Exists())
            //            {
            //                ActivePickups.Remove(p);
            //                removed = true;
            //                break;
            //            }
            //        }
            //        if (removed)
            //        {
            //            TriggerServerEvent("rFramework:VerifyDropMoney", int.Parse(args[0].ToString()), CasePickup, 1234);
            //        }
            //        else
            //        {
            //            SetNotificationTextEntry("STRING");
            //            AddTextComponentString("Max Pickup Count Met");
            //            DrawNotification(true, false);
            //        }
            //    }
            //}), false);

            RegisterCommand("kill", new Action<int, List<object>, string>((source, args, raw) =>
            {
                ApplyDamageToPed(PlayerPedId(), 1000, false);
            }), false);

            EventHandlers.Add("gameEventTriggered", new Action<string, List<dynamic>>(OnGameEventTriggered));
            EventHandlers.Add("rFramework:CreatePickup", new Action<int, long, int>(DropPickup));
        }

        private async void DropPickup(int amount, long type, int fourDigitCode)
        {
            Vector3 PC = GetEntityCoords(Game.PlayerPed.Handle, true);
            Vector3 PFV = GetEntityForwardVector(Game.PlayerPed.Handle) * 1.2f;
            Vector3 SPC = GetSafePickupCoords(PC.X + PFV.X, PC.Y + PFV.Y, PC.Z + PFV.Z, 1, 1);

            Prop PickupProp = null;

            if (type == CashPickup)
            {
                PickupProp = await CreateAmbientPickup(PickupType.MoneyVariable, SPC, CashModel, amount);
                TriggerServerEvent("rFramework:CreateMoneyPickup", PickupProp.NetworkId, amount);
            } else if(type == CasePickup)
            {
                PickupProp = await CreateAmbientPickup(PickupType.MoneySecurityCase, SPC, CaseModel, amount);
                TriggerServerEvent("rFramework:CreateCasePickup", PickupProp.NetworkId, amount, fourDigitCode);
            }
            ActivePickups.Add(PickupProp);
            SetEntityAsMissionEntity(PickupProp.Handle, true, true);

            AddBlipForPickup(PickupProp.Handle);
        }

        private async void OnGameEventTriggered(string name, List<object> args)
        {
            int amount = 0;
            if (args.Count > 6)
            {
                amount = await ParseObjectToInt(args[6]);
            }
            if (name.Equals("CEventNetworkPlayerCollectedAmbientPickup"))
            {
                if (args[0].ToString().Equals(CaseModel.Hash.ToString()))
                {
                    TriggerServerEvent("rFramework:PickupCase", amount);
                } else if(args[0].ToString().Equals(CashModel.Hash.ToString()))
                {
                    TriggerServerEvent("rFramework:PickupCash", amount);
                }
            }
        }

        public static async Task<Prop> CreateAmbientPickup(PickupType type, Vector3 position, Model model, int value)
        {
            int handle = CitizenFX.Core.Native.API.CreateAmbientPickup((uint)type, position.X, position.Y, position.Z, 0, value, (uint)model.Hash, false, true);

            if (handle == 0)
            {
                return null;
            }

            return new Prop(handle);
        }

        public async static Task<int> ParseObjectToInt(object o)
        {
            return int.Parse(o.ToString());
        }

        public async static Task LoadModel(Model model)
        {
            RequestModel((uint)model.Hash);
            while (!model.IsLoaded)
            {
                await Delay(2);
            }
            return;
        }

        public async static void playAnim(string dict, string name, int duration, float leadIn)
        {
            while (!HasAnimDictLoaded(dict))
            {
                RequestAnimDict(dict);
            }
            TaskPlayAnim(Game.PlayerPed.Handle, dict, name, leadIn, 1.0f, duration, 12, 0, false, false, true);
        }
    }
}
