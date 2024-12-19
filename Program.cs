﻿namespace Generator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class Program
    {
        /// <summary>
        /// Source URL of Unicode emoji text file.
        /// </summary>
        private const string SourceUrl = "https://unicode.org/Public/emoji/latest/emoji-test.txt";

        /// <summary>
        /// Maximum emoji version to include in output file (last version is probably not supported by browsers/fonts yet).
        /// </summary>
        private const float MaxVersion = 14.0F;

        /// <summary>
        /// Output .cs file name/location.
        /// </summary>
        private const string OutFile = "FullEmojiList.cs";

        /// <summary>
        /// Set to 'true' to generate long names like `SmileysAndEmotion_GrinningFace` (instead of `GrinningFace`).
        /// </summary>
        private const bool IncludeGroupInEmojiName = false;

        public static async Task Main(string[] args)
        {
            using var outputFile = File.CreateText(OutFile);

            var stat = await new Program().RunAsync(outputFile).ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("DONE");
            Console.WriteLine($"  Groups: {stat.groupCount}");
            Console.WriteLine($"  Emojis: {stat.emojiCount}");
        }

        private static string FormatName(string source)
        {
            var parts = source
                .Replace(':', '_')
                .Replace('-', ' ')
                .Replace("&", "And")
                .Replace("#", "NumberSign")
                .Replace("*", "Asterisk")
                .Replace(",", string.Empty)
                .Replace("’", string.Empty)
                .Split(new[] { ' ', '-', ',', '’', '!', '“', '”', '(', ')', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(x));

            return string.Concat(parts);
        }

        private async Task<(int groupCount, int emojiCount)> RunAsync(TextWriter destination)
        {
            using var source = await DownloadAsync().ConfigureAwait(false);

            await destination.WriteLineAsync("// <auto-generated />");
            await destination.WriteLineAsync("namespace System.Text");
            await destination.WriteLineAsync("{");

            await destination.WriteLineAsync("    /// <summary>");
            await destination.WriteLineAsync($"    /// Emoji list (from Unicode version {MaxVersion:0.0}).");
            await destination.WriteLineAsync("    /// </summary>");
            await destination.WriteLineAsync("    /// <remarks>");
            await destination.WriteLineAsync($"    /// Created from <see href=\"{SourceUrl}\"/> using <see href=\"https://github.com/justdmitry/FullEmojiList\"/> generator at {DateTimeOffset.UtcNow:R}.");
            await destination.WriteLineAsync("    /// </remarks>");
            await destination.WriteLineAsync("    public static class Emoji");
            await destination.WriteLineAsync("    {");

            var groupCount = 0;
            var emojiCount = 0;

            var lastGroup = string.Empty;
            var lastSubgroup = string.Empty;

            await foreach (var emoji in ParseSourceAsync(source))
            {
                if (emoji.group != lastGroup)
                {
                    if (groupCount > 0)
                    {
                        await destination.WriteLineAsync($"        #endregion");
                        await destination.WriteLineAsync();
                    }

                    lastGroup = emoji.group;
                    groupCount++;
                    await destination.WriteLineAsync($"        #region {emoji.group}");
                }

                if (IncludeGroupInEmojiName)
                {
                    await destination.WriteLineAsync($"        public const string {emoji.group}_{emoji.name} = \"{emoji.value}\";");
                }
                else
                {
                    await destination.WriteLineAsync($"        public const string {emoji.name} = \"{emoji.value}\";");
                }

                emojiCount++;
            }

            if (groupCount > 0)
            {
                await destination.WriteLineAsync($"        #endregion");
            }

            await destination.WriteLineAsync("    }");
            await destination.WriteLineAsync("}");
            await destination.FlushAsync();

            return (groupCount, emojiCount);
        }

        private async Task<StreamReader> DownloadAsync()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(SourceUrl).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return new StreamReader(stream);
        }

        private async IAsyncEnumerable<(string group, string subgroup, string name, string value)> ParseSourceAsync(StreamReader stream)
        {
            const string groupPrefix = "# group:";
            const string subgroupPrefix = "# subgroup:";
            const string commentPrefix = "#";

            var group = string.Empty;
            var subgroup = string.Empty;

            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith(groupPrefix, StringComparison.Ordinal))
                {
                    group = FormatName(line.Substring(groupPrefix.Length));
                    continue;
                }

                if (line.StartsWith(subgroupPrefix, StringComparison.Ordinal))
                {
                    subgroup = FormatName(line.Substring(subgroupPrefix.Length));
                    continue;
                }

                if (line.StartsWith(commentPrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var emoji = ParseEmoji(line);
                if (emoji != null)
                {
                    yield return (group, subgroup, emoji.Value.name, emoji.Value.value);
                }
            }
        }

        private (string name, string value)? ParseEmoji(string line)
        {
            var parts = line.Split(new[] { ';', '#' }, 3);

            if (parts[1].Trim() != "fully-qualified")
            {
                return null;
            }

            var versionAndName = parts[2].Split('E', 2)[1].Split(' ', 2);
            var version = float.Parse(versionAndName[0]);

            if (version > MaxVersion)
            {
                return null;
            }

            var name = FormatName(versionAndName[1]);

            if (!IncludeGroupInEmojiName && char.IsDigit(name[0]))
            {
                name = "_" + name;
            }

            var surrogates = parts[0].Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x, NumberStyles.HexNumber))
                .Select(x => char.ConvertFromUtf32(x));

            var value = string.Concat(surrogates);

            return (name, value);
        }
    }
}
