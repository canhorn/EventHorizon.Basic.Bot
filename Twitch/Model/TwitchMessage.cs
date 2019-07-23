using System;
using System.Text.RegularExpressions;
using EventHorizon.Basic.Bot.Message;

namespace EventHorizon.Basic.Bot.Twitch.Model
{
    public struct TwitchMessage : IMessage
    {
        public static readonly TwitchMessage NULL = new TwitchMessage(
            null, 
            null, 
            null, 
            null, 
            null, 
            default(TwitchMessageTags)
        );

        private string prefix;
        private string username;
        private string command;
        private string[] parameters;
        private string trailing;
        private TwitchMessageTags tags;

        public string Prefix
        {
            get { return prefix; }
        }
        public string Username
        {
            get { return username; }
        }
        public string Command
        {
            get { return command; }
        }
        public string[] Parameters
        {
            get { return parameters; }
        }
        public string Trailing
        {
            get { return trailing; }
        }
        public IMessageTags Tags
        {
            get { return tags; }
        }

        private TwitchMessage(
            string prefix, 
            string username, 
            string command, 
            string[] parameters, 
            string trailing, 
            TwitchMessageTags tags
        )
        {
            this.prefix = prefix;
            this.username = username;
            this.command = command;
            this.parameters = parameters;
            this.trailing = trailing;
            this.tags = tags;
        }
        public static TwitchMessage Parse(
            string rawMessage
        )
        {
            if (string.IsNullOrEmpty(
                rawMessage
            ))
            {
                return TwitchMessage.NULL;
            }
            rawMessage = rawMessage.Trim();

            var tags = TwitchMessageTags.EMPTY;
            if (rawMessage.StartsWith(
                "@"
            ))
            {
                int endOfTags = rawMessage.IndexOf(
                    " "
                );
                if (endOfTags == -1)
                {
                    Console.WriteLine(
                        $"[WARN] Parsing error: Couldn't find whitespace after tags: {rawMessage}"
                    );
                    return TwitchMessage.NULL;
                }
                tags = TwitchMessageTags.parse(
                    rawMessage.Substring(
                        1, 
                        endOfTags
                    )
                );
                rawMessage = rawMessage.Substring(
                    endOfTags + 1
                );
            }

            var prefix = "";
            var command = "INVALID";
            var parameters = new string[0];
            var trailing = "";

            int endOfPrefix = 0;
            int endOfCommand = 0;

            // Get prefix if available
            if (rawMessage.StartsWith(
                ":"
            ))
            {
                endOfPrefix = rawMessage.IndexOf(
                    " "
                );
                if (endOfPrefix == -1)
                {
                    Console.WriteLine(
                        $"[WARN] Parsing error: Couldn't find whitespace after prefix: {rawMessage}"
                    );
                    return TwitchMessage.NULL;
                }
                prefix = rawMessage.Substring(
                    1, 
                    endOfPrefix
                );
            }

            // Find and get trailing if available
            // :tmi.twitch.tv CAP * ACK :twitch.tv/tags twitch.tv/commands twitch.tv/membership\r\n
            endOfCommand = rawMessage.IndexOf(
                ":", 
                endOfPrefix
            );
            if (endOfCommand == -1)
            {
                // No trailing, so the command takes up the remaining length
                endOfCommand = rawMessage.Length;
            }
            else
            {
                trailing = rawMessage.Substring(
                    endOfCommand + 1, 
                    rawMessage.Length - endOfCommand - 1
                );
            }

            // Get commands and parameters
            var commandAndParameter = rawMessage.Substring(
                endOfPrefix, 
                endOfCommand - endOfPrefix
            ).Trim();
            var parts = Regex.Split(
                commandAndParameter, 
                " "
            );
            if (parts.Length > 1)
            {
                // Get parameters if available
                parameters = new string[parts.Length - 1];
                Array.Copy(
                    parts, 
                    1, 
                    parameters, 
                    0, 
                    parts.Length - 1
                );
            }
            // First part must be command
            command = parts[0];

            var username = prefix;
            int endOfNick = username.IndexOf(
                "!"
            );
            if (endOfNick > -1)
            {
                username = username.Substring(
                    0, 
                    endOfNick
                );
            }

            return new TwitchMessage(
                prefix, 
                username, 
                command, 
                parameters, 
                trailing, 
                tags
            );
        }
    }
}