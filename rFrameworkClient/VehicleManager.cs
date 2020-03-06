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

        public static List<string> BonesToCheck = new List<string>()
        {
            "chassis",
            "chassis_lowlod",
            "chassis_dummy",
            "seat_dside_f",
            "seat_dside_r",
            "seat_dside_r1",
            "seat_dside_r2",
            "seat_dside_r3",
            "seat_dside_r4",
            "seat_dside_r5",
            "seat_dside_r6",
            "seat_dside_r7",
            "seat_pside_f",
            "seat_pside_r",
            "seat_pside_r1",
            "seat_pside_r2",
            "seat_pside_r3",
            "seat_pside_r4",
            "seat_pside_r5",
            "seat_pside_r6",
            "seat_pside_r7",
            "window_lf1",
            "window_lf2",
            "window_lf3",
            "window_rf1",
            "window_rf2",
            "window_rf3",
            "window_lr1",
            "window_lr2",
            "window_lr3",
            "window_rr1",
            "window_rr2",
            "window_rr3",
            "door_dside_f",
            "door_dside_r",
            "door_pside_f",
            "door_pside_r",
            "handle_dside_f",
            "handle_dside_r",
            "handle_pside_f",
            "handle_pside_r",
            "wheel_lf",
            "wheel_rf",
            "wheel_lm1",
            "wheel_rm1",
            "wheel_lm2",
            "wheel_rm2",
            "wheel_lm3",
            "wheel_rm3",
            "wheel_lr",
            "wheel_rr",
            "suspension_lf",
            "suspension_rf",
            "suspension_lm",
            "suspension_rm",
            "suspension_lr",
            "suspension_rr",
            "spring_rf",
            "spring_lf",
            "spring_rr",
            "spring_lr",
            "transmission_f",
            "transmission_m",
            "transmission_r",
            "hub_lf",
            "hub_rf",
            "hub_lm1",
            "hub_rm1",
            "hub_lm2",
            "hub_rm2",
            "hub_lm3",
            "hub_rm3",
            "hub_lr",
            "hub_rr",
            "windscreen",
            "windscreen_r",
            "window_lf",
            "window_rf",
            "window_lr",
            "window_rr",
            "window_lm",
            "window_rm",
            "bodyshell",
            "bumper_f",
            "bumper_r",
            "wing_rf",
            "wing_lf",
            "bonnet",
            "boot",
            "exhaust",
            "exhaust_2",
            "exhaust_3",
            "exhaust_4",
            "exhaust_5",
            "exhaust_6",
            "exhaust_7",
            "exhaust_8",
            "exhaust_9",
            "exhaust_10",
            "exhaust_11",
            "exhaust_12",
            "exhaust_13",
            "exhaust_14",
            "exhaust_15",
            "exhaust_16",
            "engine",
            "overheat",
            "overheat_2",
            //"petrolcap",
            "petroltank",
            "petroltank_l",
            "petroltank_r",
            "steering",
            "hbgrip_l",
            "hbgrip_r",
            "headlight_l",
            "headlight_r",
            "taillight_l",
            "taillight_r",
            "indicator_lf",
            "indicator_rf",
            "indicator_lr",
            "indicator_rr",
            "brakelight_l",
            "brakelight_r",
            "brakelight_m",
            "reversinglight_l",
            "reversinglight_r",
            "extralight_1",
            "extralight_2",
            "extralight_3",
            "extralight_4",
            "numberplate",
            "interiorlight",
            "siren1",
            "siren2",
            "siren3",
            "siren4",
            "siren5",
            "siren6",
            "siren7",
            "siren8",
            "siren9",
            "siren10",
            "siren11",
            "siren12",
            "siren13",
            "siren14",
            "siren15",
            "siren16",
            "siren17",
            "siren18",
            "siren19",
            "siren20",
            "siren_glass1",
            "siren_glass2",
            "siren_glass3",
            "siren_glass4",
            "siren_glass5",
            "siren_glass6",
            "siren_glass7",
            "siren_glass8",
            "siren_glass9",
            "siren_glass10",
            "siren_glass11",
            "siren_glass12",
            "siren_glass13",
            "siren_glass14",
            "siren_glass15",
            "siren_glass16",
            "siren_glass17",
            "siren_glass18",
            "siren_glass19",
            "siren_glass20",
            "spoiler",
            "struts",
            "misc_a",
            "misc_b",
            "misc_c",
            "misc_d",
            "misc_e",
            "misc_f",
            "misc_g",
            "misc_h",
            "misc_i",
            "misc_j",
            "misc_k",
            "misc_l",
            "misc_m",
            "misc_n",
            "misc_o",
            "misc_p",
            "misc_q",
            "misc_r",
            "misc_s",
            "misc_t",
            "misc_u",
            "misc_v",
            "misc_w",
            "misc_x",
            "misc_y",
            "misc_z",
            "misc_1",
            "misc_2",
            "weapon_1a",
            "weapon_1b",
            "weapon_1c",
            "weapon_1d",
            "weapon_1a_rot",
            "weapon_1b_rot",
            "weapon_1c_rot",
            "weapon_1d_rot",
            "weapon_2a",
            "weapon_2b",
            "weapon_2c",
            "weapon_2d",
            "weapon_2a_rot",
            "weapon_2b_rot",
            "weapon_2c_rot",
            "weapon_2d_rot",
            "weapon_3a",
            "weapon_3b",
            "weapon_3c",
            "weapon_3d",
            "weapon_3a_rot",
            "weapon_3b_rot",
            "weapon_3c_rot",
            "weapon_3d_rot",
            "weapon_4a",
            "weapon_4b",
            "weapon_4c",
            "weapon_4d",
            "weapon_4a_rot",
            "weapon_4b_rot",
            "weapon_4c_rot",
            "weapon_4d_rot",
            "turret_1base",
            "turret_1barrel",
            "turret_2base",
            "turret_2barrel",
            "turret_3base",
            "turret_3barrel",
            "ammobelt",
            "searchlight_base",
            "searchlight_light",
            "attach_female",
            "roof",
            "roof2",
            "soft_1",
            "soft_2",
            "soft_3",
            "soft_4",
            "soft_5",
            "soft_6",
            "soft_7",
            "soft_8",
            "soft_9",
            "soft_10",
            "soft_11",
            "soft_12",
            "soft_13",
            "forks",
            "mast",
            "carriage",
            "fork_l",
            "fork_r",
            "forks_attach",
            "frame_1",
            "frame_2",
            "frame_3",
            "frame_pickup_1",
            "frame_pickup_2",
            "frame_pickup_3",
            "frame_pickup_4",
            "freight_cont",
            "freight_bogey",
            "freightgrain_slidedoor",
            "door_hatch_r",
            "door_hatch_l",
            "tow_arm",
            "tow_mount_a",
            "tow_mount_b",
            "tipper",
            "combine_reel",
            "combine_auger",
            "slipstream_l",
            "slipstream_r",
            "arm_1",
            "arm_2",
            "arm_3",
            "arm_4",
            "scoop",
            "boom",
            "stick",
            "bucket",
            "shovel_2",
            "shovel_3",
            "Lookat_UpprPiston_head",
            "Lookat_LowrPiston_boom",
            "Boom_Driver",
            "cutter_driver",
            "vehicle_blocker",
            "extra_1",
            "extra_2",
            "extra_3",
            "extra_4",
            "extra_5",
            "extra_6",
            "extra_7",
            "extra_8",
            "extra_9",
            "extra_ten",
            "extra_11",
            "extra_12",
            "break_extra_1",
            "break_extra_2",
            "break_extra_3",
            "break_extra_4",
            "break_extra_5",
            "break_extra_6",
            "break_extra_7",
            "break_extra_8",
            "break_extra_9",
            "break_extra_10",
            "mod_col_1",
            "mod_col_2",
            "mod_col_3",
            "mod_col_4",
            "mod_col_5",
            "handlebars",
            "forks_u",
            "forks_l",
            "wheel_f",
            "swingarm",
            "wheel_r",
            "crank",
            "pedal_r",
            "pedal_l",
            "static_prop",
            "moving_prop",
            "static_prop2",
            "moving_prop2",
            "rudder",
            "rudder2",
            "wheel_rf1_dummy",
            "wheel_rf2_dummy",
            "wheel_rf3_dummy",
            "wheel_rb1_dummy",
            "wheel_rb2_dummy",
            "wheel_rb3_dummy",
            "wheel_lf1_dummy",
            "wheel_lf2_dummy",
            "wheel_lf3_dummy",
            "wheel_lb1_dummy",
            "wheel_lb2_dummy",
            "wheel_lb3_dummy",
            "bogie_front",
            "bogie_rear",
            "rotor_main",
            "rotor_rear",
            "rotor_main_2",
            "rotor_rear_2",
            "elevators",
            "tail",
            "outriggers_l",
            "outriggers_r",
            "rope_attach_a",
            "rope_attach_b",
            "prop_1",
            "prop_2",
            "elevator_l",
            "elevator_r",
            "rudder_l",
            "rudder_r",
            "prop_3",
            "prop_4",
            "prop_5",
            "prop_6",
            "prop_7",
            "prop_8",
            "rudder_2",
            "aileron_l",
            "aileron_r",
            "airbrake_l",
            "airbrake_r",
            "wing_l",
            "wing_r",
            "wing_lr",
            "wing_rr",
            "engine_l",
            "engine_r",
            "nozzles_f",
            "nozzles_r",
            "afterburner",
            "wingtip_1",
            "wingtip_2",
            "gear_door_fl",
            "gear_door_fr",
            "gear_door_rl1",
            "gear_door_rr1",
            "gear_door_rl2",
            "gear_door_rr2",
            "gear_door_rml",
            "gear_door_rmr",
            "gear_f",
            "gear_rl",
            "gear_lm1",
            "gear_rr",
            "gear_rm1",
            "gear_rm",
            "prop_left",
            "prop_right",
            "legs",
            "attach_male",
            "draft_animal_attach_lr",
            "draft_animal_attach_rr",
            "draft_animal_attach_lm",
            "draft_animal_attach_rm",
            "draft_animal_attach_lf",
            "draft_animal_attach_rf",
            "wheelcover_l",
            "wheelcover_r",
            "barracks",
            "pontoon_l",
            "pontoon_r",
            "no_ped_col_step_l",
            "no_ped_col_strut_1_l",
            "no_ped_col_strut_2_l",
            "no_ped_col_step_r",
            "no_ped_col_strut_1_r",
            "no_ped_col_strut_2_r",
            "light_cover",
            "emissives",
            "neon_l",
            "neon_r",
            "neon_f",
            "neon_b",
            "dashglow",
            "doorlight_lf",
            "doorlight_rf",
            "doorlight_lr",
            "doorlight_rr",
            "unknown_id",
            "dials",
            "engineblock",
            "bobble_head",
            "bobble_base",
            "bobble_h",
            "chassis_Control"
        };

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
