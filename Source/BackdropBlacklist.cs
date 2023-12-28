using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.BackdropHider;
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
        // foreach (Backdrop backdrop in this.Backdrops)
        // {
        //+  if (!IsBackdropEnabled(backdrop))
        //+    continue;    
        //   if (backdrop.Visible)

        ILCursor cursor = new(il);
        ILLabel loopEnd = default;
        if (!cursor.TryGotoNext(
          instr => instr.MatchLdloc(2),
          instr => instr.MatchLdfld<Backdrop>("Visible"),
          instr => instr.MatchBrfalse(out loopEnd)
        ))
        {
            Logger.Log(LogLevel.Error, "BackdropHider", "Couldn't find BackdropRenderer.Render CIL sequence to hook!");
            return;
        }
        cursor.Emit(OpCodes.Ldloc_2);
        cursor.EmitDelegate(IsBackdropEnabled);
        cursor.Emit(OpCodes.Brfalse, loopEnd);
    }

    private static bool IsBackdropEnabled(Backdrop backdrop)
    {
        if (BackdropHiderModule.Settings.OverrideAllBackdrops == BackdropHiderModuleSettings.OverrideAllValue.HideAll)
            return false;
        else if (BackdropHiderModule.Settings.OverrideAllBackdrops == BackdropHiderModuleSettings.OverrideAllValue.ShowAll)
            return true;

        return backdrop switch {
            BlackholeBG => BackdropHiderModule.Settings.BlackholeBG,
            CoreStarsFG => BackdropHiderModule.Settings.CoreStarsFG,
            DreamStars => BackdropHiderModule.Settings.DreamStars,
            FinalBossStarfield => BackdropHiderModule.Settings.FinalBossStarfield,
            Godrays => BackdropHiderModule.Settings.Godrays,
            HeatWave => BackdropHiderModule.Settings.HeatWave,
            MirrorFG => BackdropHiderModule.Settings.MirrorFG,
            NorthernLights => BackdropHiderModule.Settings.NorthernLights,
            Parallax => BackdropHiderModule.Settings.Parallax,
            Petals => BackdropHiderModule.Settings.Petals,
            Planets => BackdropHiderModule.Settings.Planets,
            RainFG => BackdropHiderModule.Settings.RainFG,
            ReflectionFG => BackdropHiderModule.Settings.ReflectionFG,
            Snow => BackdropHiderModule.Settings.Snow,
            StardustFG => BackdropHiderModule.Settings.StardustFG,
            Starfield => BackdropHiderModule.Settings.Starfield,
            StarsBG => BackdropHiderModule.Settings.StarsBG,
            Tentacles => BackdropHiderModule.Settings.Tentacles,
            WindSnowFG => BackdropHiderModule.Settings.WindSnowFG,
            _ => true
        };
    }
}