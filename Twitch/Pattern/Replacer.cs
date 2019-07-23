using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EventHorizon.Basic.Bot.Twitch.Pattern
{
    public class Replacer
    {
        private readonly string[] replacementValues;
        private readonly Regex pattern;

        /// <summary>
        /// Create a Replacer that replaces the keys in the Map with their values.
        /// The keys are compiled into a Pattern as alternatives, so each key has to
        ///  be a valid regex as to not interfere with the other keys.
        /// 
        /// ArgumentException when one of the replacement keys does not compile to a Pattern
        /// </summary>
        /// <param name="replacements">The map of replacements</param>
        public Replacer(
            IDictionary<string, string> replacements
        )
        {
            this.replacementValues = new string[replacements.Count];
            var stringBuilder = new StringBuilder(
                ""
            );
            int i = 0;
            foreach (string item in replacements.Keys)
            {
                try
                {
                    new Regex(item);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        "Invalid replacement pattern.", 
                        ex
                    );
                }
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(
                        "|"
                    );
                }
                stringBuilder.Append(
                    "("
                ).Append(
                    item
                ).Append(
                    ")"
                );

                // Add the replacement values to the array in the same order as the
                // groups
                replacementValues[i] = replacements[item];
                i++;
            }
            pattern = new Regex(
                stringBuilder.ToString(),
                RegexOptions.IgnoreCase
            );
        }

        /// <summary>
        /// Replaces anything in the input String, based on the Map specified for
        ///  this Replacer.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Replace(
            string input
        )
        {
            int lastAppendPos = 0;
            Match matcher = pattern.Match(input);
            StringBuilder stringBuilder = new StringBuilder();
            Match match = matcher.NextMatch();
            while (
                match != null && match.Success
            )
            {
                stringBuilder.Append(
                    input, 
                    lastAppendPos, 
                    matcher.Index
                );
                stringBuilder.Append(
                    GetReplacement(
                        matcher
                    )
                );
                lastAppendPos = matcher.Length;
                match = matcher.NextMatch();
            }
            stringBuilder.Append(
                input, 
                lastAppendPos, 
                input.Length
            );
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the replacement for the given Matcher. Gets the index of the group
        ///  that matched and returns the replacement value for that index from the
        ///  array of replacements.
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        private string GetReplacement(
            Match matcher
        )
        {
            for (int i = 1; i <= matcher.Groups.Count; i++)
            {
                if (matcher.Groups[i] != null)
                {
                    return replacementValues[--i];
                }
            }
            return null;
        }
    }
}