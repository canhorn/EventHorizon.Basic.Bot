using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EventHorizon.Basic.Bot.Message;

namespace EventHorizon.Basic.Bot.Twitch.Model
{
    /// <summary>
    /// Factory Methods
    /// </summary>
    public partial struct TwitchMessageTags 
    {
        /// <summary>
        /// Parse the given IRCv3 tags String (no leading @) into a TwitchMessageTags object.
        /// </summary>
        /// <param name="tags">The tags String</param>
        /// <returns>TwitchMessageTags object, empty if tags was null</returns>
        public static TwitchMessageTags parse(string tags)
        {
            IDictionary<string, string> parsedTags = ParseTags(tags);
            if (parsedTags == null)
            {
                return EMPTY;
            }
            return new TwitchMessageTags(parsedTags);
        }

        /// <summary>
        /// Create a new TwitchMessageTags object with the given key/value pairs.
        /// </summary>
        /// <param name="args">Alternating key/value pairs</param>
        /// <returns></returns>
        public static TwitchMessageTags create(
            params string[] args
        )
        {
            IDictionary<string, string> tags = new Dictionary<string, string>();
            var it = args.GetEnumerator();
            while (it.MoveNext())
            {
                string key = it.Current as string;
                if (it.MoveNext())
                {
                    tags.Add(
                        key, 
                        it.Current as string
                    );
                }
                else
                {
                    tags.Add(
                        key, 
                        null
                    );
                }
            }
            return new TwitchMessageTags(
                tags
            );
        }

        private static IDictionary<string, string> ParseTags(
            string data
        )
        {
            if (data == null)
            {
                return null;
            }
            string[] tags = data.Split(
                ";"
            );
            if (tags.Length > 0)
            {
                IDictionary<string, string> result = new Dictionary<string, string>();
                foreach (var tag in tags)
                {
                    string[] keyValue = tag.Split(
                        "=", 
                        2
                    );
                    if (keyValue.Length == 2)
                    {
                        result.Add(
                            keyValue[0], 
                            DecodeTagsValue(
                                keyValue[1]
                            )
                        );
                    }
                    else if (!string.IsNullOrEmpty(
                        keyValue[0]
                    ))
                    {
                        result.Add(
                            keyValue[0], 
                            null
                        );
                    }
                }
                return result;
            }
            return null;
        }
    }
}