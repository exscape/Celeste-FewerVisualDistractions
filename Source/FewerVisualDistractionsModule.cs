using System;

namespace Celeste.Mod.FewerVisualDistractions {
    public class FewerVisualDistractionsModule : EverestModule {
        public static FewerVisualDistractionsModule Instance { get; private set; }

        public override Type SettingsType => typeof(FewerVisualDistractionsModuleSettings);
        public static FewerVisualDistractionsModuleSettings Settings => (FewerVisualDistractionsModuleSettings) Instance._Settings;

        private static WindIndicator windIndicator = null;

        public FewerVisualDistractionsModule() {
            Instance = this;
#if DEBUG
            Logger.SetLogLevel("FewerVisualDistractions", LogLevel.Verbose);
#else
            Logger.SetLogLevel("FewerVisualDistractions", LogLevel.Info);
#endif
        }

        public override void Load() {
            BackdropBlacklist.Load();
            DeathEffectTweaker.Load();
            ParallaxTweaker.Load();
            AdditionalEffectHider.Load();
            FarewellTweaker.Load();

            Everest.Events.Level.OnLoadLevel += Level_OnLoadLevel;
        }

        public override void Unload() {
            BackdropBlacklist.Unload();
            DeathEffectTweaker.Unload();
            ParallaxTweaker.Unload();
            AdditionalEffectHider.Unload();
            FarewellTweaker.Unload();

            Everest.Events.Level.OnLoadLevel -= Level_OnLoadLevel;
        }
        private void Level_OnLoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (windIndicator == null)
                windIndicator = new WindIndicator(level);
            else
                windIndicator.SwitchLevel(level);

            level.Add(windIndicator);
        }
    }
}
