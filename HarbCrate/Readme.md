
# HarbCrate

**HarbCrate**  is a mod that adds several defensive items and equipment to the game. It's mostly centered around shields.
Most items have a display model, and appear properly on most survivors.
![preview](https://cdn.discordapp.com/attachments/585466102256238602/709014553320620053/unknown.png)

---

### Items

* *Fragment* is a tier 1 (white) useless item, but maybe it does something when you get enough of them.
* The *Obsidian Bouche* is a tier 2 (green) defensive item that increases your maximum shield on multikill. It's comparable to infusion.
* The *Brawn over Brain* is a tier 3 (red) defensive item that aims to improve the interaction of shields, transcendence aside, with healing items. It does this by splitting up damage taken to deal a nonlethal amount to your health before dealing the rest to your shields (and then back to your health if you run out of shields). It also gives a 50% debuff duration reduction while you have shields.

### Equipment

* The *Coldsnap* freezes enemies around you for `3` seconds.
* The *Divination Distillate* heals both health and shields over time, and increases your luck while active. However, there's a catch: Once you are at full health and shields, the effect stops! If uninterrupted, the duration is `7` seconds.
* *The Writhing Jar* is a lunar equipment.

---

You can view detailed breakdowns of the items on the [wiki](https://github.com/harbingerofme/R2Mods/wiki/Harbcrate). Be warned, it contains spoilers and it might be more fun to find out yourself.


### Known Issues

There's a few bugs I haven't been able to work out in time. They are in order of severity. Most important issues first.

* Models aren't properly visible in the logbook. *I would like to solve this as much as possible, but it's a rabbit hole I simply have not the time for to go down into.*
* The Fragment doesn't have a model and is a question mark. *I'm not a modeller and I commission all my models. This one didn't go through. If you are a modeller and texturer, get in contact with me.*
* The Divination Distillate buff is just an orange square. *Again, time constraint here. It might be an upsteam issue, do not know.*
* The max shields take a few seconds to update when using the Obsidian Bouche as a client. *This bug is purely visible and eventually fixes itself, so I could care less.*

### Special Thanks

In no particular order:

* **Flow** for being my modeller.
* **Alex** for helping write the Writhing Jar lore.
* **Ghor** for helping me solve my prefab issue, helping me figure out how to display rules for each indivual character, and for changing how luck was handled in the base game so I could modify it. Like seriously, thank you.
* **iDeath** for patiently listen to me being stupid on items and buffs.
* And **you** for reading all the way to the end.

### Changelog

* 1.0.1
	* Fixed a bug where the names of items wouldn't be XML safe internally. This lead to run reports failing.