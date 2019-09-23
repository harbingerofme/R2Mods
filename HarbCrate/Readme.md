# Harb Crate
---
**Harb Crate**  is a mod that adds several new items and equipment to the game. These items were designed to not break anything, but rather play with ideas I don't think have been put to use (enough) before. With the equipment extra care was taken to not make it overpowered in combination with gesture of the drowned.

---
Overview. More detailed numbers will follow as the release draws closer. Numbers that are **bolded** are likely subject to change.

### Items
* The *Perfect Timepiece* is tier 2 defensive item that reduces the duration of debuffs and damage over time effects on you.
* The *Brawn over Brain* (rename pending) is a tier 3 defensive item that aims to improve the interaction of shields, transcendence aside, with healing items. It does this by splitting up damage taken to deal a nonlethal amount to your health before dealing the rest to your shields (and then back to your health if you run out of shields.)
### Equipment
* The *Coldsnap* freezes enemies around you for **3** seconds.
* The *Divination Distillate* heals both health and shields, and gives enemies a chance to drop items. However, there's a catch: Once you are at full health and shields, the effect stops! If uninterrupted, the duration is **5** seconds.
* The *Gravity Displacement Grenade* is a grenade that throws enemies forcefully in directions from the centre. *I am not happy with this item and it will likely see reworks*
* *The Writhing Jar* is an overloading worm in a jar. It's not friendly. It will eat your kids.

---
## RC1 compatability
The following IL hooks are used:
* RoR2.HealthComponent.TakeDamage

Compatability patches will need to be written.