using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static rFrameworkClient.Functions;

namespace rFrameworkClient
{
    class ArmoredTruck
    {

        public ArmoredTruck()
        {

            //EventHandlers.Add("rFramework:StartArmoredTruck", new Action<Vector3, float, List<dynamic>, float>(StartArmoredTruck));

            //RegisterCommand("truck", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    TriggerServerEvent("rFramework:StartTruck");
            //}), false);
        }

        //private async void StartArmoredTruck(Vector3 Location, float Heading, List<dynamic> Nodes, float ParkHeading)
        //{
        //    DebugWrite("Spawning an armored truck - DEBUG/TESTING - "+Location);
        //    DebugWrite(Nodes);

        //    ProfilerEnterScope("1");
        //    //Create truck
        //    await LoadModel(GetHashKey("stockade"));
        //    ClearAreaOfVehicles(Location.X, Location.Y, Location.Z, 10, false, false, false, false, false);
        //    int Truck = CreateVehicle((uint)GetHashKey("stockade"), Location.X, Location.Y, Location.Z, Heading, true, true);
        //    SetEntityLoadCollisionFlag(Truck, true);

        //    AddBlipForEntity(Truck);
        //    SetPedIntoVehicle(Game.PlayerPed.Handle, Truck, 0);
        //    ProfilerExitScope();


        //    ProfilerEnterScope("2");
        //    //Create driver
        //    await LoadModel(1669696074);
        //    int Driver = CreatePedInsideVehicle(Truck, 6, 1669696074, -1, true, true);
        //    SetDriverAbility(Driver, 100f);
        //    ProfilerExitScope();

        //    ProfilerEnterScope("3");
        //    for (int i = 0; i < Nodes.Count() - 1; i++)
        //    {
        //        ProfilerEnterScope("3.1");
        //        Vector3 Node = Nodes[i];
        //        TaskVehicleDriveToCoordLongrange(Driver, Truck, Node.X, Node.Y, Node.Z, 15f, 1074528293, 5f);
        //        ProfilerExitScope();
        //        await Delay(500);
        //        while (GetScriptTaskStatus(Driver, 0x21d33957) == 1)
        //        {
        //            await Delay(500);
        //        }
        //    }
        //    ProfilerExitScope();

        //    Vector3 ParkNode = Nodes[Nodes.Count() - 1];


        //    ProfilerEnterScope("4");
        //    float NodeDistance = GetDistanceBetweenCoords(Nodes[Nodes.Count() - 2].X, Nodes[Nodes.Count() - 2].Y, Nodes[Nodes.Count() - 2].Z,
        //        ParkNode.X, ParkNode.Y, ParkNode.Z, false);

        //    if(NodeDistance > 20f)
        //    {
        //        DebugWrite("We need to split the raycasting");
        //    }
        //    ProfilerExitScope();

        //    //int Raycast = StartShapeTestCapsule(Nodes[Nodes.Count() - 2].X, Nodes[Nodes.Count() - 2].Y, Nodes[Nodes.Count() - 2].Z,
        //    //    ParkNode.X, ParkNode.Y, ParkNode.Z, 2, 2, Truck, 7);

        //    //bool RaycastHit = false;
        //    //Vector3 RaycastEndCoords = new Vector3();
        //    //Vector3 RaycastSurfaceNormal = new Vector3();
        //    //int RaycastEntityHit = 0;

        //    //GetShapeTestResult(Raycast, ref RaycastHit, ref RaycastEndCoords, ref RaycastSurfaceNormal, ref RaycastEntityHit);

        //    //DebugWrite(RaycastHit);
        //    //DebugWrite(RaycastEndCoords);
        //    //DebugWrite(RaycastSurfaceNormal);
        //    //DebugWrite(RaycastEntityHit);

        //    //DebugWrite("Done!");
        //    //TaskVehiclePark(Driver, Truck, RaycastEndCoords.X, RaycastEndCoords.Y, RaycastEndCoords.Z, ParkHeading, 1, 20f, false);
        //    //await Delay(500);
        //    //while (GetScriptTaskStatus(Driver, 0xefc8537e) == 1)
        //    //{
        //    //    await Delay(500);
        //    //}

        //    //DebugWrite("Done Parking!");
        //}

        //public static async Task LoadModel(int model)
        //{
        //    while (!HasModelLoaded((uint)model))
        //    {
        //        RequestModel((uint)model);
        //        await Delay(0);
        //    }

        //    return;
        //}
    }
}
