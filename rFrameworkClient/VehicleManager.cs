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

       
        private static List<Vector3> PointsToCheck = new List<Vector3>()
        {
            new Vector3(0,2.2f,0)
        };

        public static Dictionary<int, float> TestBoneIndexDeformation = new Dictionary<int, float>();
        public static Dictionary<Vector3, Vector3> PointDeformation = new Dictionary<Vector3, Vector3>();

        public static Vehicle VehicleOne;
        public static Vehicle VehicleTwo;

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

            //RegisterCommand("testdamage", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    SetVehicleColourCombination(Veh.Handle, 1);
            //    SetVehicleEngineHealth(Veh.Handle, 1000);
            //    float distY = 0;
            //    if (args.Count > 1)
            //    {
            //        distY = float.Parse(args[1].ToString());
            //    }
            //    float distX = 0;
            //    if (args.Count > 0)
            //    {
            //        distX = float.Parse(args[0].ToString());
            //    }
            //    SetVehicleDamage(Veh.Handle, distX, distY, 0, 1600, 1600, true);
            //    DebugWrite("Done");
            //}), false);

            //RegisterCommand("drawbones", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    Tick += DrawBones;
            //}), false);
            //RegisterCommand("undrawbones", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    Tick -= DrawBones;
            //}), false);

            //RegisterCommand("buycar", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    Vehicle VehicleToBuy = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    DebugWrite(NetworkDoesNetworkIdExist(VehicleToBuy.NetworkId));
            //    TriggerServerEvent("rFramework:PurchaseVehicle", VehicleToBuy.NetworkId);
            //}), false);

            //RegisterCommand("checkdamage2", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    TestBoneIndexDeformation = new Dictionary<int, float>();
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));

            //    Vector3 CarPos = GetEntityCoords(Veh.Handle, false);
            //    foreach (Vector3 Point in PointsToCheck)
            //    {
            //        Vector3 Offset = Point - CarPos;
            //        Vector3 Def = GetVehicleDeformationAtPos(Veh.Handle, Offset.X, Offset.Y, Offset.Z);
            //        DebugWrite(Def);
            //    }
            //}), false);

            //RegisterCommand("checkdamage", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    TestBoneIndexDeformation = new Dictionary<int, float>();
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));

            //    Vector3 CarPos = GetEntityCoords(Veh.Handle, false);
            //    foreach(string Bone in BonesToCheck)
            //    {
            //        int BoneIndex = GetEntityBoneIndexByName(Veh.Handle, Bone);
            //        if (BoneIndex > 0)
            //        {
            //            Vector3 BonePos = GetWorldPositionOfEntityBone(Veh.Handle, BoneIndex);
            //            Vector3 Offset = BonePos - CarPos;
            //            //Damage multiply 4000 and radius 500 yielded good results
            //            //25000, 20
            //            //13000, 850
            //            Vector3 Def = GetVehicleDeformationAtPos(Veh.Handle, Offset.X, Offset.Y, Offset.Z) * 100000;
            //            TestBoneIndexDeformation.Add(BoneIndex, Def.Length());
            //            DebugWrite(Bone+" - "+Def.Length());
            //        }
            //    }
            //}), false);

            //RegisterCommand("echeckdamage", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    PointDeformation = new Dictionary<Vector3, Vector3>();
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    VehicleOne = Veh;
            //    SetEntityCoords(Veh.Handle, 0, 0, 0, true, false, false, true);
            //    SetEntityHeading(Veh.Handle, 0);
            //    SetEntityCollision(Veh.Handle, false, false);
            //    FreezeEntityPosition(Veh.Handle, true);

            //    Vector3[] BoundingBox = GetEntityBoundingBox(Veh.Handle);
            //    float StartX = BoundingBox[0].X;
            //    float FinalX = BoundingBox[2].X;

            //    float StartY = BoundingBox[0].Y;
            //    float FinalY = BoundingBox[3].Y;

            //    float StartZ = BoundingBox[0].Z;
            //    float FinalZ = BoundingBox[4].Z;

            //    //too many points is bad!!
            //    float Detail = 3f;

            //    float xIncrement = (FinalX - StartX) / Detail;
            //    float yIncrement = (FinalY - StartY) / Detail;
            //    float zIncrement = (FinalZ - StartZ) / Detail;
            //    //DebugWrite(FinalX - StartX);
            //    //DebugWrite(FinalY - StartY);
            //    //DebugWrite(FinalZ - StartZ);
            //    //DebugWrite("^1--------------");
            //    //foreach(Vector3 v in BoundingBox)
            //    //{
            //    //    DebugWrite(v);
            //    //}
            //    for (float i = StartX; i <= FinalX; i += xIncrement)
            //    {
            //        for(float j = StartY; j <= FinalY + yIncrement/2; j += yIncrement / 2)
            //        {
            //            for (float k = StartZ + (StartZ - FinalZ)/2; k < FinalZ + (StartZ - FinalZ) / 2 + zIncrement; k += zIncrement)
            //            {
            //                Vector3 TestPoint = new Vector3();
            //                TestPoint.X = i;
            //                TestPoint.Y = j;
            //                TestPoint.Z = k;
            //                Vector3 l = GetVehicleDeformationAtPos(Veh.Handle, TestPoint.X, TestPoint.Y, TestPoint.Z);
            //                try
            //                {
            //                    PointDeformation.Add(TestPoint, l);
            //                } catch(Exception e)
            //                {
            //                    DebugWrite(e);
            //                }
            //            }
            //        }
            //    }
            //}), false);

            //RegisterCommand("applydamage", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    VehicleTwo = Veh;
            //    Vector3 CarPos = GetEntityCoords(Veh.Handle, false);
            //    foreach (int BoneIndex in TestBoneIndexDeformation.Keys)
            //    {
            //        Vector3 BonePos = GetWorldPositionOfEntityBone(Veh.Handle, BoneIndex);
            //        Vector3 Offset = BonePos - CarPos;

            //        SetVehicleDamage(Veh.Handle, Offset.X, Offset.Y, Offset.Z, TestBoneIndexDeformation[BoneIndex], 50, true);
            //        DebugWrite("done");
            //    }

            //    CopyVehicleDamages(VehicleOne.Handle, VehicleTwo.Handle);
            //    DebugWrite("Copied Vehicle Damage From " + VehicleOne.Handle + " To " + VehicleTwo.Handle);
            //}), false);

            //RegisterCommand("eapplydamage", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    VehicleTwo = Veh;
            //    Vector3 CarPos = GetEntityCoords(Veh.Handle, false);
            //    float radius = 1;
            //    float mult = 400;
            //    if (args.Count > 0)
            //    {
            //        mult = float.Parse(args[0].ToString());
            //    }
            //    foreach (Vector3 TestPoint in PointDeformation.Keys)
            //    {
            //        if (PointDeformation[TestPoint].Length() > 0)
            //        {
            //            //Try applying damage 150, radius 50 until results are roughly the same?
            //            //SetVehicleDamage(Veh.Handle, CarPos.X + TestPoint.X, CarPos.Y + TestPoint.Y, CarPos.Z + TestPoint.Z, (float)Math.Pow(10, PointDeformation[TestPoint].Length() * 100), 1600, true);
            //            float dmgValue = (float)Math.Pow(10, PointDeformation[TestPoint].Length() * 10);
            //            SetVehicleDamage(Veh.Handle, CarPos.X + TestPoint.X, CarPos.Y + TestPoint.Y, CarPos.Z + TestPoint.Z, dmgValue, 1600, true);
            //            DebugWrite(dmgValue);
            //            //SetVehicleDamage(Veh.Handle, CarPos.X + TestPoint.X, CarPos.Y + TestPoint.Y, CarPos.Z + TestPoint.Z, 1600, 1600, true);
            //        }
            //    }

            //    //CopyVehicleDamages(VehicleOne.Handle, VehicleTwo.Handle);
            //    //DebugWrite("Copied Vehicle Damage From " + VehicleOne.Handle + " To " + VehicleTwo.Handle);
            //}), false);

            //RegisterCommand("setone", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    VehicleOne = Veh;
            //}), false);

            //RegisterCommand("settwo", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    VehicleTwo = Veh;
            //}), false);

            //RegisterCommand("givecar", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Model VehicleModel = new Model(args[0].ToString());
            //    while (!VehicleModel.IsLoaded)
            //    {
            //        RequestModel((uint)VehicleModel.Hash);
            //        await Delay(50);
            //    }
            //    Vehicle VehicleToGive = new Vehicle(CreateVehicle((uint)VehicleModel.Hash, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, 0, true, false));
            //    SetPedIntoVehicle(Game.PlayerPed.Handle, VehicleToGive.Handle, -1);
            //}), false);

            //RegisterCommand("savecar", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    Dictionary<string, int> VehicleInfo = new Dictionary<string, int>();

            //    //Vehicle Color
            //    int VehPrimaryColor = 0;
            //    int VehSecondaryColor = 0;
            //    GetVehicleColours(Veh.Handle, ref VehPrimaryColor, ref VehSecondaryColor);
            //    VehicleInfo.Add("PrimaryColor", VehPrimaryColor);
            //    VehicleInfo.Add("SecondaryColor", VehSecondaryColor);

            //    //Vehicle Mods
            //    foreach(int Mod in (int[])Enum.GetValues(typeof(VehicleModType)))
            //    {
            //        DebugWrite(Mod);
            //    }

            //    //TriggerServerEvent("rFramework:UpdateVehicle", JsonConvert.SerializeObject(Mods));
            //}), false);

            //RegisterCommand("checkbones", new Action<int, List<object>, string>(async (source, args, raw) =>
            //{
            //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //    foreach (string Bone in BonesToCheck)
            //    {
            //        int BoneIndex = GetEntityBoneIndexByName(Veh.Handle, Bone);
            //        DebugWrite(Bone+": "+Veh.Bones.HasBone(Bone)+" - "+ GetWorldPositionOfEntityBone(Veh.Handle, BoneIndex));
            //    }
            //}), false);
        }

        //private async Task DrawRelativeVehiclePoint()
        //{
        //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
        //    Vector3 CarPos = GetEntityCoords(Veh.Handle, false);

        //    DrawBox(CarPos.X+-0.01f, CarPos.Y+-0.01f+2.2f, CarPos.Z+-0.01f, CarPos.X+0.01f, CarPos.Y+0.01f+2.2f, CarPos.Z+0.01f, 255, 255, 255, 255);
        //    await Task.FromResult(0);
        //}

        //private async Task DrawBones()
        //{
        //    Vehicle Veh = new Vehicle(GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
        //    Vector3 CarPos = GetEntityCoords(Veh.Handle, false);
        //    float max = 0;
        //    foreach(Vector3 Val in PointDeformation.Values)
        //    {
        //        if (Val.Length() > max)
        //        {
        //            max = Val.Length();
        //        }
        //    }

        //    float mult = 255 / max;
        //    float maxDistance = 0;

        //    foreach (Vector3 TestPoint in PointDeformation.Keys)
        //    {
        //        if (PointDeformation[TestPoint].Length() > 0)
        //        {
        //            float DistToCar = GetDistanceBetweenCoords(CarPos.X, CarPos.Y, CarPos.Z, TestPoint.X, TestPoint.Y, TestPoint.Z, true);
        //            if (DistToCar > maxDistance)
        //            {
        //                maxDistance = DistToCar;
        //            }
        //            Color color = Color.FromArgb((int)(PointDeformation[TestPoint].Length() * mult), 0, 0);
        //            Vector3 DeformationVector = PointDeformation[TestPoint];
        //            //DeformationVector has virtually 0 X component
        //            //DrawLine(TestPoint.X, TestPoint.Y, TestPoint.Z, TestPoint.X + DeformationVector.X, TestPoint.Y + DeformationVector.Y, TestPoint.Z + DeformationVector.Z, color.R, color.G, color.B, 255);
        //            DrawBox(CarPos.X + TestPoint.X - 0.01f, CarPos.Y + TestPoint.Y - 0.01f, CarPos.Z + TestPoint.Z - 0.01f, CarPos.X + TestPoint.X + 0.01f, CarPos.Y + TestPoint.Y + 0.01f, CarPos.Z + TestPoint.Z + 0.01f, color.R, color.G, color.B, 255);
        //        }
        //    }
        //    //DrawBox(- 0.01f,- 0.01f,- 0.01f,0.01f,0.01f, 0.01f, 255, 255, 255, 255);
        //    await Task.FromResult(0);
        //}

        //internal static Vector3[] GetEntityBoundingBox(int entity)
        //{
        //    Vector3 min = Vector3.Zero;
        //    Vector3 max = Vector3.Zero;

        //    GetModelDimensions((uint)GetEntityModel(entity), ref min, ref max);
        //    //const float pad = 0f;
        //    const float pad = 0.001f;
        //    var retval = new Vector3[8]
        //    {
        //        // Bottom
        //        GetOffsetFromEntityInWorldCoords(entity, min.X - pad, min.Y - pad, min.Z - pad),
        //        GetOffsetFromEntityInWorldCoords(entity, max.X + pad, min.Y - pad, min.Z - pad),
        //        GetOffsetFromEntityInWorldCoords(entity, max.X + pad, max.Y + pad, min.Z - pad),
        //        GetOffsetFromEntityInWorldCoords(entity, min.X - pad, max.Y + pad, min.Z - pad),

        //        // Top
        //        GetOffsetFromEntityInWorldCoords(entity, min.X - pad, min.Y - pad, max.Z + pad),
        //        GetOffsetFromEntityInWorldCoords(entity, max.X + pad, min.Y - pad, max.Z + pad),
        //        GetOffsetFromEntityInWorldCoords(entity, max.X + pad, max.Y + pad, max.Z + pad),
        //        GetOffsetFromEntityInWorldCoords(entity, min.X - pad, max.Y + pad, max.Z + pad)
        //    };
        //    return retval;
        //}
    }
}
