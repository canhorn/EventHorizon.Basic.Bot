# About

This project is a bear minimum library for creating Bots.  

## WebSocket Abstraction
The project also includes an abstraction around a raw WebSocket to help with event handling and triggering.

## Twitch Client/Bot

Included in the project is a client that can be used to connect a bot/user to the Twitch IRC client using the WebSocket connection. The API allows for sending raw message against the API, or using an extension it can send private messages. On received messages it will also parse the message into an easier to work with POCO.

# Implementation

To implement the client use the below example, checkout the Program.cs to see a working implementation with the client started on a background thread.

```csharp
var Url = "irc-ws.chat.twitch.tv";
var Port = 443;
var AccessToken = "[[BOT_ACCESS_TOKEN]]";
var Nickname = "[[BOT_NICKNAME]]";
var MainChannel = "[[BOT_MAIN_CHANNEL]]";
var client = new TwitchClient(
    new TwitchClientOptions(
        Url,
        Port,
        AccessToken,
        Nickname,
        MainChannel
    )
    {
        {
            OnOpen = () => Task.Run(() => {
                Console.WriteLine("OnOpen");
            }),
            OnClose = (status, description) => Task.Run(() => {
                Console.WriteLine("OnClose");
            }),
            OnReceived = (message) => Task.Run(() => {
                Console.WriteLine("OnReceived");
            }),
            OnConnected = () =>
            {
                Console.WriteLine("OnConnect");
            },
            ReconnectOnError = true,
            SecondsBetweenReconnect = 3,
            MessageSendDelay = TwitchClientOptions.MODERATOR_MESSAGE_SEND_DELAY
        }
    }
);
await client.Start();
```