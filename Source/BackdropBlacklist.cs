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

        if (!cursor.TryGotoNext(instr => instr.MatchLdfld<Backdrop>("Visible")))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find BackdropRenderer.Render CIL sequence to hook!");
            return;
        }

        // Split to enable compatibility with the Maddie's Helping Hand (previously MaxHelpingHand) mod.
        // ParallaxFadeOutController.cs in that mod also patches on Visible, but by allowing for code between ldfld and brfalse,
        // which are next to each other in vanilla, both mods can do their thing
        ILCursor branch = cursor.Clone();
        if (!branch.TryGotoNext(instr => instr.MatchBrfalse(out loopEnd)))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find BackdropRenderer.Render branch!");
            return;
        }

        branch.Index += 1;
        branch.Emit(OpCodes.Ldloc_2);
        branch.EmitDelegate(IsBackdropEnabled);
        branch.Emit(OpCodes.Brfalse, loopEnd);
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
            WindSnowFG => FewerVisualDistractionsModule.Settings.WindSnow,
            _ => true
        };
    }
}
