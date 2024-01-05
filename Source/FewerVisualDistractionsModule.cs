using System;

namespace Celeste.Mod.FewerVisualDistractions {
    public class FewerVisualDistractionsModule : EverestModule {
        public static FewerVisualDistractionsModule Instance { get; private set; }

        public override Type SettingsType => typeof(FewerVisualDistractionsModuleSettings);
        public static FewerVisualDistractionsModuleSettings Settings => (FewerVisualDistractionsModuleSettings) Instance._Settings;

        public FewerVisualDistractionsModule() {
            Instance = this;
#if DEBUG
            Logger.SetLogLevel("FewerVisualDistractions", LogLevel.Verbose);
#else
            Logger.SetLogLevel(nameof(FewerVisualDistractionsModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            BackdropBlacklist.Load();
            DeathEffectTweaker.Load();
            ParallaxSpeedLimiter.Load();
            WaterfallHider.Load();

            Everest.Events.Level.OnLoadLevel += Level_OnLoadLevel;
        }

        public override void Unload() {
            BackdropBlacklist.Unload();
            DeathEffectTweaker.Unload();
            ParallaxSpeedLimiter.Unload();
            WaterfallHider.Unload();

            Everest.Events.Level.OnLoadLevel -= Level_OnLoadLevel;
        }
        private void Level_OnLoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            level.Add(new WindIndicator(level));
        }
    }
}
