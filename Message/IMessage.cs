using System.Collections.Generic;

namespace EventHorizon.Basic.Bot.Message
{
    public interface IMessage
    {
        string Prefix { get; }
        string Username { get; }
        string Command { get; }
        string[] Parameters { get; }
        string Trailing { get; }
        IMessageTags Tags { get; }
    }
    public interface IMessageTags 
    {
        IDictionary<string, string> Tags { get; }

        /// <summary>
        /// Returns true if the given key is contained in these tags.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>true if the key is in the tags, false otherwise</returns>
        bool ContainsKey(
            string key
        );

        /// <summary>
        /// Returns true if there are no key/value pairs.
        /// </summary>
        /// <returns>true if empty</returns>
        bool IsEmpty();

        /// <summary>
        /// Returns true if the given key in the tags is equal to "1", false
        ///  otherwise.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if equal to 1, false otherwise</returns>
        bool IsTrue(
            string key
        );

        /// <summary>
        /// Check if given key has given value in its list.
        /// </summary>
        /// <param name="key">The key to validate.</param>
        /// <param name="value">The value to check.</param>
        /// <returns>True if equal, false otherwise</returns>
        bool IsValue(
            string key, 
            string value
        );

        /// <summary>
        /// Check if given key has any values.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if empty, false otherwise</returns>
        bool IsEmpty(
            string key
        );

        /// <summary>
        /// Returns the String associated with key, or null if the key doesn't exist.
        /// </summary>
        /// <param name="key">The key to look up in the tags</param>
        /// <returns>String associated with this key, or null</returns>
        string Get(
            string key
        );

        /// <summary>
        /// Returns the String associated with key, or the defaultValue if the key
        ///  doesn't exist.
        /// </summary>
        /// <param name="key">The key to look up in the tags</param>
        /// <param name="defaultValue">The default value to return if key isn't in tags</param>
        /// <returns>String associated with this key, or defaultValue</returns>
        string Get(
            string key, 
            string defaultValue
        );

        /// <summary>
        /// Returns the integer associated with the given key, or the defaultValue if
        ///  no integer was found for that key.
        /// </summary>
        /// <param name="key">The key to retrieve the value for</param>
        /// <param name="defaultValue">The default value to return if the given key doesn't point to an integer value</param>
        /// <returns>The integer associated with the key, or defaultValue</returns>
        int GetInteger(string key, int defaultValue);

        /// <summary>
        /// Returns the long associated with the given key, or the defaultValue if no 
        ///  long was found for that key.
        /// </summary>
        /// <param name="key">The key to retrieve the value for</param>
        /// <param name="defaultValue">The default value to return if the given key doesn't point to a long value</param>
        /// <returns></returns>
        long GetLong(string key, long defaultValue);

         /// <summary>
         /// Build a IRCv3 tags String for this tags object (no leading @).
         /// </summary>
         /// <returns>The tags string (may be empty if this tags object is empty)</returns>
        string ToTagsString();
    }
}