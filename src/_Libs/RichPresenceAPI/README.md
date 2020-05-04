# Using Quarrel Rich Presence API

#### Initialize connection
``` cs
QuarrelRichPresenceService quarrelRichPresenceService = new QuarrelRichPresenceService();
await quarrelRichPresenceService.TryConnectAsync();
```

#### Creating a game
``` cs
Game game = new Game("Test", Quarrel.RichPresence.Models.Enums.ActivityType.Playing);
```

#### Complex games
``` cs
RichGame game = new RichGame("Test", Quarrel.RichPresence.Models.Enums.ActivityType.Playing);
game.Details = "details";
game.State = "state";
```
#### Setting prescence
``` cs
await quarrelRichPresenceService.SetRawActivity(game);
```


![alt text](https://github.com/UWPCommunity/Quarrel/blob/rewrite/src/_Libs/RichPresenceAPI/Presence.png)
![alt text](https://github.com/UWPCommunity/Quarrel/blob/rewrite/src/_Libs/RichPresenceAPI/Presence2.png)
