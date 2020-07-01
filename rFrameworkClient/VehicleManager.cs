using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.Functions;

namespace rFrameworkClient
{
    class VehicleManager
    {
        public static List<rDealership> Dealerships;
        public static Dictionary<string, rVehicle> Vehicles;

        public VehicleManager()
        {

            //RegisterCommand("getvehicle", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));

            //    //Prints all possible mods
            //    SetVehicleModKit(Veh.Handle, 0);
            //    foreach (VehicleMod m in Veh.Mods.GetAllMods())
            //    {
            //        DebugWrite(m.LocalizedModTypeName);
            //    }
            //}), false);

            //RegisterCommand("unlockdoor", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    //DoorSystem research - conclusion? almost none of it works in FiveM
            //}), false);
        } 
    }
}
