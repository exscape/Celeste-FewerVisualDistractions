using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.FewerVisualDistractions;
public static class BackdropBlacklist
{
    public static void Load()
    {
        IL.Celeste.BackdropRenderer.Render += BackdropRendererHook;
    }

    public static void Unload()
    {
        IL.Celeste.BackdropRenderer.Render -= BackdropRendererHook;
    }

    private static void BackdropRendererHook(ILContext il)
    {
        // Add a second condition to the renderer: render if .Visible *and* IsBackdropEnabled returns true
        ILCursor cursor = new(il);
        ILLabel loopEnd = default;

        if (!cursor.TryGotoNext(
          instr => instr.MatchLdloc(2),
          instr => instr.MatchLdfld<Backdrop>("Visible"),
          instr => instr.MatchBrfalse(out loopEnd)
        ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find BackdropRenderer.Render CIL sequence to hook!");
            return;
        }

        cursor.Emit(OpCodes.Ldloc_2);
        cursor.EmitDelegate(IsBackdropEnabled);
        cursor.Emit(OpCodes.Brfalse, loopEnd);
    }

    private static bool IsBackdropEnabled(Backdrop backdrop)
    {
        if (FewerVisualDistractionsModule.Settings.OverrideAllBackdrops == FewerVisualDistractionsModuleSettings.OverrideAllValue.HideAll)
            return false;
        else if (FewerVisualDistractionsModule.Settings.OverrideAllBackdrops == FewerVisualDistractionsModuleSettings.OverrideAllValue.ShowAll)
            return true;

        return backdrop switch {
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
            WindSnowFG => FewerVisualDistractionsModule.Settings.WindSnowFG,
            _ => true
        };
    }
}
