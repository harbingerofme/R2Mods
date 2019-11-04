# HarbTweaks #

**HarbTweaks** is a collection of small mods that are each too small to have their own page. While most of these mods have been released on their own, they have been updated to disable themselves if they find this mod loaded.

**No interference guarantee**: if a tweak is disabled, it will not be doing anything that can break. If a tweak causes interference with another mod, disabling the tweak will ensure the other mod will run normally. If you are using the configuration manager, this means that these tweaks can be disabled/enabled/configured on the fly and have immediate effects.

Recommend that you use [Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) *[(download page)](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases)* to modify these values in game.

**Installation:** Drop into your Bepinex/plugins/ folder.

--- 

## Enabled by default:

* **Bigger Lockboxes** makes the rusted lockbox spawned from rusted keys  increase in size as you acquire more keys. It can be configured to just all scale up all boxes in the first place and not scale with keys. **Multiplayer:** entirely clientside.
* **First Stage Spawns** aims to improves the pace of the game's start. Enemies appear prespawned on the first stage, at twice the density of a normal prespawn. Can be configured for different densities. **Multiplayer:** entirely hostside
* **No More Triple Question** prevents multishop terminals roll a question mark three times, and also prevents them from rolling three times the same item. Can be configured to disallow question marks and duplicates completely. **Multiplayer:** Let me know.

---

## Disabled by default:

These are niche features asked for by a small subset of people.

* **Greedy Lockboxes** prevents players from opening rusted lockboxes if they don't have a key. **Multiplayer:** Hostside, unknown for only client.
* **No Forward Saw** sets the force applied on the player when using MUL-T's Power Saw to `0`. Can be configured. **Multiplayer:** Let me know.
* **Shorter Medkits** makes the time you have to wait for a medkit to apply to `0.9` seconds. Can be configured. **Multiplayer:** Hostside.

---

##License, github stuff:## 

This will also appear on Thunderstore because I will not write two READMEs.

All stuff outside /Modules may be and is intended to be reused without attribution.
Stuff inside /Modules must be clearly attributed.