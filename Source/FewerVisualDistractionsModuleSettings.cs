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

        [SettingSubText("Show a text indicator when there is wind. Useful if WindSnow is disabled below")]
        public bool ShowWindIndicator { get; set; } = false;

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

        [SettingSubText("Show waterfalls in chapter 6?")]
        public bool ShowWaterfalls { get; set; } = true;

        [SettingSubText("This setting overrides all the individual settings below")]
        public OverrideAllValue OverrideAllBackdrops { get; set; } = OverrideAllValue.Disabled;

        // One setting for each backdrop available in the vanilla game
        [SettingSubHeader("Individual backdrop toggles")]
        public bool BlackholeBG { get; set; } = true;
        public bool CoreStarsFG { get; set; } = true;
        public bool DreamStars { get; set; } = true;
        public bool FinalBossStarfield { get; set; } = true;
        public bool Godrays { get; set; } = true;
        public bool HeatWave { get; set; } = true;
        public bool MirrorFG { get; set; } = true;
        public bool NorthernLights { get; set; } = true;
        public bool Parallax { get; set; } = true;
        public bool Petals { get; set; } = true;
        public bool Planets { get; set; } = true;
        public bool RainFG { get; set; } = true;
        public bool ReflectionFG { get; set; } = true;
        public bool Snow { get; set; } = true;
        public bool StardustFG { get; set; } = true;
        public bool Starfield { get; set; } = true;
        public bool StarsBG { get; set; } = true;
        public bool Tentacles { get; set; } = true;
        public bool WindSnowFG { get; set; } = true;
    }
}
