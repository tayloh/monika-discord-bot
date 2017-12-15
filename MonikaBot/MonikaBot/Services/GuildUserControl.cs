using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonikaBot
{
    public class GuildUserControl
    {
        private readonly DiscordSocketClient _client;
        private IReadOnlyCollection<SocketGuild> _guilds;
        public Dictionary<SocketGuild, IReadOnlyCollection<SocketGuildUser>> GuildAndUsersDictionary { get; private set; } = new Dictionary<SocketGuild, IReadOnlyCollection<SocketGuildUser>>();
        
        public GuildUserControl(DiscordSocketClient client)
        {
            _client = client;
            _client.Ready += client_Ready;
            _client.JoinedGuild += client_JoinedGuild;
        }

        private Task client_JoinedGuild(SocketGuild socketGuild)
        {
           return UpdateGuilds();
        }

        private Task client_Ready()
        {
            return UpdateGuilds();
        }

        private Task UpdateGuilds()
        {
            GuildAndUsersDictionary.Clear();
            _guilds = _client.Guilds;

            foreach (var guild in _guilds)
            {
                try
                {
                    GuildAndUsersDictionary.Add(guild, guild.Users);

                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Failed to update GuildAndUsersDictionary");
                }
            }
            return Task.CompletedTask;
        }

    }
}
