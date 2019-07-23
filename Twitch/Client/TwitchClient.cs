using System;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Basic.Bot.Client;
using EventHorizon.Basic.Bot.Queue.Model;
using EventHorizon.Basic.Bot.Socket;
using EventHorizon.Basic.Bot.Twitch.Api;
using EventHorizon.Basic.Bot.Twitch.Model;

namespace EventHorizon.Basic.Bot.Twitch.Client
{
    public class TwitchClient : IClientSendMessage, IStartClient
    {
        private readonly TwitchClientOptions _options;
        private bool _started = false;
        private WebSocketWrapper _socketWrapper;
        private DelayedActionQueue<string> _actionQueue;
        public TwitchClientOptions Options
        {
            get { return _options; }
        }
        public TwitchClient(
            TwitchClientOptions options
        )
        {
            this.ValidateOptions(
                options
            );
            _options = options;
            // This queue is necessary to make sure we do not hit the Command & Message Limits
            _actionQueue = DelayedActionQueue<string>.Create(
                new StandardDelayedAction(
                    (string message) => this.SendMessageToSocket(
                        message
                    )
                ),
                _options.MessageSendDelay
            );
        }
        private void ValidateOptions(
            TwitchClientOptions options
        )
        {
            this.ValidateStringParameter(
                options.Url,
                "options.Url"
            );
            if (options.Port <= 0)
            {
                throw new ArgumentException(
                    "options.Port cannot be 0 or less",
                    "options.Port"
                );
            }
            this.ValidateStringParameter(
                options.AccessToken,
                "options.AccessToken"
            );
            this.ValidateStringParameter(
                options.Nickname,
                "options.Nickname"
            );
            this.ValidateStringParameter(
                options.MainChannel,
                "options.MainChannel"
            );
            if (options.MessageSendDelay < TwitchClientOptions.VERIFIED_BOT_MESSAGE_SEND_DELAY)
            {
                throw new ArgumentException(
                    $"options.MessageSendDelay cannot be less than TwitchClientOptions.VERIFIED_BOT_MESSAGE_SEND_DELAY ({TwitchClientOptions.VERIFIED_BOT_MESSAGE_SEND_DELAY})",
                    "options.MessageSendDelay"
                );
            }
        }
        private void ValidateStringParameter(
            string paramValue,
            string paramName
        )
        {
            if (string.IsNullOrEmpty(
                paramValue
            ))
            {
                throw new ArgumentException(
                    $"{paramName} is null or empty",
                    paramName
                );
            }
        }
        public async Task Start()
        {
            if (_started)
            {
                throw new Exception("already_started");
            }
            await Task.Run(() => WebSocketClientWrapper.ConnectAsync(
                $"wss://{_options.Url}:{_options.Port}",
                CancellationToken.None,
                (context) =>
                {
                    _socketWrapper = context;

                    context.OnOpen += OnOpen;
                    context.OnClose += _options.OnClose;
                    context.OnError += _options.OnError;
                    context.OnReceive += OnReceived;

                    _options.OnConnected?.Invoke();
                },
                true,
                false,
                3,
                _options.InternalWebSocketOptions
            ));
        }

        public Task SendMessage(
            string message
        )
        {
            _actionQueue.Add(message);
            return Task.CompletedTask;
        }

        private Task SendMessageToSocket(
            string message
        )
        {
            return _socketWrapper.SendAsync(
                message
            );
        }

        private async Task OnOpen()
        {
            Console.WriteLine(
                "TwitchClient.OnOpen"
            );
            // Setup Capabilities
            await _socketWrapper.SendAsync(
                "CAP REQ :twitch.tv/membership"
            );
            await _socketWrapper.SendAsync(
                "CAP REQ :twitch.tv/tags"
            );
            await _socketWrapper.SendAsync(
                "CAP REQ :twitch.tv/commands"
            );
            // Login
            await _socketWrapper.SendAsync(
                $"PASS {_options.AccessToken}"
            );
            await _socketWrapper.SendAsync(
                $"NICK {_options.Nickname}"
            );
            // Join Main Channel
            await _socketWrapper.SendAsync(
                $"JOIN #{_options.MainChannel}"
            );
            await _options.OnOpen?.Invoke();
        }
        private async Task OnReceived(
            string message
        )
        {
            // Internal Keepalive logic
            if (message.StartsWith(
                "PING"
            ))
            {
                await _socketWrapper.SendAsync(
                    $"PONG :tmi.twitch.tv"
                );
            }
            var splitToCommands = message.Split(
                "\r\n",
                StringSplitOptions.RemoveEmptyEntries
            );
            foreach (var command in splitToCommands)
            {
                await _options.OnReceived?.Invoke(
                    TwitchMessage.Parse(
                        command
                    )
                );
            }
        }
    }
}