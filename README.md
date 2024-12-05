This is the README for Elemental Siege. Thank you for playing our game.
# List of Demo/Test Levels that is playable in the final build:
- TitleScene

- Level1

- Level2

- Level3

- Level4

- Level5

# List of Demo/Test Levels not playable in the final build:
Note: Not included in final playable build
- DamageTest

- EnemySpawnerTest

- EnemyTest

- TerrainTestScene

- TestRenderScene

- TowerVariantTestScene

# Third-Party Resources

## Sounds

All sounds sourced from [Freesound](https://freesound.org/)
- Windcaller and whirlwind towers: Wind.wav by mr101986 - https://freesound.org/s/94715/ - License: Creative Commons 0

- Flamethrower tower: Flamethrower.wav by MATRIXXX_ - [https://freesound.org/s/459884/](https://freesound.org/s/459884/) - License: Creative Commons 0

- Wildfire tower: Silencyo_CC_Fire in Fireplace_Close Up_Reverberant.aif by silencyo - https://freesound.org/s/81800/ - License: Creative Commons 0

- Frozen tower: Freeze sound effect FX by antonsoederberg - https://freesound.org/s/685253/ - License: Creative Commons 0

- Fountain Tower: S22-02 Water fountain; decorative; courtyard.wav by craigsmith - https://freesound.org/s/675569/ - License: Creative Commons 0

- Tidal Tower: W_1 Wave.wav by Yarmonics - https://freesound.org/s/441852/ - License: Creative Commons 0

- Hurricane Tower: Electric zap.wav by michael_grinnell - https://freesound.org/s/512471/ - License: Creative Commons 0

- Tower Placement: One Heavy Thud by AVstudent - https://freesound.org/s/732783/ - License: Creative Commons 0

- Tectonic Tower: Cracking Earthquake (cracking soil, cracking stone) by uagadugu - https://freesound.org/s/222521/ - License: Creative Commons 0

- Fireball and Flamethrower Tower upon firing: Magic Fire Spell Movement by TheLittleCrow - https://freesound.org/s/761330/ - License: Creative Commons 0

- Fireball Tower projectile collision: Fireball Explosion by HighPixel - [https://freesound.org/people/HighPixel/sounds/431174/](https://freesound.org/people/HighPixel/sounds/431174/) - License: Creative Commons 0

- Bulwark Tower: Concrete SMASH 2 by magnuswaker -- https://freesound.org/s/522099/ -- License: Creative Commons 0

- Banishment Tower: Stone Push by msavioti -- https://freesound.org/s/558216/ -- License: Creative Commons 0 

- Quicksand Tower: Fish flopping over on sand by adviseme333 -- https://freesound.org/s/679389/ -- License: Creative Commons 0 

## Assets

Assets sourced from the Unity Store:

- [Handpainted Grass & Ground Textures](https://assetstore.unity.com/packages/2d/textures-materials/nature/handpainted-grass-ground-textures-187634)

## Tutorials Used for VFX, Textures, and Shader Textures:

- [Water texture (Substance Painter)](https://www.youtube.com/watch?v=JNLE42VlfqE)

- [Flamethrower](https://www.youtube.com/watch?v=IY2K2cOE0R8)

- [Flame trail](https://www.youtube.com/watch?v=wvK6MNlmCCE)

- [Magic Orb](https://www.youtube.com/watch?v=7bMOhNUA1bI)

- [Portal](https://www.youtube.com/watch?v=V2NZb0WN7SY)

- [Lightning Strike](https://www.youtube.com/watch?v=XPh0jiqf0iQ)

- [Ground Cracks](https://www.youtube.com/watch?v=qiAiVa0HtyE)

- [Toxic Quicksand](https://www.youtube.com/watch?v=uOhWT6TxZgE)

- [Stronghold Shield](https://www.youtube.com/watch?v=IZAzckJaSO8)

## Software and Tools:

- Maya

- Zbrush

- Zbrush third-party tool(s):

  - [Orb brushes by Michael Vicente](https://orb.gumroad.com/l/nOkHw) (commercial use permitted)
 
 - Substance Painter

- Clip Studio Paint

- Gimp

- Blender

- Minecraft (level prototyping)

- Figma (UI prototyping)


# Known Bugs:

- Towers may be placed at the wrong elevation on some tiles, or may be placeable on tiles where they should not be.

- Tidal Towers at the top of ramps are extremely powerful.

- All air towers are very strong in comparison to the other towers. 

- Stunning enough enemies to bottleneck the entire wave (if it's a large wave) is very bad for performance.

- Sometimes, some enemies lose their navigation abilities and revert to being physics objects. This soft-locks the player for several minutes until the enemy unloads after its lifespan runs out. This happens frequently on level 5.

- The final waves of the last few levels may cause lag due to the large number of enemies.

- The flamethrower tower does not do damage to enemies.

- Air enemies instantly die on spawn after wave 4 or 5, depending on the level

- Sometimes exiting out of tower placement mode doesn’t work.

# Generative AI

Usage of generative AI models for creating/revising code is indicated with comments above the function header or applicable block of code. Each model used is credited as appropriate. 
