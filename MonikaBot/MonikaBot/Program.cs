using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MonikaBot
{
    class Program
    {

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;


        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult(); //starta MainAsync och vänta


        public async Task MainAsync()
        {
            string tokenFile = ConfigurationManager.AppSettings["TokenFile"];
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 20
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                LogLevel = LogSeverity.Info
            });

            _client.Log += Logger;
            _commands.Log += Logger;

            string token = ReadToken(tokenFile);

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += () =>
            {
                Console.WriteLine($"{DateTime.Now, -19} [Monika] I'm up and running~");
                return Task.CompletedTask;
            };
            

            await Task.Delay(-1);
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

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('~', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Logger(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now, -19} [{msg.Severity}] {msg.Source}: {msg.Message} {msg.Exception}");

            return Task.CompletedTask;
        }
    }
}