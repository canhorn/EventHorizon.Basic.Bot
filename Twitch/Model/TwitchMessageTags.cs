using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EventHorizon.Basic.Bot.Message;
using EventHorizon.Basic.Bot.Twitch.Pattern;

namespace EventHorizon.Basic.Bot.Twitch.Model
{
    public partial struct TwitchMessageTags : IMessageTags
    {
        private const string TRUE_AS_STRING = "1";
        private static IDictionary<string, string> EMPTY_TAGS = new Dictionary<string, string>();

        /**
         * Empty MsgTags object. Can be used if no tags should be provided, to still
         * have a MsgTags object.
         */
        public static TwitchMessageTags EMPTY = new TwitchMessageTags(null);

        private IDictionary<string, string> tags;

        public IDictionary<string, string> Tags
        {
            get { return tags; }
        }

        private TwitchMessageTags(
            IDictionary<string, string> tags
        )
        {
            if (tags == null)
            {
                this.tags = EMPTY_TAGS;
            }
            else
            {
                this.tags = tags;
            }
        }

        /**
         * Returns true if the given key is contained in these tags.
         * 
         * @param key The key to look for
         * @return true if the key is in the tags, false otherwise
         */
        public bool ContainsKey(
            string key
        )
        {
            return tags.ContainsKey(
                key
            );
        }

        /**
         * Returns true if there are no key/value pairs.
         * 
         * @return true if empty
         */
        public bool IsEmpty()
        {
            return tags.Count == 0;
        }

        public bool IsTrue(
            string key
        )
        {
            string value;
            if (tags.TryGetValue(
                key, 
                out value
            ))
            {
                return TRUE_AS_STRING.Equals(
                    value
                );
            }
            return false;
        }

        public bool IsValue(
            string key, 
            string value
        )
        {
            string outValue;
            if (tags.TryGetValue(
                key, 
                out outValue
            ))
            {
                return value.Equals(
                    outValue
                );
            }
            return false;
        }

        public bool IsEmpty(
            string key
        )
        {
            return !tags.ContainsKey(
                key
            ) || string.IsNullOrEmpty(
                tags[key]
            );
        }

        public string Get(
            string key
        )
        {
            return Get(
                key, 
                null
            );
        }

        public string Get(
            string key, 
            string defaultValue
        )
        {
            if (tags.ContainsKey(
                key
            ))
            {
                return tags[key];
            }
            return defaultValue;
        }

        public int GetInteger(
            string key, 
            int defaultValue
        )
        {
            string tag = "";
            if (tags.TryGetValue(
                key, 
                out tag
            ))
            {
                int value;
                if (int.TryParse(
                    tag, 
                    out value
                ))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        public long GetLong(
            string key, 
            long defaultValue
        )
        {
            string tag = "";
            if (tags.TryGetValue(
                key, 
                out tag
            ))
            {
                long value;
                if (long.TryParse(
                    tag, 
                    out value
                ))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        public string ToTagsString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var it = tags.Keys.AsEnumerable();
            foreach (var tag in tags)
            {
                var key = tag.Key;
                var value = tag.Value;
                if (IsValidKey(
                    key
                ))
                {
                    stringBuilder.Append(key);
                    if (IsValidValue(
                        value
                    ))
                    {
                        stringBuilder
                            .Append(
                                "="
                            ).Append(
                                EscapeValue(
                                    value
                                )
                            );
                    }
                    stringBuilder.Append(
                        ";"
                    );
                }
            }
            return stringBuilder.ToString();
        }

        public override bool Equals(
            object obj
        )
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }

            var other = (TwitchMessageTags)obj;
            return object.Equals(
                this.tags, 
                other.tags
            );
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 53 * hash + this.tags.GetHashCode();
            return hash;
        }

        private static string PATTERN = "[a-z0-9-]+";
        private static Regex REGEX = new Regex(PATTERN, RegexOptions.IgnoreCase);

        private static bool IsValidKey(
            string key
        )
        {
            return REGEX.Match(
                key
            ).Length > 0;
        }

        private static bool IsValidValue(
            string value
        )
        {
            return value != null;
        }

        private static string EscapeValue(
            string value
        )
        {
            return TagsValueEncoded(
                value
            );
        }

        public static string TagsValueEncoded(
            string valueToEncode
        )
        {
            if (valueToEncode == null)
            {
                return null;
            }

            IDictionary<string, string> replacements2Reverse = new Dictionary<string, string>();
            replacements2Reverse.Add("\\s", "\\s");
            replacements2Reverse.Add("\n", "\\n");
            replacements2Reverse.Add("\r", "\\r");
            replacements2Reverse.Add(";", "\\:");
            replacements2Reverse.Add("\\\\", "\\\\");

            return new Replacer(
                replacements2Reverse
            ).Replace(
                valueToEncode
            );
        }

        public static string DecodeTagsValue(
            string valueToDecode
        )
        {
            if (valueToDecode == null)
            {
                return null;
            }

            Dictionary<string, string> replacements2 = new Dictionary<string, string>();
            replacements2.Add("\\\\s", " ");
            replacements2.Add("\\\\n", "\n");
            replacements2.Add("\\\\r", "\r");
            replacements2.Add("\\\\:", ";");
            replacements2.Add("\\\\\\\\", "\\");
            return new Replacer(
                replacements2
            ).Replace(
                valueToDecode
            );
        }

        public override string ToString()
        {
            return tags.ToString();
        }
    }
}