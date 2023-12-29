using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FewerVisualDistractions {
    public class FewerVisualDistractionsModule : EverestModule {
        public static FewerVisualDistractionsModule Instance { get; private set; }

        public override Type SettingsType => typeof(FewerVisualDistractionsModuleSettings);
        public static FewerVisualDistractionsModuleSettings Settings => (FewerVisualDistractionsModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(FewerVisualDistractionsModuleSession);
        public static FewerVisualDistractionsModuleSession Session => (FewerVisualDistractionsModuleSession) Instance._Session;

        public override Type SaveDataType => typeof(FewerVisualDistractionsModuleSaveData);
        public static FewerVisualDistractionsModuleSaveData SaveData => (FewerVisualDistractionsModuleSaveData) Instance._SaveData;

        public FewerVisualDistractionsModule() {
            Instance = this;
#if DEBUG
            Logger.SetLogLevel(nameof(FewerVisualDistractionsModule), LogLevel.Verbose);
#else
            Logger.SetLogLevel(nameof(BackdropHiderModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            BackdropBlacklist.Load();
            DeathEffectTweaker.Load();
            ParallaxSpeedLimiter.Load();

            Everest.Events.Level.OnLoadLevel += Level_OnLoadLevel;
        }

        private void Level_OnLoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            level.Add(new WindIndicator(level));
        }

        public override void Unload() {
            BackdropBlacklist.Unload();
            DeathEffectTweaker.Unload();
            ParallaxSpeedLimiter.Unload();

            Everest.Events.Level.OnLoadLevel -= Level_OnLoadLevel;
        }
    }
}
