using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using static rConfig.ConfigManager;
namespace rDiscordBot
{
    class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private SocketGuild _guild;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            //Hard coded token... oops!
            var token = "MzQ0NjY3MDg1NjU1NTA2OTQ0.XeyA7g.ouNWgJpsmwp8dyZrwqvLpp0h6mg";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += () =>
            {
                ulong GuildID = 649489673869066263;
                InitializeGuild(GuildID);

                InitializePermissions();

                Permissions.WriteAllPlayerPermissions();

                return Task.CompletedTask;
            };

            _client.GuildMemberUpdated += (oldMember, newMember) =>
            {
                if (oldMember.Roles.Count != newMember.Roles.Count)
                {
                    UpdateUserPermissions(newMember);
                }
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }

        private void InitializeGuild(ulong GuildID)
        {
            _guild = _client.GetGuild(GuildID);
        }

        private async void InitializePermissions()
        {
            await _guild.DownloadUsersAsync();
            foreach (SocketGuildUser user in _guild.Users)
            {
                if (user.Id != _client.CurrentUser.Id)
                {
                    foreach (SocketRole role in user.Roles)
                    {
                        Permissions.AddPlayerRole(user.Id, role.Id);
                    }
                }
            }
        }

        private void UpdateUserPermissions(SocketGuildUser user)
        {
            if (user.Id != _client.CurrentUser.Id)
            {
                List<ulong> RoleIDs = new List<ulong>();
                foreach (SocketRole role in user.Roles)
                {
                    RoleIDs.Add(role.Id);
                }
                Permissions.SetPlayerRoles(user.Id, RoleIDs);
                Permissions.WritePlayerPermissions(user.Id);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
