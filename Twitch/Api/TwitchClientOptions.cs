using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using EventHorizon.Basic.Bot.Message;

namespace EventHorizon.Basic.Bot.Twitch.Api
{
    public struct TwitchClientOptions
    {
        public static int VERIFIED_BOT_MESSAGE_SEND_DELAY = 4;
        public static int MODERATOR_MESSAGE_SEND_DELAY = 300;
        public static int KNOWN_BOT_MESSAGE_SEND_DELAY = 600;
        public static int STANDARD_MESSAGE_SEND_DELAY = 1500;
        public string Url { get; }
        public int Port { get; }
        public string AccessToken { get; }
        public string Nickname { get; }
        public string MainChannel { get; }
        public Func<Task> OnOpen { get; set; }
        public Func<WebSocketCloseStatus, string, Task> OnClose { get; set; }
        public Func<Exception, Task> OnError { get; set; }
        public Func<IMessage, Task> OnReceived { get; set; }
        public Action OnConnected { get; set; }
        public bool ReconnectOnError { get; set; }
        public bool ReconnectOnClose { get; set; }
        public int SecondsBetweenReconnect { get; set; }
        public Action<ClientWebSocketOptions> InternalWebSocketOptions { get; set; }
        public int MessageSendDelay { get; set; }

        public TwitchClientOptions(
            string url,
            int port,
            string accessToken,
            string nickname,
            string mainChannel
        ) {
            Url = url;
            Port = port;
            AccessToken = accessToken;
            Nickname = nickname;
            MainChannel = mainChannel;
            OnOpen = null;
            OnClose = null;
            OnError = null;
            OnReceived = null;
            OnConnected = null;
            ReconnectOnError = false;
            ReconnectOnClose = false;
            SecondsBetweenReconnect = 0;
            InternalWebSocketOptions = null;
            MessageSendDelay = STANDARD_MESSAGE_SEND_DELAY;
        }
    }
}