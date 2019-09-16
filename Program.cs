using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading.Tasks;
using EventHorizon.Basic.Bot.Message;
using EventHorizon.Basic.Bot.Twitch.Client;
using EventHorizon.Basic.Bot.Twitch.Api;
using EventHorizon.Basic.Bot.Twitch.Extensions;

namespace EventHorizon.Basic.Bot
{
    class Program
    {
        static string Url = "irc-ws.chat.twitch.tv";
        static int Port = 443;
        static string Nickname = "[[BOT_NICKNAME]]";
        static string AccessToken = "[[BOT_ACCESS_TOKEN]]";
        static string MainChannel = "[[BOT_MAIN_CHANNEL]]";

        static TwitchClient client;
        static void Main(
            string[] args
        )
        {
            Console.WriteLine(File.ReadAllText("AppDetails.txt"));
            StartClient();
            // Start the Console loop.
            var running = true;
            while (running)
            {
                var promptText = Console.ReadLine();
                var command = promptText.Split(" ")[0];
                switch (command)
                {
                    case "start":
                        StartClient();
                        break;
                    case "exit":
                        running = false;
                        break;
                }
            }
        }

        private static void StartClient()
        {
            if (client != null)
            {
                Console.WriteLine(
                    "[WARN] Client already started."
                );
                return;
            }
            client = new TwitchClient(
                new TwitchClientOptions(
                    Url,
                    Port,
                    AccessToken,
                    Nickname,
                    MainChannel
                )
                {
                    OnOpen = OnOpen,
                    OnClose = OnClose,
                    OnReceived = OnReceived,
                    OnConnected = () =>
                    {
                        Console.WriteLine("OnConnect");
                    },
                    ReconnectOnError = true,
                    SecondsBetweenReconnect = 3,
                    MessageSendDelay = TwitchClientOptions.MODERATOR_MESSAGE_SEND_DELAY
                }
            );
            Console.WriteLine("Waiting for client Start...");
            Task.Run(() => client.Start());
        }

        static Task OnOpen()
        {
            Console.WriteLine(
                "OnOpen"
            );
            return Task.CompletedTask;
        }
        static Task OnClose(
            WebSocketCloseStatus status,
            string description
        )
        {
            Console.WriteLine(
                "OnClose"
            );
            Console.WriteLine(
                $"{status}: {description}"
            );
            return Task.CompletedTask;
        }
        static Task OnError(
            Exception error
        )
        {
            Console.WriteLine(
                "OnError"
            );
            Console.WriteLine(
                error.Message
            );
            return Task.CompletedTask;
        }
        static async Task OnReceived(
            IMessage message
        )
        {
            LogRawMessage(message);
            if ("PRIVMSG".Equals(message.Command))
            {
                if (message.Trailing.Contains(
                    "!rollDice"
                ))
                {
                    await client.SendPrivateMessage(
                        $"@{message.Username} rolled a dice and got {RollDice()}"
                    );
                }
            }
        }

        static Random random = new Random();
        static int RollDice()
        {
            return random.Next(1, 6);
        }

        private static void LogRawMessage(IMessage message)
        {
            if (!string.IsNullOrWhiteSpace(
                message.Trailing
            ))
            {
                Console.WriteLine(
                    message.Trailing
                );
            }
        }
    }
}
