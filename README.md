# stardewvalley-esp
Onscreen indicators for various entities and objects in Stardew Valley.

### Example pictures
foragables | npcs | a lot of things | menu
:-----------:|:----------------------:|:------:|:-------:
![example1-npcs](https://i.imgur.com/U9TZGnw.png)|![example2-foraging](https://i.imgur.com/SvqttR7.png)|![example3-colorful](https://i.imgur.com/ptozisY.png)|![example4-menu](https://i.imgur.com/ItBPOIu.png)

### Planned features
- [x] Filter objects
- [x] Pick your own colors
- [x] Entity checklist
  - [x] NPCs
  - [x] Other players (untested, should work with NPCs)
  - [x] Farm Animals
  - [x] Foragables
  - [x] Stone, twigs, weed
  - [x] Ore panning spots
  - [x] Fishing hotspots
- [ ] Ingame menu to change settings
  - [x] Pick colors for objects
  - [ ] Pick hotkeys
- [x] Cleanup

### Settings
(outdated as of [28ab0cc](https://github.com/rei2hu/stardewvalley-esp/commit/28ab0cc395f4afcd9fb01af18deaa6173e12f553), valid for any version up to and including 1.0.2)

When you first start the mod, a file called `settings.json` will be placed in the same folder as the mod location e.g. "Stardew Valley/Mods/sdv-helper". As entities are encountered in the game, the file will be populated, however you can populate it manually if you want. Each entry is formatted like so:
```js
{
  "Name": number,
  // ...
}
```
- Name is the name of the object e.g. Weeds, Stone, and for NPCs and animals, their actual name.
- the number is a number between 0 and 20 (21?). 0 means it is disabled, and other numbers mean other colors. Specifically it follows the color picker you can find in a chest's menu or in the menu for this.

### Menu
You can open the configuration menu with `k`. It should be pretty straightforward; it allows you to easily pick colors or disable labels. If you change the settings in the middle of a game via the actual file, they can be reloaded with `l`.

### Is this cheating?
I don't know, but who cares it's Stardew Valley.
