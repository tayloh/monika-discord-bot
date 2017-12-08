using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace MonikaBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider provider)
        {
            _client = client;
            _commands = commands;
            _provider = provider;

            _client.MessageReceived += OnMessageRecievedAsync;
        }

        private async Task OnMessageRecievedAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            if (message.Author.Username == _client.CurrentUser.Username) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('~', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            var context = new SocketCommandContext(_client, message); //?
            var result = await _commands.ExecuteAsync(context, argPos, _provider); //?

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync("Uhhm, " + result.ErrorReason);
        }
    }
}
