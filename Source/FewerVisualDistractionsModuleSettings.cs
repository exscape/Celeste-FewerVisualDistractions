namespace Celeste.Mod.FewerVisualDistractions {
    public class FewerVisualDistractionsModuleSettings : EverestModuleSettings {

        public enum OverrideAllValue
        {
            Disabled,
            HideAll,
            ShowAll
        }

        public enum DeathEffectSettingValue
        {
            Hidden,
            NoFlashes,
            Standard
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
            Bottom
        }

        [SettingSubText("Show a text indicator when there is wind. Useful if Wind Snow is disabled below")]
        public bool ShowWindIndicator { get; set; } = false;
        public PositionValue WindIndicatorPosition { get; set; } = PositionValue.Top;
        [SettingSubText("Distance between indicator and the selected screen edge")]
        [SettingRange(-2, 55, LargeRange = true)]
        public int WindIndicatorOffset { get; set; } = 0;

        [SettingSubHeader("Death effects")]
        [SettingSubText("Show screen wipes when the level reloads, e.g. on death")]
        public bool ScreenWipes { get; set; } = true;

        [SettingSubText("Changes the rotating objects around the player as they die and respawn")]
        public DeathEffectSettingValue RotatingDeathEffect { get; set; } = DeathEffectSettingValue.Standard;

        [SettingSubText("Enable the warping effect around the player as they die?")]
        public bool WarpingDeathEffect { get; set; } = true;

        [SettingSubHeader("Various effects")]
        [SettingRange(0, 50, true)]
        [SettingSubText("Limits the speed of the background parallax (e.g. in windy areas)")]
        public int MaxParallaxSpeed { get; set; } = 50;

        [SettingSubText("Removes/changes parallax during player movement. The setting above also applies!")]
        public ParallaxSettingValue ParallaxDuringMovement { get; set; } = ParallaxSettingValue.Standard;

        [SettingSubText("Show waterfalls in chapter 6?")]
        public bool ShowWaterfalls { get; set; } = true;

        [SettingSubText("Show tentacles (purple-ish veil covering half the screen) in chapter 6?")]
        public bool ShowTentacles { get; set; } = true;
        [SettingSubText("Show heat distortion in Core?")]
        public bool ShowHeatDistortion { get; set; } = true;

        [SettingSubText("This setting overrides all the individual settings below")]
        public OverrideAllValue OverrideAllBackdrops { get; set; } = OverrideAllValue.Disabled;

        // One setting for each backdrop available in the vanilla game
        [SettingSubHeader("Individual backdrop toggles")]
        public bool BlackholeBG { get; set; } = true;
        public bool CoreStarsFG { get; set; } = true;
        public bool DreamStars { get; set; } = true;
        [SettingSubText("Fast-moving background in chapter 6")]
        public bool FinalBossStarfield { get; set; } = true;
        public bool Godrays { get; set; } = true;
        public bool HeatWave { get; set; } = true;
        public bool MirrorFG { get; set; } = true;
        public bool NorthernLights { get; set; } = true;
        [SettingSubText("Background images, see README for more info")]
        public bool Parallax { get; set; } = true;
        public bool Petals { get; set; } = true;
        public bool Planets { get; set; } = true;
        public bool RainFG { get; set; } = true;
        public bool ReflectionFG { get; set; } = true;
        [SettingSubText("Snow mostly used in chapter 1")]
        public bool Snow { get; set; } = true;
        public bool StardustFG { get; set; } = true;
        public bool Starfield { get; set; } = true;
        public bool StarsBG { get; set; } = true;
        public bool Tentacles { get; set; } = true;
        [SettingSubText("Snow showing wind, used mostly in chapter 4")]
        public bool WindSnow { get; set; } = true;
    }
}
