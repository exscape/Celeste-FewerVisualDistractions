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
            SingleColor,
            Standard
        }

        [SettingSubText("Allows for disabling screen wipes, e.g. when the level reloads after death")]
        public bool ScreenWipes { get; set; } = true;

        [SettingSubText("Changes the moving objects around the player as they die")]
        public DeathEffectSettingValue DeathEffect { get; set; } = DeathEffectSettingValue.Standard;

        // Used to make the IL patch easier
        [SettingIgnore]
        public bool SingleColorDeathEffect
        {
            get
            {
                return this.DeathEffect == DeathEffectSettingValue.SingleColor;
            }
        }

        [SettingSubText("Enable the warping effect around the player as they die?")]
        public bool DeathWarpEffect { get; set; } = true;

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
