using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MonikaBot
{
    public class Program
    {

        private DiscordSocketClient _client;
        private CommandService _commands;
        private GuildUserControl _guildUsers;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult(); //starta MainAsync (som aldrig blir klar)


        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 200
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = true,
                LogLevel = LogSeverity.Info
            });

            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<StartupService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Random>()
                .AddSingleton<GuildMonitorService>()
                .AddSingleton<GuildUserControl>()
                .AddSingleton<TimedMessagingService>();

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<GuildMonitorService>();
            provider.GetRequiredService<GuildUserControl>();
            provider.GetRequiredService<TimedMessagingService>();

            await Task.Delay(-1);
        }


    }
}