﻿using System;
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

        private readonly string scaleformID = "ATM";

        public static bool isUsingATM = false;

        private static bool latestTransactionIsWithdrawal = false;
        private static int latestTransactionAmount = 0;

        private static Dictionary<int, int> buttonParams;

        private readonly int[] ATMObjectHashes = new int[]
        {
            -1126237515, -1364697528, 506770882
        };

        private static int currentScreen;

        private bool awaitingResult = false;
        private int returnScaleform;
        public ATMManager()
        {
            EventHandlers.Add("rFramework:ATMTransactionSuccess", new Action(OpenTransactionComplete));

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

                    if (IsControlPressed(0, 51) && !isUsingATM)
                    {
                        LoadATMScaleform();
                        isUsingATM = true;
                    }
                }
            }
            await Delay(0);
        }

        private async void LoadATMScaleform()
        {
            scaleform = await LoadScaleform(scaleformID);
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
            if (currentScreen == 0)
            {
                switch (selection)
                {
                    case 1:
                        OpenWithdrawalScreen();
                        break;
                    case 2:
                        OpenDepositScreen();
                        break;
                    case 4:
                        CloseATM();
                        break;
                    default:
                        OpenMenuScreen();
                        break;
                }
            } else if (currentScreen == 1 || currentScreen == 2)
            {
                switch (selection)
                {
                    case 4:
                        OpenMenuScreen();
                        break;
                    default:
                        DepositWithdrawalButtonHandle(buttonParams[selection]);
                        break;
                }
            } else if(currentScreen == 3)
            {
                switch (selection)
                {
                    case 1:
                        OpenTransactionPending();
                        TriggerServerEvent("rFramework:MoneyTransaction", latestTransactionAmount, latestTransactionIsWithdrawal);
                        break;
                    case 2:
                        OpenWithdrawalScreen();
                        break;
                }
            } else if (currentScreen == 5)
            {
                OpenMenuScreen();
            }
        }

        private static void DepositWithdrawalButtonHandle(int cashAmount)
        {
            if (currentScreen == 1)
            {
                OpenConfirmationScreen(true, cashAmount);
                latestTransactionAmount = cashAmount;
            }
            else if (currentScreen == 2)
            {
                OpenConfirmationScreen(false, cashAmount);
                latestTransactionAmount = cashAmount;
            }
            else if (currentScreen == 5)
            {
                OpenMenuScreen();
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

        private static void OpenMenuScreen()
        {
            currentScreen = 0;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Choose a service.");
            scaleform.CallFunction("SET_DATA_SLOT", 1, "Withdraw");
            scaleform.CallFunction("SET_DATA_SLOT", 2, "Deposit");
            scaleform.CallFunction("SET_DATA_SLOT", 3, "Transaction Log");
            scaleform.CallFunction("SET_DATA_SLOT", 4, "Exit");
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_MENU");
        }

        private void OpenWithdrawalScreen()
        {
            currentScreen = 1;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Select the amount you wish to withdraw from this account.");

            int amount = (int)PlayerManager.GetPlayerCash();
            SetupATMMoneyButtons(amount);

            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_CASH_OPTIONS");

            latestTransactionIsWithdrawal = true;
        }

        private void OpenDepositScreen()
        {
            currentScreen = 2;


            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Select the amount you wish to deposit into this account.");

            int amount = (int)PlayerManager.GetPlayerCash();
            SetupATMMoneyButtons(amount);
           
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_CASH_OPTIONS");

            latestTransactionIsWithdrawal = false;
        }

        private static void SetupATMMoneyButtons(int amount)
        {
            if (amount >= 100000)
            {
                buttonParams = new Dictionary<int, int>()
                {
                    {1, 50 },
                    {2, 500 },
                    {3, 2500 },
                    {5, 10000 },
                    {6, 100000 },
                    {7, amount },
                };
                scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
                scaleform.CallFunction("SET_DATA_SLOT", 2, "$500");
                scaleform.CallFunction("SET_DATA_SLOT", 3, "$2,500");
                scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
                scaleform.CallFunction("SET_DATA_SLOT", 5, "$10,000");
                scaleform.CallFunction("SET_DATA_SLOT", 6, "$100,000");
                if (buttonParams.Values.Contains<int>(amount))
                {
                    scaleform.CallFunction("SET_DATA_SLOT", 7, "$" + String.Format("{0:n0}", Math.Abs(amount)));
                }
            }
            else if (amount >= 10000)
            {
                buttonParams = new Dictionary<int, int>()
                {
                    {1, 50 },
                    {2, 500 },
                    {3, 2500 },
                    {5, 10000 },
                    {6, amount },
                };
                scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
                scaleform.CallFunction("SET_DATA_SLOT", 2, "$500");
                scaleform.CallFunction("SET_DATA_SLOT", 3, "$2,500");
                scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
                scaleform.CallFunction("SET_DATA_SLOT", 5, "$10,000");
                if (buttonParams.Values.Contains<int>(amount))
                {
                    scaleform.CallFunction("SET_DATA_SLOT", 6, "$" + String.Format("{0:n0}", Math.Abs(amount)));
                }
            }
            else if (amount >= 2500)
            {
                buttonParams = new Dictionary<int, int>()
                {
                    {1, 50 },
                    {2, 500 },
                    {3, 2500 },
                    {5, amount },
                };
                scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
                scaleform.CallFunction("SET_DATA_SLOT", 2, "$500");
                scaleform.CallFunction("SET_DATA_SLOT", 3, "$2,500");
                scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
                if (buttonParams.Values.Contains<int>(amount))
                {
                    scaleform.CallFunction("SET_DATA_SLOT", 5, "$" + String.Format("{0:n0}", Math.Abs(amount)));
                }
            }
            else if (amount >= 500)
            {
                buttonParams = new Dictionary<int, int>()
                {
                    {1, 50 },
                    {2, 500 },
                    {3, amount },
                };
                scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
                scaleform.CallFunction("SET_DATA_SLOT", 2, "$500");
                scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
                if (buttonParams.Values.Contains<int>(amount))
                {
                    scaleform.CallFunction("SET_DATA_SLOT", 3, "$" + String.Format("{0:n0}", Math.Abs(amount)));
                }
            }
            else if (amount >= 50)
            {
                buttonParams = new Dictionary<int, int>()
                {
                    {1, 50 },
                    {2, amount },
                };
                scaleform.CallFunction("SET_DATA_SLOT", 1, "$50");
                scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
                if (buttonParams.Values.Contains<int>(amount))
                {
                    scaleform.CallFunction("SET_DATA_SLOT", 2, "$" + String.Format("{0:n0}", Math.Abs(amount)));
                }
            }
            else
            {
                buttonParams = new Dictionary<int, int>()
                {
                    {1, amount }
                };
                scaleform.CallFunction("SET_DATA_SLOT", 4, "Back");
                if (buttonParams.Values.Contains<int>(amount))
                {
                    scaleform.CallFunction("SET_DATA_SLOT", 1, "$" + String.Format("{0:n0}", Math.Abs(amount)));
                }
            }
        }

        private static void OpenConfirmationScreen(bool isWithdrawal, int amount)
        {
            currentScreen = 3;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            if (isWithdrawal)
            {
                scaleform.CallFunction("SET_DATA_SLOT", 0, "Do you wish to withdraw $" + String.Format("{0:n0}", Math.Abs(amount)) + " from your account?");
            } else
            {
                scaleform.CallFunction("SET_DATA_SLOT", 0, "Do you wish to deposit $" + String.Format("{0:n0}", Math.Abs(amount)) + " into your account?");
            }
            scaleform.CallFunction("SET_DATA_SLOT", 1, "Yes");
            scaleform.CallFunction("SET_DATA_SLOT", 2, "No");
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_MESSAGE");
        }

        private void OpenTransactionPending()
        {
            currentScreen = 4;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Transaction Pending");
            scaleform.CallFunction("DISPLAY_MESSAGE");
        }

        private void OpenTransactionComplete()
        {
            currentScreen = 5;

            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Transaction Complete");
            scaleform.CallFunction("SET_DATA_SLOT", 1, "Back");
            UpdateDisplayBalance();
            scaleform.CallFunction("DISPLAY_MESSAGE");
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
