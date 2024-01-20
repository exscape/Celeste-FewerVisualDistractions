# Fewer Visual Distractions for Celeste

A mod for **[Celeste](https://store.steampowered.com/app/504230/Celeste/)** that lets you reduce visual stimulation/visual distractions.  
This mod is mostly intended for people who for one reason or another can't tolerate or don't want "busy" visuals.

# Features

* Hide certain backdrops/stylegrounds, for example to hide all moving snow (hide the Snow and WindSnow backdrops)
* Limit the speed of (or entirely stop) backdrop motion, such as the fast-moving cloud background in Chapter 4
* Lock the background movement (parallax) in place, or lock it to follow the camera movement
* Change the stars in dream blocks (chapter 2) to follow the camera
* Show a text indicator when there is wind (strength and direction) -- useful if you disable the WindSnow effect
* Tweak or remove effects on death:
  - The wipe/fade to black can be disabled
  - The ring-shaped effect around the player at death can be limited to a single color to remove blinking, or disabled entirely
  - The circular background-warping effect can be disabled
* Hide various other effects:
  - Waterfalls
  - Tentacles -- the purple-ish veil that hides half the screen in parts of chapter 6
  - Heat distortion in Core (also see the "Heat Wave" backdrop which can be toggled separately)

**Every setting is configurable**, so it's up to you how to use the mod.  
**All settings are disabled by default**, so installing the mod should not change anything; you need to enable the setting you're interested in.

Settings take effect immediately, so in many cases you can see whether the setting is correct or not even before leaving the settings menu.  

This mod is probably most useful for people prone to motion sickness, highly sensitive persons, and so on.  
Some options might also be useful for speedruns and other types of challenging play, though there are other mods made specifically for such uses (such as [Speedrun Tool](https://gamebanana.com/tools/6597), [CelesteTAS](https://gamebanana.com/tools/6715) and others).

# Parallax settings

Most settings are relatively self-explanatory (at least once you've seen the effect in question), but I feel that the parallax settings could use a bit more explanation.  
A definition of parallax is "a displacement or difference in the apparent position of an object viewed along two different lines of sight".  
In Celeste, most rooms have backgrounds that move at different speeds, and separately from the camera (often at e.g. 0.1x - 0.4x of the cameras movement speed), creating an illusion of depth.  
There is also a separate parallax effect that makes the entire background constantly move in some places, most notably chapter 4, where the clouds move with the wind.

The mod has several settings relating to parallax:

* "Max Parallax Speed" only adjusts the speed of the background motion due to wind. Set to 0 in order to disable this effect entirely.
* "Parallax During Movement" adjusts the behavior of the background motion as the camera moves, and has three settings: "Standard" (multiple layers can move at different speeds), "Follow Camera" (the effect is disabled, and the background moves at the same rate as the level itself) and "Locked" (the background is fixed in place and does not move even when the camera moves).
* Finally, there's the "Dream Block Stars Follow Camera" setting which applies the above logic to the twinkling, colorful stars in dream blocks (chapter 2) when enabled.

I feel that "Follow Camera" is the least bothersome type for the latter two settings.

# Recommended settings

Clearly, this will be **extremely individual**, but for those wanting to remove distracting effects while keeping the look of the game as close to standard as possible, I find the following useful:

Show Wind Indicator: On  
Screen Wipes: Off  
Rotating Death Effect: Remove flashing  
Warping Death Effect: Off  
Max Parallax Speed: 0 (makes the background static in chapters where it moves even when you stand still)  
Parallax During Movement: Follow Camera  
Dream Block Stars Follow Camera: On  
Show Waterfalls: Off (though they are pretty)  
Show Tentacles: Off (makes the game itself easier though, unlike almost every other setting)  
Show Heat Distortion: Off  
Override All Backdrops: No Override  

After that comes a list of all individual backdrops.  
The ones I've personally disabled are Snow, Stars BG (first used in chapter 2), Wind Snow (first used in chapter 4), Final Boss Starfield (used in chapter 6) and Heat Wave (used in chapter 8).
