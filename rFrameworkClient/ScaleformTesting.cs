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
    class ScaleformTesting : BaseScript
    {
        private Scaleform scaleform;
        private int scaleformHandle;
        public ScaleformTesting()
        {
            RegisterCommand("ts", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string id = "ATM";
                TestScaleform(id);
            }), false);
        }

        private async void TestScaleform(string id)
        {
            int ScreenObject = GetHashKey("prop_fleeca_atm");
            float x = Game.PlayerPed.Position.X;
            float y = Game.PlayerPed.Position.Y;
            float z = Game.PlayerPed.Position.Z;
            Vector3 forward = GetEntityForwardVector(Game.PlayerPed.Handle);
            //CreateObject(ScreenObject, x + forward.X, y + forward.Y, z, true, true, false);
            scaleformHandle = CreateScaleformHandle("tvscreen", ScreenObject);

            scaleform = await LoadScaleform(id);
            scaleform.CallFunction("SHOW_CURSOR");
            scaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            scaleform.CallFunction("SET_DATA_SLOT", 0, "Choose a service.");
            scaleform.CallFunction("SET_DATA_SLOT", 1, "Withdraw");
            scaleform.CallFunction("SET_DATA_SLOT", 2, "Deposit");
            scaleform.CallFunction("SET_DATA_SLOT", 3, "Transaction Log");
            scaleform.CallFunction("DISPLAY_BALANCE", new object[]{
                "FatUnicornzz", "Account balance ", 400
            });
            scaleform.CallFunction("DISPLAY_MENU");

            //scaleform.CallFunction("updateBalance");
            //scaleform.CallFunction("UPDATE_TEXT");

            Tick += ScaleformTesting_Tick;
        }

        private async Task ScaleformTesting_Tick()
        {
            DisableAllControlActions(0);
            SetMouseCursorActiveThisFrame();
            scaleform.CallFunction("SET_MOUSE_INPUT", GetDisabledControlNormal(0, (int)Control.CursorX), GetDisabledControlNormal(0, (int)Control.CursorY));
            scaleform.Render2D();
            //SetTextRenderId(scaleformHandle);
            //Function.Call((Hash)0x40332D115A898AF5, scaleform.Handle, true);
            //SetUiLayer(4);
            //Function.Call((Hash)0xc6372ecd45d73bcd, scaleform.Handle, true);
            //DrawScaleformMovie(scaleform.Handle, 0.4f, 0.35f, 0.8f, 0.75f, 255, 255, 255, 255, 0);
            //SetTextRenderId(API.GetDefaultScriptRendertargetRenderId());
            //Function.Call((Hash)0x40332D115A898AF5, scaleform.Handle, false);
            return;
        }

        private async Task<Scaleform> LoadScaleform(string id)
        {
            Scaleform scaleform = new Scaleform(id);
            while (!scaleform.IsLoaded)
            {
                RequestScaleformMovieInteractive(id);
                await Delay(0);
            }
            DebugWrite("^1IsLoaded: " + scaleform.IsLoaded);
            return scaleform;
        }

        private int CreateScaleformHandle(string name, int model)
        {
            int handle = 0;
            if (!IsNamedRendertargetRegistered(name))
            {
                RegisterNamedRendertarget(name, false);
            }

            if (!IsNamedRendertargetLinked((uint)model))
            {
                LinkNamedRendertarget((uint)model);
            }

            if (IsNamedRendertargetRegistered(name))
            {
                handle = GetNamedRendertargetRenderId(name);
            }
            return handle;
        }
    }
}
