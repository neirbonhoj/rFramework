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
    class VehicleManager : BaseScript
    {
        public static List<rDealership> Dealerships;
        public static Dictionary<string, rVehicle> Vehicles;

        public VehicleManager()
        {

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

            RegisterCommand("unlockdoor", new Action<int, List<object>, string>((source, args, raw) =>
            {
                //DoorSystem research - conclusion? almost none of it works in FiveM

                //int door = GetClosestObjectOfType(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, 100.0f, 746855201, false, false, false);
                //AddDoorToSystem(746855201, (uint)GetHashKey("v_ilev_bk_gate2"), 262.1981f, 222.5188f, 106.4296f, false, false, false);
                //DoorControl(746855201, 262.1981f, 222.5188f, 106.4296f, true, 0.0f, 50.0f, 0f);
                //SetStateOfClosestDoorOfType(746855201, 262.1981f, 222.5188f, 106.4296f, true, 0, false);
                //DebugWrite("added door");
                int refdoor = 0;
                //DebugWrite(DoorSystemFindExistingDoor(262.1981f, 222.5188f, 106.4296f, GetHashKey("v_ilev_bk_gate2"), ref refdoor));
                uint door = (uint)refdoor;
                DebugWrite("Have Control: " + NetworkRequestControlOfDoor(746855201));
                DebugWrite("Network Control: " + NetworkHasControlOfDoor(746855201));
                DebugWrite("IsDoorRegisteredWithSystem: " + IsDoorRegisteredWithSystem(1206354175));
                RemoveDoorFromSystem(door);
                DebugWrite("IsDoorRegisteredWithSystem: " + IsDoorRegisteredWithSystem(door));
                //DoorControl(746855201, 262.1981f, 222.5188f, 106.4296f, true, 0.0f, 50.0f, 0f);
                SetStateOfClosestDoorOfType(door, 262.1981f, 222.5188f, 106.4296f, true, 0, false);
                DebugWrite(door);
                DebugWrite("DoorSystemGetDoorPendingState: " + DoorSystemGetDoorPendingState((uint)door));
                DebugWrite("DoorSystemGetDoorState: " + DoorSystemGetDoorState((uint)door));
                DoorSystemSetDoorState((uint)door, 1, false, true);
                DebugWrite("DoorSystemGetDoorState: " + DoorSystemGetDoorState((uint)door));
                DoorSystemSetHoldOpen((uint)door, true);
                DebugWrite("DoorSystemGetOpenRatio: " + DoorSystemGetOpenRatio((uint)door));
                DoorSystemSetOpenRatio(746855201, -1.0f, false, true);
                //AddDoorToSystem(746855201, (uint)GetHashKey("v_ilev_bk_gate2"), 262.1981f, 222.5188f, 106.4296f, false, false, false);
                //DoorSystemSetAutomaticDistance(746855201, 0, false, false);
            }), false);
        } 
    }
}
