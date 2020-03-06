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
        private Scaleform scaleform;
        private readonly string scaleformID = "ATM";

        private int currentScreen;

        private bool awaitingResult = false;
        private int returnScaleform;
        public ATMManager()
        {
            RegisterCommand("ts", new Action<int, List<object>, string>((source, args, raw) =>
            {
                LoadScaleform(scaleformID);
                if (scaleform != null)
                {
                    LoadNewScreen(0);
                    Tick += ScaleformTesting_Tick;
                }
            }), false);
        }

        private async Task ScaleformTesting_Tick()
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
                    DebugWrite(GetScaleformMovieMethodReturnValueInt(returnScaleform));
                } else
                {
                    //If the click result isn't ready to be read, it needs to be offloaded
                    //to the next tick but not using delay
                    awaitingResult = true;
                }
            }

            if (awaitingResult)
            {
                if (IsScaleformMovieMethodReturnValueReady(returnScaleform))
                {
                    int clickResult = GetScaleformMovieMethodReturnValueInt(returnScaleform);
                    awaitingResult = false;

                    LoadNewScreen(clickResult);
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

        private void LoadNewScreen(int screen)
        {
            if (screen == 0)
            {
                //Menu screen
                currentScreen = 0;

                scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
                scaleform.CallFunction("SET_DATA_SLOT", 0, "Choose a service.");
                scaleform.CallFunction("SET_DATA_SLOT", 1, "Withdraw");
                scaleform.CallFunction("SET_DATA_SLOT", 2, "Deposit");
                scaleform.CallFunction("SET_DATA_SLOT", 3, "Transaction Log");
                scaleform.CallFunction("DISPLAY_BALANCE", new object[]{
                    "FatUnicornzz", "Account balance ", 400
                });
                scaleform.CallFunction("DISPLAY_MENU");
            } else if(currentScreen == 0 && screen == 1)
            {
                //Withdrawal screen
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
                scaleform.CallFunction("DISPLAY_BALANCE", new object[]{
                    "FatUnicornzz", "Account balance ", 400
                });
                scaleform.CallFunction("DISPLAY_CASH_OPTIONS");
            }
        }

        private async Task LoadScaleform(string id)
        {
            Scaleform scaleform = new Scaleform(id);
            while (!scaleform.IsLoaded)
            {
                RequestScaleformMovieInteractive(id);
                await Delay(0);
            }
            this.scaleform = scaleform;
        }
    }
}
