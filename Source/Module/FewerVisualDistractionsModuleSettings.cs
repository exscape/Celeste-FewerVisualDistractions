using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Celeste.Mod.FewerVisualDistractions;

public class FewerVisualDistractionsModuleSettings : EverestModuleSettings
{
    public enum OverrideAllValue
    {
        Disabled,
        HideAll,
        ShowAll
    }

    public enum DeathEffectSettingValue
    {
        Standard,
        NoFlashes,
        Hidden
    }

    public enum ParallaxSettingValue
    {
        Standard,
        FollowCamera,
        Locked
    }

    public enum PositionValue
    {
        Top,
        Center,
        Bottom
    }

    public enum WindIndicatorTypeValue
    {
        Graphical,
        Text
    }

    [SettingSubText("Mod master switch. If this is off, all other settings below are ignored.")]
    [SettingName("FewerVisualDistractions_Settings_ModEnabled")]
    public bool ModEnabled { get; set; } = true;

    [SettingName("FewerVisualDistractions_Settings_Wind")]
    public WindSettingsMenu Wind { get; set; } = new WindSettingsMenu();

    [SettingSubMenu]
    public class WindSettingsMenu
    {
        [SettingSubText("Amount of Wind Snow / Stardust to render, in percent")]
        [SettingRange(0, 100, LargeRange = true)]
        public int WindSnowAndStardustAmount { get; set; } = 100;

        [SettingSubText("Show an indicator when there is wind. Useful if Wind Snow is set to zero above")]
        public bool ShowWindIndicator { get; set; } = false;

        public WindIndicatorTypeValue WindIndicatorType { get; set; } = WindIndicatorTypeValue.Graphical;

        public PositionValue WindIndicatorPosition { get; set; } = PositionValue.Top;
    }

    public DeathEffectsMenu DeathEffects { get; set; } = new DeathEffectsMenu();

    [SettingSubMenu]
    public class DeathEffectsMenu
    {
        [SettingSubText("Show screen wipes on death?")]
        public bool ScreenWipes { get; set; } = true;

        [SettingSubText("Changes the rotating objects around the player during death and respawn")]
        public DeathEffectSettingValue RotatingDeathEffect { get; set; } = DeathEffectSettingValue.Standard;

        [SettingSubText("Enable the warping effect around the player as they die?")]
        public bool WarpingDeathEffect { get; set; } = true;
    }

    [SettingName("FewerVisualDistractions_Settings_Parallax")]
    public ParallaxSettingsMenu Parallax { get; set; } = new ParallaxSettingsMenu();

    [SettingSubMenu]
    public class ParallaxSettingsMenu
    {
        [SettingRange(0, 50, true)]
        [SettingName("FewerVisualDistractions_Settings_MaxParallaxSpeed")]
        [SettingSubText("Limits the speed of the background parallax (e.g. in windy areas)")]
        public int MaxParallaxSpeed { get; set; } = 50;

        [SettingSubText("Removes/changes parallax during player movement. The setting above also applies!")]
        public ParallaxSettingValue ParallaxDuringMovement { get; set; } = ParallaxSettingValue.Standard;

        [SettingSubText("Change the stars in dream blocks to follow the camera?")]
        public bool DreamBlockStarsFollowCamera { get; set; } = false;
    }

    public VariousEffectsMenu VariousEffects { get; set; } = new VariousEffectsMenu();

    [SettingSubMenu]
    public class VariousEffectsMenu
    {
        [SettingSubText("Show waterfalls (in Reflection)?")]
        public bool ShowWaterfalls { get; set; } = true;

        [SettingSubText("Show tentacles (purple-ish veil covering half the screen) in Reflection?")]
        public bool ShowTentacles { get; set; } = true;

        [SettingSubText("Show heat distortion in Core?")]
        public bool ShowHeatDistortion { get; set; } = true;

        [SettingSubHeader("Farewell-specific settings")]
        [SettingSubText("Show floating Debris in Farewell? (Small specks of dust)")]
        public bool ShowFloatingDebris { get; set; } = true;

        [SettingSubText("Show background floating creatures in Farewell?")]
        public bool ShowFloatingCreatures { get; set; } = true;

        [SettingSubText("Show lightning bolts inside Farewell lightning/electrical areas?")]
        public bool ShowLightningBolts { get; set; } = true;

        [SettingSubText("Animate edges of Farewell lightning/electrical areas?")]
        public bool AnimateLightningAreas { get; set; } = true;

        [SettingSubText("Allow rainbow colored crystals in Farewell to change colors?")]
        public bool AnimateCrystalColors { get; set; } = true;

        [SettingSubText("Animate background noise on billboards (tutorial screens) in Farewell?")]
        public bool AnimateBillboardNoise { get; set; } = true;

        [SettingSubText("Animate glitching green lines in Farewell?")]
        public bool AnimateGlitches { get; set; } = true;

        [SettingSubText("Animate the black hole background in Farewell?")]
        public bool AnimateBlackHoleBackground { get; set; } = true;

        [SettingSubHeader("PICO-8 settings")]
        [SettingName("FewerVisualDistractions_Settings_ShowPicoSnow")]
        [SettingSubText("Render snow particles in the PICO-8 emulator?")]
        public bool ShowPico8Snow { get; set; } = true;

        [SettingSubText("Enable background cloud movement in the PICO-8 emulator?")]
        [SettingName("FewerVisualDistractions_Settings_AnimatePicoClouds")]
        public bool Pico8CloudMovement { get; set; } = true;

        [SettingSubText("Enable blinking hair for double dash in the PICO-8 emulator?")]
        [SettingName("FewerVisualDistractions_Settings_BlinkingHair")]
        public bool Pico8BlinkingHair { get; set; } = true;
    }

    [SettingName("FewerVisualDistractions_Settings_Backdrops")]
    public BackdropsMenu Backdrops { get; set; } = new BackdropsMenu();

    [SettingSubMenu]
    public class BackdropsMenu
    {
        [SettingSubText("This setting overrides all the individual settings below")]
        public OverrideAllValue OverrideAllBackdrops { get; set; } = OverrideAllValue.Disabled;

        // One setting for each backdrop available in the vanilla game
        [SettingSubText("Background used in Farewell")]
        public bool Blackhole { get; set; } = true;

        [SettingSubText("Small white stars used in a single room of Core")]
        public bool CoreStars { get; set; } = true;

        [SettingSubText("Not used (see Stars BG instead)")]
        public bool DreamStars { get; set; } = true;

        [SettingSubText("Fast-moving background in Reflection")]
        public bool FinalBossStarfield { get; set; } = true;

        [SettingSubText("Slanted, translucent, pink rectangles in Reflection")]
        public bool Godrays { get; set; } = true;

        [SettingSubText("Tiny floating particles (blue/red) in Core. Also see Heat Distortion setting above")]
        public bool HeatWave { get; set; } = true;

        [SettingSubText("Tiny floating particles in Mirror Temple")]
        public bool MirrorParticles { get; set; } = true;

        [SettingSubText("Aurora in the intro of Reflection")]
        public bool NorthernLights { get; set; } = true;

        [SettingSubText("Used for almost all background images, see README for more info")]
        public bool Parallax { get; set; } = true;

        [SettingSubText("Falling flower petals in Reflection")]
        public bool Petals { get; set; } = true;

        [SettingSubText("Star-shaped \"planets\" in Farewell")]
        public bool Planets { get; set; } = true;

        [SettingSubText("Rain used in several chapters")]
        public bool Rain { get; set; } = true;

        [SettingSubText("Tiny floating particles in Reflection")]
        public bool ReflectionParticles { get; set; } = true;

        [SettingSubText("Snow used in several chapters")]
        public bool Snow { get; set; } = true;

        [SettingSubText("Particles showing wind in Farewell; amount can be adjusted in Wind Settings")]
        public bool Stardust { get; set; } = true;

        [SettingSubText("Stars moving around in Farewell")]
        public bool Starfield { get; set; } = true;

        [SettingSubText("Falling stars in Old Site")]
        public bool Stars { get; set; } = true;

        [SettingSubText("Not used (but see Various Effects above)")]
        public bool Tentacles { get; set; } = true;

        [SettingSubText("Snow showing wind; amount can be adjusted in Wind Settings")]
        public bool WindSnow { get; set; } = true;
    }

    // Third-party backdrops seen since the mod was first installed.
    // Allows for toggling these (in a submenu only visible in the main menu) without having to add support for every custom mod.
    // Key: Type FullName (instance.GetType().FullName)
    // Value: (Assembly as string, backdrop enabled)
    [SettingIgnore]
    public SortedDictionary<string, (string, bool)> AdditionalBackdrops { get; set; } = [];

    [SettingName("FewerVisualDistractions_Settings_BackdropsFromMods")]
    [YamlIgnore]
    public BackdropsFromModsMenu BackdropsFromMods { get; set; } = new BackdropsFromModsMenu();

    [SettingSubMenu]
    public class BackdropsFromModsMenu
    {
        [SettingSubText("This setting overrides all the individual settings below")]
        public OverrideAllValue OverrideAllModdedBackdrops { get; set; } = OverrideAllValue.Disabled;

        // The Create...Entry method just below replaces this one item with every modded backdrop
        [YamlIgnore]
        public bool BackdropsFromModsDummy { get; set; } = true;

        // Don't make this static no matter what your IDE says; Everest only looks for instance methods
        public void CreateBackdropsFromModsDummyEntry(TextMenuExt.SubMenu subMenu, bool inGame)
        {
            if (FewerVisualDistractionsModule.Settings.AdditionalBackdrops.Count == 0)
            {
                subMenu.Add(new TextMenu.Button("No modded backdrops encountered yet!"));
                subMenu.Add(new TextMenu.Button("Make sure you've loaded a level with custom"));
                subMenu.Add(new TextMenu.Button("backdrops, and then return here."));
                return;
            }

            var backdropRegex = new Regex(@"\.([^.]+)$");

            foreach (var (fullName, (assembly, value)) in FewerVisualDistractionsModule.Settings.AdditionalBackdrops)
            {
                // Hide entries from mods not currently active.
                // Save the settings though, in case it's reenabled later.
                if (!Everest.Modules.Any(m => m.GetType().Assembly.GetName().Name == assembly))
                    continue;

                Match backdropMatch = backdropRegex.Match(fullName);
                string displayName;

                // "Mod name: backdrop name" if possible, otherwise use the full name including namespace
                if (backdropMatch.Success)
                    displayName = $"{assembly}: {backdropMatch.Groups[1].Value}";
                else
                    displayName = fullName;

                subMenu.Add(new TextMenu.OnOff(displayName, value).Change(newValue =>
                    FewerVisualDistractionsModule.Settings.AdditionalBackdrops[fullName] = (assembly, newValue)));
            }
        }
    }
}
