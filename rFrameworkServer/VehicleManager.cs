using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using static rFrameworkServer.Functions;
using static rFrameworkServer.ConfigManager;
using System.IO;

namespace rFrameworkServer
{
    class VehicleManager : BaseScript
    {

        public static List<rDealership> Dealerships;
        public static Dictionary<int, rVehicle> Vehicles;

        public VehicleManager()
        {
            EventHandlers.Add("rFramework:PurchaseVehicle", new Action<Player, int>(PurchaseVehicle));
            EventHandlers.Add("rFramework:UpdateVehicle", new Action<Player, string>(UpdateVehicle));

            Dealerships = JsonConvert.DeserializeObject<List<rDealership>>(config["Dealerships"].ToString());
            Vehicles = JsonConvert.DeserializeObject<Dictionary<int, rVehicle>>(config["Vehicles"].ToString());
        }

        private static void writejson([FromSource] Player p, string json)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"resources\rFramework\VehicleJson.txt");
            File.WriteAllText(path, json);
        }

        public static void PurchaseVehicle([FromSource] Player player, int VehicleNetworkID)
        {
            rFrameworkPlayer rPlayer = PlayerManager.GetrFrameworkPlayer(player);
            Vehicle VehToBuy = new Vehicle(NetworkGetEntityFromNetworkId(VehicleNetworkID));
            
            int VehicleHash = VehToBuy.Model;
            if(Vehicles.TryGetValue(VehicleHash, out _))
            {
                int VehiclePrice = Vehicles[VehicleHash].price;
                if (rPlayer.BankBalance >= VehiclePrice)
                {
                    PlayerManager.ChangePlayerMoney(rPlayer, -VehiclePrice, 0);
                    PlayerManager.UpdatePlayerCash(rPlayer);
                    string VehicleGuid = Guid.NewGuid().ToString();
                    Dictionary<string, string> OwnedVehicleList;
                    if (rPlayer.Vehicles != "")
                    {
                        OwnedVehicleList = JsonConvert.DeserializeObject<Dictionary<string, string>>(rPlayer.Vehicles);
                    } else
                    {
                        OwnedVehicleList = new Dictionary<string, string>();
                    }
                    OwnedVehicleList.Add(VehicleGuid, "");
                    rPlayer.Vehicles = JsonConvert.SerializeObject(OwnedVehicleList);
                }
            } else
            {
                DebugWrite("Can't Purchase This Car");
            }
        }

        public static void UpdateVehicle([FromSource] Player player, string VehicleInfoJSON)
        {
            DebugWrite(VehicleInfoJSON);
            Vector3 PC = GetEntityCoords(player.Character.Handle);
            List<int> Mods = JsonConvert.DeserializeObject<List<int>>(VehicleInfoJSON);
            CreateVehicle((uint)GetHashKey("dubsta3"), PC.X, PC.Y, PC.Z + 5, GetEntityHeading(player.Character.Handle), true, true);
        }

        public static void GetPlayerVehicles()
        {

        }

        public static void UpdatePlayerVehicles(rFrameworkPlayer rPlayer)
        {

        }
    }
}
