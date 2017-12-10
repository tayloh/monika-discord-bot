using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonikaBot
{
    public class TimedMessagingService
    {
        private readonly DiscordSocketClient _client;
        private readonly Random _random;
        private delegate Task SendMessageAction();

        public TimedMessagingService(GuildUserControl guildUsers, Random random, DiscordSocketClient client)
        {
            _client = client;
            _random = random;
            _client.Ready += client_ReadyAsync;
        }


        private async Task client_ReadyAsync()
        {
            await ScheduleAction(SendPrivateMessage, new TimeSpan(1, 0, 0)).ConfigureAwait(false);
            await ScheduleAction(SendMessageToGuildDefaultChannel, new TimeSpan(1, 0, 0, 0)).ConfigureAwait(false);
        }


        private Task ScheduleAction(SendMessageAction sendMessageAction, TimeSpan interval)
        {
            Task.Run(async() => {
                while (true)
                {
                    await Task.Delay(interval);
                    await sendMessageAction(); 
                }
            });
            return Task.CompletedTask;
        }

        private async Task SendPrivateMessage()
        {
            foreach (var guild in _client.Guilds)
            {
                foreach (var guildUser in guild.Users)
                {
                    if (guildUser.Status != UserStatus.Offline && !guildUser.IsBot)
                    {
                        var DMchannel = await guildUser.GetOrCreateDMChannelAsync();
                        await DMchannel.SendMessageAsync("Private message.");
                    }
                }
            }
        }
        
        private async Task SendMessageToGuildDefaultChannel()
        {
            foreach (var guild in _client.Guilds)
            {
                await guild.DefaultChannel.SendMessageAsync("Message to default channel.");
            }
        }
    }
}
