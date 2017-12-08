using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace MonikaBot
{
    class GuildMonitorService
    {
        private readonly DiscordSocketClient _client;
        private readonly Random _random;

        public GuildMonitorService(DiscordSocketClient client, Random random)
        {
            _client = client;
            _random = random;
            AddSubscriptions();
        }

        private void AddSubscriptions()
        {
            _client.ChannelCreated += OnChannelCreatedAsync;
            _client.ChannelDestroyed += OnChannelDestroyedAsync;
            _client.JoinedGuild += OnJoinedGuildAsync;

            _client.MessageDeleted += OnMessageDeletedAsync;
            _client.UserBanned += OnUserBannedAsync;
            _client.UserJoined += OnUserJoinedAsync;
            _client.UserLeft += OnUserLeftAsync;
            _client.UserUnbanned += OnUserUnbannedAsync;
        }

        private Task OnUserUnbannedAsync(SocketUser arg1, SocketGuild arg2)
        {
            throw new NotImplementedException();
        }

        private Task OnUserLeftAsync(SocketGuildUser arg)
        {
            throw new NotImplementedException();
        }

        private Task OnUserJoinedAsync(SocketGuildUser arg)
        {
            throw new NotImplementedException();
        }

        private Task OnUserBannedAsync(SocketUser arg1, SocketGuild arg2)
        {
            throw new NotImplementedException();
        }

        private Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            throw new NotImplementedException();
        }

        private async Task OnJoinedGuildAsync(SocketGuild guild)
        {
            var name = guild.Name;
            var owner = guild.Owner;
            var defaultChannel = guild.DefaultChannel;

            await defaultChannel.SendMessageAsync(GenerateReply(name, owner));
        }

        private string GenerateReply(string guildName, SocketGuildUser guildOwner)
        {
            string reply = null;
            var status = guildOwner.Status;
            switch (status)
            {
                case UserStatus.Online:
                    reply = "I feel honored joining " + guildName + ", a pleasure to meet you " + guildOwner + 
                        ", just don't forget that you're my bitch now ^.^/";
                    break;
                case UserStatus.Offline:
                    reply = "Hey, can someone tell " + guildOwner + " about my arrival when he gets online? Thaaank you.";
                    break;
                case UserStatus.Invisible:
                    reply = "Thanks for letting me in! " + " And hey " + guildOwner + ", can't hide from me, I know you're lurking in invis mode ;)";
                    break;
                case UserStatus.AFK:
                    reply = "Just me, tuning in, heya everyone.." + " I see the owner," + guildOwner + ", is AFK so, tell him I joined.";
                    break;
                default:
                    reply = "... some wierd exception";
                    break;
            }
            return $"[{guildName}]"+reply;
        }

        private async Task OnChannelDestroyedAsync(SocketChannel channel)
        {
            var unboxed = channel as SocketGuildChannel;
            if (unboxed == null) return;
            var defaultChannel = unboxed.Guild.DefaultChannel;
            await defaultChannel.SendMessageAsync("Ahaha~ seems like channel \"" + unboxed.Name + "\" just got DELETED :smirk:");
        }

        private async Task OnChannelCreatedAsync(SocketChannel channel)
        {
            var unboxed = channel as SocketGuildChannel;
            if (unboxed == null) return;
            var defaultChannel = unboxed.Guild.DefaultChannel;
            await defaultChannel.SendMessageAsync("Lol, that's a really uncreative name, \"" + unboxed.Name + "\", I mean come on..");
        }
    }
}
