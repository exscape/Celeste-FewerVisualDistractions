using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.BackdropHider {
    public class BackdropHiderModule : EverestModule {
        public static BackdropHiderModule Instance { get; private set; }

        public override Type SettingsType => typeof(BackdropHiderModuleSettings);
        public static BackdropHiderModuleSettings Settings => (BackdropHiderModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(BackdropHiderModuleSession);
        public static BackdropHiderModuleSession Session => (BackdropHiderModuleSession) Instance._Session;

        public override Type SaveDataType => typeof(BackdropHiderModuleSaveData);
        public static BackdropHiderModuleSaveData SaveData => (BackdropHiderModuleSaveData) Instance._SaveData;

        public BackdropHiderModule() {
            Instance = this;
#if DEBUG
            Logger.SetLogLevel(nameof(BackdropHiderModule), LogLevel.Verbose);
#else
            Logger.SetLogLevel(nameof(BackdropHiderModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            // TODO: apply any hooks that should always be active
            BackdropBlacklist.Load();
            ParallaxSpeedLimiter.Load();
        }

        public override void Unload() {
            // TODO: unapply any hooks applied in Load()
            BackdropBlacklist.Unload();
            ParallaxSpeedLimiter.Unload();
        }
    }
}
