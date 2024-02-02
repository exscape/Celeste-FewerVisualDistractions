using System.Linq;

namespace Celeste.Mod.FewerVisualDistractions;
using static FewerVisualDistractionsModuleSettings;

public static class BackdropBlacklist
{
    public static void Load()
    {
        On.Celeste.BackdropRenderer.Render += BackdropRenderer_Render;
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

        return backdrop switch
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
            _ => true
        };
    }

    public static void Unload()
    {
        On.Celeste.BackdropRenderer.Render -= BackdropRenderer_Render;
    }
}
