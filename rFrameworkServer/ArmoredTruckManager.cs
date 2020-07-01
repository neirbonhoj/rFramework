using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static rFrameworkServer.Functions;

namespace rFrameworkServer
{
    class ArmoredTruckManager : BaseScript
    {
        private Vector3 TruckSpawnLocation = new Vector3(-19.21f, -671.33f, 32.34f);
        private float TruckSpawnHeading = 183.2f;

        private List<Vector3> MidtownFleecaPickupLocation = new List<Vector3>{
            new Vector3(135.12f, -1023.71f, 29.23f),
            new Vector3(153.32f, -1030.68f, 29.19f)};
        private float MidtownFleecaParkHeading = 248.85f;

        private List<Vector3> UptownFleecaPickupLocation = new List<Vector3>{
            new Vector3(176.01f, -212.66f, 54.09f),
            new Vector3(282.56f, -252f, 53.98f),
            new Vector3(320.33f, -266.29f, 53.9f)};
        private float UptownFleecaParkHeading= 245.89f;

        private List<Vector3> GreatOceanPickupLocation = new List<Vector3>
        {
            new Vector3(-2976.99f, 455.72f, 15.14f),
            new Vector3(-2973.45f, 482.73f, 15.26f),
        };
        private float GreatOceanParkHeading = 354.95f;

        public ArmoredTruckManager()
        {
            EventHandlers.Add("rFramework:StartTruck", new Action<Player>(CreateArmoredTruckEvent));
        }

        private void CreateArmoredTruckEvent([FromSource] Player player)
        {
            DebugWrite("Creating armoed truck - request by " + player.Name);
            Player closestPlayer = null;
            float minDistance = -1;

            foreach (Player p in Players)
            {
                DebugWrite("Checking player " + p.Name + "'s distance to truck spawn");

                Vector3 difference = TruckSpawnLocation - GetEntityCoords(GetPlayerPed(p.Handle));
                float distance = difference.Length();

                DebugWrite("Distance: " + distance);

                if (distance < minDistance || minDistance < 0)
                {
                    minDistance = distance;
                    closestPlayer = p;
                }
            }

            if (closestPlayer==null)
            {
                DebugWrite("No players found - armored truck will not spawn");
                return;
            }

            DebugWrite("Closest player is " + closestPlayer.Name + "- spawning armored truck on their client");
            //TriggerClientEvent(closestPlayer, "rFramework:StartArmoredTruck", TruckSpawnLocation, TruckSpawnHeading, UptownFleecaPickupLocation, UptownFleecaParkHeading);
            TriggerClientEvent(closestPlayer, "rFramework:StartArmoredTruck", new Vector3(124.55f, -193.99f, 54.57f), 245.22f, UptownFleecaPickupLocation, UptownFleecaParkHeading);
        }
    }
}
