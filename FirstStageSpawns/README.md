# IF YOU ARE ALSO USING BIGGERLOCKBOXES, MAKE SURE THAT ONE IS ATLEAST 2.0.0

When you normally spawn in on the first stage, there are no enemies in the area and you have to wait for the enmies to spawn. This is often rather annoying, because you just want to get going.

This mod changes that. The first stage will now contain prespawned enemies. It accomplishes that by telling the game that it's actually on the second stage instead when checking if it should be prespawning enemies for the first stage.

Compatible with biggerlockboxes 2.0.0+. It might seem weird I need to specify that, but that mod operates on the same place as this mod.

### Installation: 
Place the dll in your Bepinex/Plugins folder.

### Multiplayer:
Not tested. Let me know if you do test it.

### RC1 Compatibility
This mod IL hooks `RoR2.SceneDirector.PopulateScene`.

## Changelog:
- 2.0.0 Now uses an IL Hook to prevent unwanted sideeffects from occuring.
- 1.1.0 Fixed Nullreference exception that could happen when entering a special stage such as the Bazaar between time.
- 1.0.1 Updated Readme as it was unclear.
- 1.0.0 Release!