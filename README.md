# Fewer Visual Distractions for Celeste

A mod for **[Celeste](https://store.steampowered.com/app/504230/Celeste/)** that lets you reduce visual stimulation/visual distractions.  

This mod is probably most useful for people prone to motion sickness, highly sensitive persons, and so on.  
Some options might also be useful for speedruns and other types of challenging play, though there are other mods made specifically for such uses (such as [Speedrun Tool](https://gamebanana.com/tools/6597), [CelesteTAS](https://gamebanana.com/tools/6715) and others).

[Open a GitHub issue](https://github.com/exscape/Celeste-FewerVisualDistractions/issues/) if you have a feature request, or found a bug!


Demo video on YouTube:  
[![Mod Demo on YouTube](https://img.youtube.com/vi/CIi7CZ_K0po/0.jpg)](https://www.youtube.com/watch?v=CIi7CZ_K0po)

https://github.com/exscape/Celeste-FewerVisualDistractions/assets/143653/220ed628-392d-459a-88fc-9a78e6dd5bfa

# Features

**Every setting is configurable, and disabled by default**, so it's up to you how to use the mod.  
After installation, all settings are available in the "Mod Options" menu when the game is paused. (And in the main menu.)

Available settings include:

* Hide certain backdrops/stylegrounds, for example to hide all moving snow (hide the Snow and WindSnow backdrops)
  - Also works with backdrops from mods, which are detected on first use and then available as a toggle
* Reduce the amount of wind snow rendered (0-100%)
* Limit the speed of (or entirely stop) backdrop motion, such as the fast-moving cloud background in Chapter 4
* Lock the background movement (parallax) in place, or lock it to follow the camera movement
* Change the stars in dream blocks (chapter 2) to follow the camera
* Show an indicator when there is wind (strength and direction) -- useful if you disable the WindSnow effect
* Tweak or remove effects on death:
  - The wipe/fade to black can be disabled
  - The ring-shaped effect around the player at death can be limited to a single color to remove blinking, or disabled entirely
  - The circular background-warping effect can be disabled
* Hide, or stop various other effects from being animated:
  - Hide waterfalls
  - Hide tentacles -- the purple-ish veil that hides half the screen in parts of chapter 6
  - Hide heat distortion in Core (also see the "Heat Wave" backdrop which can be toggled separately)
  - Hide snow and cloud movement in the PICO-8 version of Celeste
* Farewell-specific settings (also applies to some modded maps):
  - Hide floating creatures
  - Hide floating debris
  - Hide lightning bolts inside lightning/electricity areas
  - Stop lightning/electricity areas from animating
  - Stop rainbow-colored crystals (aka spinners) from changing color
  - Stop billboard noise (during the wavedash tutorials) from animating
  - Stop glitches (green lines) from animating
  - Stop the black hole animation

Settings take effect immediately, so in many cases you can see whether the setting is correct or not even before leaving the settings menu.  
A useful Celeste feature is that the HUD/menu is hidden when you hold down the Journal button (default LT on a controller). Toggling options still work while the HUD is hidden, so you can view before/after while toggling the settings, without having to look through all the text.

# Changelog

## v0.9.4 (2024-03-06)

* Add compatibility with [Speedrun Tool](https://gamebanana.com/tools/6597) save state handling.

There was a compatibility bug where using this mod together with Speedrun Tool would make using save states freeze the game entirely, which should be fixed when using v0.9.4 or later.

## v0.9.3 (2024-02-13)

By **far** the biggest update yet! Tons of changes under the hood, but several very visible ones, too.

New additions:

* Many new effect toggles (8 or so); most are only used in Farewell (and mods)
* Support for disabling individual backdrops from mods -- compatible with all modded backdrops
* "Master switch" setting to quickly toggle all settings on and off
* Wind snow (and stardust, used in Farewell) percentage setting, if you want to reduce but not disable wind snow

Improvements:

* The settings UI has been recreated from scratch, moving everything into submenus for much better organization
* Major internal rewrites to improve mod compatibility (big reduction in IL patches)
* Wind indicator display logic has been further improved (hide during cutscenes; don't randomly hide on death)
* Wind indicator can now be centered (vertically)

**Unfortunately, the settings rework means your old settings won't be respected, and need to be set up again.**

Bug fixes:

* "Follow Camera" parallax setting replaced with "Mixed", which causes fewer (if any) graphical issues but a similar look. I still recommend using "Standard" though, and considered removing this option entirely.
* "Locked" parallax setting no longer causes graphical issues in The Summit
* The "Planets" backdrop (used in Farewell) now respects the parallax settings

## v0.9.2 (2024-01-25)

* Add a graphical wind indicator with a much improved look versus the text indicator
* Improve wind indicator display logic (only show once wind has appeared; always show in rooms with changing wind)
* Add two settings for PICO-8 Celeste: disable snow, and disable cloud movement

The PICO-8 game is a secret in chapter 3, and can be played from the main menu once found.

## v0.9.1 (2024-01-22)

* Remove unrelated changes: the music and game timer no longer pause when the game is paused. These changes remained in the released mod by accident.

## v0.9.0 (2024-01-20)

Initial release.

# Installation

Install the mod from [Olympus](https://everestapi.github.io/), the installer for Celeste mod loader Everest.  
Binaries of the mod are available from [GameBanana](https://gamebanana.com/mods/491205) if you want to install it manually.

# Recommended settings

Clearly, this will be **extremely individual**, but for those wanting to remove the most distracting effects/movement while keeping the look of the game as close to standard as possible, I find the following useful:

* Wind
  - Wind Snow And Stardust Amount: 0 or 10-20 (lets you still see the wind direction without the wind indicator)
  - Show Wind Indicator: On if the above setting is set to 0
* Death Effects
  - Screen Wipes: Off
  - Rotating Death Effect: Remove flashing
* Parallax:
  - Max Parallax Speed: 0
* Various Effects:
  - Show Tentacles: Off if you're bothered by them (only used in chapter 6, Reflection)
  - Show Heat Distortion: Off (only used in chapter 8, Core)
  - Show Lightning Bolts: Off (only used in chapter 9, Farewell)
  - Animate Lightning Areas: Off (only used in chapter 9, Farewell)
  - Animate Billboard Noise: Off (only used in a small part of chapter 9, Farewell)
  - Animate Black Hole Background: Off (only used in chapter 9, Farewell)
* Backdrops:
  - Final Boss Starfield: Off (fast-moving background in chapter 6, Reflection)
  - Snow: Off (small snow particles in several chapter, but not the wind in chapter 4)

I personally use more settings than these, but I expect these to be the "big ones" for many who might be interested in using this mod.

# Parallax settings

Most settings are relatively self-explanatory (at least once you've seen the effect in question), but I feel that the parallax settings could use a bit more explanation.  
A definition of parallax is "a displacement or difference in the apparent position of an object viewed along two different lines of sight".  
In Celeste, most rooms have backgrounds that move at different speeds, and separately from the camera (often at e.g. 0.1x - 0.4x of the cameras movement speed), creating an illusion of depth.  
There is also a separate parallax effect that makes the entire background constantly move in some places, most notably chapter 4, where the clouds move with the wind.

The mod has several settings relating to parallax:

* "Max Parallax Movement Speed" only adjusts the speed of the background motion due to wind. Set to 0 in order to disable this effect entirely.
* "Parallax During Movement" adjusts the behavior of the background motion as the camera moves, and has three settings: "Standard" (multiple layers can move at different speeds), "Locked" (the background is fixed in place and does not move even when the camera moves) and "Mixed" (backgrounds follow the camera where possible without artifacts, and stays at standard otherwise).
* Finally, there's the "Dream Block Stars Follow Camera" setting which applies the above logic to the twinkling, colorful stars in dream blocks (chapter 2) when enabled.

**"Parallax During Movement" is a setting I'm considering removing** as it is hard (if not impossible) to implement correctly such that it works with all maps (including mods) without having to specifically add support for each map.

The previous setting "Follow Camera" was replaced by "Mixed" in v0.9.3 as it had pretty severe issues that made it unusable on many maps (vanilla and modded).  
"Mixed" tries to do the same thing (have the background move at the same speed as the camera) where it makes sense and won't cause graphical issues, and keeps the movement unchanged if not -- specifically, it only follows the camera if the background repeats *and* the parallax is greater than a certain amount (currently 0.2).
**However, it is quite close to Standard in appearance, yet can cause subtle unintended graphical changes, so I suggest using "Standard" where possible.**
