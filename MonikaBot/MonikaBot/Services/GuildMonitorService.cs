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

            _client.ChannelCreated += OnChannelCreatedAsync;
            _client.ChannelDestroyed += OnChannelDestroyedAsync;
            _client.JoinedGuild += OnJoinedGuildAsync;

            _client.MessageDeleted += OnMessageDeletedAsync;
            _client.UserBanned += OnUserBannedAsync;
            _client.UserJoined += OnUserJoinedAsync;
            _client.UserLeft += OnUserLeftAsync;
            _client.UserUnbanned += OnUserUnbannedAsync;
        }


        private async Task OnUserUnbannedAsync(SocketUser user, SocketGuild guild) //utkommenterat funkar inte
        {
            await guild.DefaultChannel.SendMessageAsync(user.Mention + " Seems like this guy got unbanned from the server, beware folks, once a criminal" +
                " always a criminal~ :grimacing:");
        }

        private async Task OnUserLeftAsync(SocketGuildUser guildUser)
        {
            //finns nåt sätt att komma åt användares DM efter de lämnat?
            string[] messages = new string[3];
            messages[0] = "BYE THEN.";
            messages[1] = "Happy thoughts? :hugging:";
            messages[2] = "We don't need him anyway :ok_hand:";
            await guildUser.Guild.DefaultChannel.SendMessageAsync(guildUser.Mention + " Left the server. " + messages[_random.Next(0, 2)]);

        }

        private async Task OnUserJoinedAsync(SocketGuildUser guildUser)
        {
            string[] messages = new string[5];
            messages[0] = "Welcome~ :hand_splayed:";
            messages[1] = "Everyone say helloooo!";
            messages[2] = "This guy has a really cool name, don't you think?";
            messages[3] = "Wooo new member.. :upside_down:";
            messages[4] = "Treat him/her well everybody~ cuz I won't :rage:";
            await guildUser.Guild.DefaultChannel.SendMessageAsync(guildUser.Mention + " " + messages[_random.Next(0, 4)]);
            await guildUser.SendMessageAsync("Enjoy your stay at " + guildUser.Guild.Name + ", I'm Monika, the best bot around (trust me)" +
                ", ever need anything, just ask me ok(~help)? I know it all. :ok_hand:");
        }

        private async Task OnUserBannedAsync(SocketUser user, SocketGuild guild)
        {
            string[] messages = new string[3];
            messages[0] = "just got DELETED :smirk:";
            messages[1] = "clearly deserved a ban :rage:";
            messages[2] = "Everybody panic! A ban streak is coming :grimacing: jk, btw " + user.Username + " just got banned.";
            await guild.DefaultChannel.SendMessageAsync(user.Mention + " " + messages[_random.Next(0, 2)]);
        }

        private async Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            await (channel as SocketGuildChannel).Guild.DefaultChannel.SendMessageAsync("Well, someone deleted their message.. and" +
                " I stored it (rekt), but my creator hasn't told me to do anyhting with it, yet. :thinking:");
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
                        ", just don't forget that you're my bitch now, ahahah ^.^/";
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

            string[] messages = new string[3];
            messages[0] = "Lol, that's a really uncreative name, \"" + unboxed.Name + "\", I mean come on..:neutral_face:";
            messages[1] = "New channel, I like it. :clap:";
            messages[2] = "Such a creative name, \"" + unboxed.Name + "\" :thumbsup:"; 
            await defaultChannel.SendMessageAsync(messages[_random.Next(0, 2)]);
        }
    }
}
