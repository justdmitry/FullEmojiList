# Full Emoji list

Auto-generated C# class that can be included directly into your source code.

Save [FullEmojiList.cs](https://github.com/justdmitry/FullEmojiList/blob/master/FullEmojiList.cs) into your project, or install [FullEmojiList](https://www.nuget.org/packages/FullEmojiList) NuGet package, and use emojis in your messages:

```csharp
var greeting = $"Hello, {Emoji.WorldMap}"; // value is "Hello, 🗺️"
```

**Attention!** Always check that your messages are correctly displayed in all required browsers and devices! Some systems/fonts may not (yet) have some new emoji, so you'll need to choose other emoji. Downgrade to older file/package version to have emojis from previous Unicode versions only.

[![NuGet](https://img.shields.io/nuget/v/FullEmojiList.svg?maxAge=86400&style=flat)](https://www.nuget.org/packages/FullEmojiList/) ![Unicode v15.1](https://img.shields.io/badge/Unicode-v15.1-5d57ff?style=flat) ![.NET Any](https://img.shields.io/badge/.NET-Any_version%21-512BD4?style=flat)


## Generator

.NET 6.0 console app to [re]generate emoji file.

Useful options (located directly in `Program.cs` file):

* `MaxVersion` - sets maximum unicode emoji version to include (emoji from higher versions will not be included in output file)
* `IncludeGroupInEmojiName` - when enabled, emoji names will include group names, e.g. `SmileysAndEmotion_GrinningFace` (instead of `GrinningFace`)

## Credits

Thanks to [anton-bot/Full-Emoji-List](https://github.com/anton-bot/Full-Emoji-List) for inspiration.