using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Discord.WebSocket;
namespace rDiscordBot
{
    class Permissions
    {
        //This dictionary will hold discord ID as the key and a list of roles stored as ulong (their ID)
        private static Dictionary<ulong, List<ulong>> UserPermissions = new Dictionary<ulong, List<ulong>>();

        public static void AddPlayerRole(ulong DiscordID, ulong role)
        {
            List<ulong> roles;
            if (UserPermissions.TryGetValue(DiscordID, out roles))
            {
                roles.Add(role);
            }
            else
            {
                UserPermissions[DiscordID] = new List<ulong>()
                {
                    role
                };
            }
        }

        public static void RemovePlayerRole(ulong DiscordID, ulong Role)
        {
            List<ulong> roles;
            if (UserPermissions.TryGetValue(DiscordID, out roles))
            {
                foreach (ulong role in roles)
                {
                    if (role.Equals(Role))
                    {
                        roles.Remove(Role);
                    }
                    break;
                }
            }
        }

        public static void SetPlayerRoles(ulong DiscordID, List<ulong> RoleIDs)
        {
            UserPermissions[DiscordID] = RoleIDs;
        }

        public static void WriteAllPlayerPermissions()
        {
            string permissionsJson = JsonConvert.SerializeObject(UserPermissions, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            string path = Path.Combine(Environment.CurrentDirectory, @"DiscordPermissionsFile.txt");
            File.WriteAllText(path, permissionsJson);
        }

        public static void WritePlayerPermissions(ulong PlayerDiscordID)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"DiscordPermissionsFile.txt");
            Dictionary<ulong, List<ulong>> ExistingPermissionsUpdate = JsonConvert.DeserializeObject<Dictionary<ulong, List<ulong>>>(File.ReadAllText(path));
            if (ExistingPermissionsUpdate.ContainsKey(PlayerDiscordID))
            {
                ExistingPermissionsUpdate[PlayerDiscordID] = UserPermissions[PlayerDiscordID];
            } else
            {
                ExistingPermissionsUpdate.Add(PlayerDiscordID, UserPermissions[PlayerDiscordID]);
            }

            string permissionsJson = JsonConvert.SerializeObject(ExistingPermissionsUpdate, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            File.WriteAllText(path, permissionsJson);
        }
    }
}
