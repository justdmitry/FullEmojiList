# Full Emoji list

Auto-generated C# class that can be included directly into your source code.

Save [FullEmojiList.cs](FullEmojiList.cs) into your project and use emojis in your messages without referencing additional packages:

```csharp
var greeting = $"Hello, {Emoji.WorldMap}"; // value is "Hello, 🗺️"
```

**Attention!** Always check how your messages are displayed in all required browsers/devices - some systems/fonts may not (yet) have some new emoji!


## Generator

.NET Core 3.1 console app to [re]generate emoji file.

Useful options (located directly in `Program.cs` file):

* `MaxVersion` - sets maximum unicode emoji version to include (emoji from higher versions will not be included in output file)
* `IncludeGroupInEmojiName` - when enabled, emoji names will include group names, e.g. `SmileysAndEmotion_GrinningFace` (instead of `GrinningFace`)

## Credits

Thanks to [anton-bot/Full-Emoji-List](https://github.com/anton-bot/Full-Emoji-List) for inspiration.