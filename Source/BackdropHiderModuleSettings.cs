namespace Celeste.Mod.BackdropHider {
    public class BackdropHiderModuleSettings : EverestModuleSettings {

        public enum OverrideAllValue
        {
            Disabled,
            HideAll,
            ShowAll
        }

        [SettingSubText("Useful if WindSnow is disabled below")]
        public bool ShowWindIndicator { get; set; } = false;

        [SettingRange(0, 100, true)]
        [SettingSubText("Limits the speed of the background parallax (e.g. in windy areas)")]
        public int MaxParallaxSpeed { get; set; } = 100;

        [SettingSubText("Overrides the individual settings below.")]
        public OverrideAllValue OverrideAllBackdrops { get; set; } = OverrideAllValue.Disabled;
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