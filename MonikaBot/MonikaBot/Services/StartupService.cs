﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MonikaBot
{
    public class StartupService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        
        public StartupService(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            string tokenFile = ConfigurationManager.AppSettings["TokenFile"];
            string TOKEN = ReadToken(tokenFile);
            if (string.IsNullOrWhiteSpace(TOKEN))
                throw new Exception("Discord bot token not found in TokenFile");

            await _client.LoginAsync(TokenType.Bot, TOKEN);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly()); //?

            _client.Ready += async () =>
            {
                Console.WriteLine($"{DateTime.Now,-19} [Monika - pre-alpha v0.0.1] I'm up and running~");
                await _client.SetGameAsync("Doki Doki Litterature Club!");
                foreach (var guild in _client.Guilds)
                {
                    if(guild.TextChannels.FirstOrDefault() != null)
                    {
                        await guild.TextChannels.FirstOrDefault().SendMessageAsync("[Monika - pre-alpha v0.0.1] I'm up and running~");
                        //await guild.DefaultChannel.SendMessageAsync("[Monika - pre-alpha v0.0.1] I'm up and running~"); depricated
                    }
                }
            };
        }

        private string ReadToken(string tokenFile)
        {
            var lines = File.ReadLines(tokenFile);
            foreach (var line in lines)
            {
                return line;
            }

            return null;

        }
    }
}
