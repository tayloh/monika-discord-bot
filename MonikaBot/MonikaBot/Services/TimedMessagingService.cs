using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using MonikaBot.Extensions;
using System.Linq;
using System.Collections.Generic;

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
            await SendMessageToGuildDefaultChannel();
            test();
            await ScheduleAction(SendPrivateMessage, new TimeSpan(1, 0, 0, 0)).ConfigureAwait(false);
            await ScheduleAction(SendMessageToGuildDefaultChannel, new TimeSpan(0, 30, 0)).ConfigureAwait(false);
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
            foreach (var guild in _client.Guilds)//om någon är med i två guilds, får vi bara skicka en gång..
            {
                foreach (var guildUser in guild.Users)
                {
                    if (guildUser.Status != UserStatus.Offline && !guildUser.IsBot)
                    {
                        var DMchannel = await guildUser.GetOrCreateDMChannelAsync();
                        using(XmlReader reader = XmlReader.Create("C:/Allrepos/monika-bot/monika-discord-bot/MonikaBot/MonikaBot/Misc/poems.xml"))
                        {
                            int numberOfStories = 312; //see poems.xml, hardcoded so that we don't need to count every tag.
                            var randomnr = _random.Next(numberOfStories);
                            var story = from s in reader.Stories()
                                        where s.Nr == randomnr
                                        select s;
                            string str = null;
                            for (var i = 0; i < 20; i++)
                            {
                                str += "**_____** ";
                            }
                            await DMchannel.SendMessageAsync(str);
                            try
                            {
                                var temp = story.First().Poem;
                                var discordMessageLengthLimit = 1994; //since we send `````` with every message
                                int.TryParse(DateTime.Now.ToString().Substring(0, 4), out int year);
                                int.TryParse(DateTime.Now.ToString().Substring(5, 2), out int month);
                                int.TryParse(DateTime.Now.ToString().Substring(8, 2), out int day);
                                await DMchannel.SendMessageAsync($"Today is {new DateTime(year, month, day).DayOfWeek.ToString().ToLower()}.");
                                await DMchannel.SendMessageAsync($"Here's your daily poem **{guildUser.Username}**! :hugging:");
                                await DMchannel.SendMessageAsync($"__{temp[0]}__ - by Me");
                                if (temp[1].Length > discordMessageLengthLimit) //discord max message size
                                {
                                    Console.WriteLine("Attempting to send parted message.");
                                    double num = Convert.ToDouble(temp[1].Length) / Convert.ToDouble(discordMessageLengthLimit);
                                    var numberOfParts = Convert.ToInt32(Math.Ceiling(num));
                                    var msg = String.Empty;
                                    for (var i = 0; i < numberOfParts; i++)
                                    {
                                        if(discordMessageLengthLimit * (i + 1) > temp[1].Length)
                                        {
                                            msg = temp[1].Substring(discordMessageLengthLimit * i);
                                        }
                                        else
                                        {
                                            msg = temp[1].Substring(discordMessageLengthLimit * i, discordMessageLengthLimit * (i + 1)); //0 to 2000, 2000 to 4000 etc..
                                        }
                                        await DMchannel.SendMessageAsync("```" + msg + "```");
                                        Console.WriteLine("Substring" + i + ": " + msg.Length);
                                    }

                                }
                                else
                                {
                                    await DMchannel.SendMessageAsync("```" + temp[1] + "```");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("InnerException: " + e.InnerException + ", Message: " + e.Message + ", Source: " + e.Source);
                                Console.WriteLine(guildUser.Username + " did not get their poem.");
                                await DMchannel.SendMessageAsync("Oops something went wrong with your poem! Too bad :persevere:");
                            }
                        }
                    }
                }
            }
        }

        private void test()
        {
            var listOfGuildUsers = new List<ulong>();
            foreach(var guild in _client.Guilds)
            {
                foreach(var user in guild.Users)
                {
                    if (!listOfGuildUsers.Contains(user.Id))
                    {
                        listOfGuildUsers.Add(user.Id);
                    }
                }
            }
        }
        
        private async Task SendMessageToGuildDefaultChannel()
        {
            var path = "C:/Allrepos/monika-bot/monika-discord-bot/MonikaBot/MonikaBot/Misc/quotes.txt";
            var lines = await File.ReadAllLinesAsync(path);
            foreach (var guild in _client.Guilds)
            {
                var lineNr = _random.Next(lines.Length);
                while (String.IsNullOrEmpty(lines[lineNr]))
                {
                    lineNr = _random.Next(lines.Length);
                }
                var message = lines[lineNr];

                if (guild.TextChannels.FirstOrDefault() != null)
                {
                    await guild.TextChannels.FirstOrDefault().SendMessageAsync("```" + message + " - Me```"); 
                }
            }
        }
    }
}
