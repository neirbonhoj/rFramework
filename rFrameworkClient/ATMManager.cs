using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.Functions;

namespace rFrameworkClient
{
    class ATMManager : BaseScript
    {
        private static Scaleform scaleform;
        private static Scaleform moneyHudScaleform;

        private readonly string scaleformID = "ATM";
        private readonly string moneyHudScaleformID = "hud_cash";

        public static bool isUsingATM = false;

        private readonly int[] ATMObjectHashes = new int[]
        {
            -1126237515, -1364697528, 506770882
        };

        private int currentScreen;

        private bool awaitingResult = false;
        private int returnScaleform;
        public ATMManager()
        {
            Tick += CheckIfATMNearby;
        }

        private async Task CheckIfATMNearby()
        {
            Vector3 pos = Game.PlayerPed.Position;
            foreach(int ATMObjectHash in ATMObjectHashes)
            {
                if(IsObjectNearPoint((uint)ATMObjectHash, pos.X, pos.Y, pos.Z, 2))
                {
                    SetTextComponentFormat("STRING");
                    AddTextComponentString("Press ~INPUT_CONTEXT~ to access the ATM.");
                    DisplayHelpTextFromStringLabel(0, false, true, -1);

                    //Show HUD Money
                    ShowHudComponentThisFrame(3);
                    ShowHudComponentThisFrame(4);

                    if (IsControlJustPressed(0, 51))
                    {
                        LoadATMScaleform();
                        isUsingATM = true;
                    }
                }
            }
        }

        private async void LoadATMScaleform()
        {
            scaleform = await LoadScaleform(scaleformID);
            moneyHudScaleform = await LoadScaleform(moneyHudScaleformID);
            if (scaleform != null)
            {
                HandleMouseSelection(0);
                Tick += ATMTick;
            }
        }

        private async Task ATMTick()
        {
            //Disable controls when using the ATM
            DisableAllControlActions(0);

            //Disable the mouse if using a controller
            if (GetLastInputMethod(0))
            {
                SetMouseCursorActiveThisFrame();
                scaleform.CallFunction("SET_MOUSE_INPUT", GetDisabledControlNormal(0, (int)Control.CursorX), GetDisabledControlNormal(0, (int)Control.CursorY));
            } else
            {
                scaleform.CallFunction("setCursorInvisible");
                scaleform.CallFunction("SET_MOUSE_INPUT", 0, 0);
            }

            //Handle button clicks
            if (IsDisabledControlJustPressed(0, (int)Control.Attack))
            {
                BeginScaleformMovieMethod(scaleform.Handle, "GET_CURRENT_SELECTION");
                returnScaleform = EndScaleformMovieMethodReturn();
                if (IsScaleformMovieMethodReturnValueReady(returnScaleform))
                {
                    int clickResult = GetScaleformMovieMethodReturnValueInt(returnScaleform);

                    HandleMouseSelection(clickResult);
                } else
                {
                    //If the click result isn't ready to be read, it needs to be offloaded
                    //to the next tick but not using delay
                    awaitingResult = true;
                }
            } else if(IsDisabledControlJustPressed(0, (int)Control.FrontendCancel))
            {
                CloseATM();
            }

            if (awaitingResult)
            {
                if (IsScaleformMovieMethodReturnValueReady(returnScaleform))
                {
                    int clickResult = GetScaleformMovieMethodReturnValueInt(returnScaleform);
                    awaitingResult = false;

                    HandleMouseSelection(clickResult);
                }
            }

            //Controller use
            if (IsDisabledControlJustPressed(0, 27))
            {
                scaleform.CallFunction("SET_INPUT_EVENT", 8);
            } else if (IsDisabledControlJustPressed(0, 20))
            {
                scaleform.CallFunction("SET_INPUT_EVENT", 9);
            }
            else if (IsDisabledControlJustPressed(0, 14))
            {
                scaleform.CallFunction("SET_INPUT_EVENT", 11);
            }
            else if (IsDisabledControlJustPressed(0, 15))
            {
                scaleform.CallFunction("SET_INPUT_EVENT", 10);
            }

            //Render the scaleform
            scaleform.Render2D();
            return;
        }

        private void HandleMouseSelection(int selection)
        {
            switch (selection)
            {
                case 0:
                    OpenMenuScreen();
                    break;
                case 1:
                    if (currentScreen == 0)
                    {
                        OpenWithdrawalScreen();
                    } else if(currentScreen == 1)
                    {
                        TriggerServerEvent("rFramework:MoneyTransaction", 50, true);
                    }
                    break;
                case 2:
                    if(currentScreen == 0)
                    {
                        OpenDepositScreen();
                    }
                    break;
                case 3:
                    break;
                case 4:
                    OpenMenuScreen();
                    break;
            }
        }

        public static async void UpdateDisplayBalance()
        {
            scaleform.CallFunction("DISPLAY_BALANCE", new object[]{
                Game.Player.Name, "Account balance ", (int)PlayerManager.GetPlayerBank()
            });
        }

        private async void CloseATM()
        {
            isUsingATM = false;
            int scaleformHandle = scaleform.Handle;
            SetScaleformMovieAsNoLongerNeeded(ref scaleformHandle);
            scaleform = null;
            Tick -= ATMTick;
        }

        private void OpenMenuScreen()
        {
            currentScreen = 0;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Choose a service.");
            scaleform.CallFunction("SET_DATA_SLOT", 1, "Withdraw");
            scaleform.CallFunction("SET_DATA_SLOT", 2, "Deposit");
            scaleform.CallFunction("SET_DATA_SLOT", 3, "Transaction Log");
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_MENU");
        }

        private void OpenWithdrawalScreen()
        {
            currentScreen = 1;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Select the amount you wish to withdraw from this account.");
            scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
            scaleform.CallFunction("SET_DATA_SLOT", 2, "$500");
            scaleform.CallFunction("SET_DATA_SLOT", 3, "$2,500");
            scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
            scaleform.CallFunction("SET_DATA_SLOT", 5, "$10,000");
            scaleform.CallFunction("SET_DATA_SLOT", 6, "$100,000");
            scaleform.CallFunction("SET_DATA_SLOT", 7, "$0");
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_CASH_OPTIONS");
        }

        private void OpenDepositScreen()
        {
            currentScreen = 2;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Select the amount you wish to deposit into this account.");
            scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
            scaleform.CallFunction("SET_DATA_SLOT", 2, "$500");
            scaleform.CallFunction("SET_DATA_SLOT", 3, "$2,500");
            scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
            scaleform.CallFunction("SET_DATA_SLOT", 5, "$10,000");
            scaleform.CallFunction("SET_DATA_SLOT", 6, "$100,000");
            scaleform.CallFunction("SET_DATA_SLOT", 7, "$0");
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_CASH_OPTIONS");
        }

        private async Task<Scaleform> LoadScaleform(string id)
        {
            Scaleform scaleform = new Scaleform(id);
            
            while (!scaleform.IsLoaded)
            {
                RequestScaleformMovieInteractive(id);
                await Delay(0);
            }
            return scaleform;
        }
    }
}
