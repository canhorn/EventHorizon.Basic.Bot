using System.Threading.Tasks;
using EventHorizon.Basic.Bot.Client;
using EventHorizon.Basic.Bot.Twitch.Client;

namespace EventHorizon.Basic.Bot.Twitch.Extensions
{
    public static class TwitchMessageExtensions
    {
        public static Task SendPrivateMessage(
            this TwitchClient client, 
            string message
        ) 
        {
            return client.SendMessage(
                $"PRIVMSG #{client.Options.MainChannel} :{message}"
            );
        }
    }
}