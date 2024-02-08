using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.FewerVisualDistractions;
using static FewerVisualDistractionsModuleSettings;

public static class BackdropBlacklist
{
    public static void Load()
    {
        On.Celeste.BackdropRenderer.Render += BackdropRenderer_Render;
        On.Celeste.Backdrop.ctor += Backdrop_ctor;
    }

    private static void Backdrop_ctor(On.Celeste.Backdrop.orig_ctor orig, Backdrop self)
    {
        orig(self);

        var type = self.GetType();

        if (type.Namespace == "Celeste")
            return;

        // This is a modded backdrop. Add it to our list of known mod backdrops, so that we can show a toggle for it in the menu
        FewerVisualDistractionsModule.Settings.AdditionalBackdrops.TryAdd(type.FullName, (type.Assembly.GetName().Name, true));
    }

    private static void BackdropRenderer_Render(On.Celeste.BackdropRenderer.orig_Render orig, BackdropRenderer self, Monocle.Scene scene)
    {
        var oldVisible = self.Backdrops.Select(b => b.Visible).ToList();
        self.Backdrops.ForEach(b => b.Visible = b.Visible && IsBackdropEnabled(b));

        orig(self, scene);

        for (int i = 0; i < oldVisible.Count; i++)
        {
            self.Backdrops[i].Visible = oldVisible[i];
        }
    }

    private static bool IsBackdropEnabled(Backdrop backdrop)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.OverrideAllBackdrops == OverrideAllValue.ShowAll)
            return true;
        if (FewerVisualDistractionsModule.Settings.OverrideAllBackdrops == OverrideAllValue.HideAll)
            return false;

        bool? shouldDisplay = backdrop switch
        {
            BlackholeBG => FewerVisualDistractionsModule.Settings.BlackholeBG,
            CoreStarsFG => FewerVisualDistractionsModule.Settings.CoreStarsFG,
            DreamStars => FewerVisualDistractionsModule.Settings.DreamStars,
            FinalBossStarfield => FewerVisualDistractionsModule.Settings.FinalBossStarfield,
            Godrays => FewerVisualDistractionsModule.Settings.Godrays,
            HeatWave => FewerVisualDistractionsModule.Settings.HeatWave,
            MirrorFG => FewerVisualDistractionsModule.Settings.MirrorFG,
            NorthernLights => FewerVisualDistractionsModule.Settings.NorthernLights,
            Parallax => FewerVisualDistractionsModule.Settings.Parallax,
            Petals => FewerVisualDistractionsModule.Settings.Petals,
            Planets => FewerVisualDistractionsModule.Settings.Planets,
            RainFG => FewerVisualDistractionsModule.Settings.RainFG,
            ReflectionFG => FewerVisualDistractionsModule.Settings.ReflectionFG,
            Snow => FewerVisualDistractionsModule.Settings.Snow,
            StardustFG => FewerVisualDistractionsModule.Settings.StardustFG,
            Starfield => FewerVisualDistractionsModule.Settings.Starfield,
            StarsBG => FewerVisualDistractionsModule.Settings.StarsBG,
            Tentacles => FewerVisualDistractionsModule.Settings.Tentacles,
            WindSnowFG => FewerVisualDistractionsModule.Settings.WindSnow,
            _ => null
        };

        if (shouldDisplay.HasValue)
            return shouldDisplay.Value;

        // If we're still here, the backdrop isn't one available in the stock game.
        // We should have a value for this in the settings, but if we don't, display the backdrop as a default.
        return FewerVisualDistractionsModule.Settings.AdditionalBackdrops.GetValueOrDefault(backdrop.GetType().FullName, (null, true)).Item2;
    }

    public static void Unload()
    {
        On.Celeste.BackdropRenderer.Render -= BackdropRenderer_Render;
        On.Celeste.Backdrop.ctor -= Backdrop_ctor;
    }
}
