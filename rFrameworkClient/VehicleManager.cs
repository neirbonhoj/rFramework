using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.Functions;

namespace rFrameworkClient
{
    class VehicleManager : BaseScript
    {
        //None of the collision stuff works
        public static List<rDealership> Dealerships;
        public static Dictionary<string, rVehicle> Vehicles;

        public VehicleManager()
        {
            //Tick += DrawRelativeVehiclePoint;

            RegisterCommand("getvehicle", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));

                //Prints all possible mods
                SetVehicleModKit(Veh.Handle, 0);
                foreach (VehicleMod m in Veh.Mods.GetAllMods())
                {
                    DebugWrite(m.LocalizedModTypeName);
                }
            }), false);

           
    }
}
