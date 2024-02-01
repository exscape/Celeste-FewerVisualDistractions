using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.FewerVisualDistractions;

public static class AdditionalEffectHider
{
    public static void Load()
    {
        // Remove waterfalls
        On.Celeste.WaterFall.Render += WaterFall_Render;
        On.Celeste.BigWaterfall.Render += BigWaterfall_Render;

        // Remove the warping effect shown behind the waterfalls
        On.Celeste.WaterFall.RenderDisplacement += WaterFall_RenderDisplacement;
        On.Celeste.BigWaterfall.RenderDisplacement += BigWaterfall_RenderDisplacement;

        // Remove the ripples on the water where the waterfall ends -- the game doesn't do this for the BigWaterfall class
        IL.Celeste.WaterFall.Update += patch_WaterFall_Update;

        // Remove chapter 6 tentacles (the veil that hides about half the screen until you get close)
        On.Celeste.ReflectionTentacles.Render += ReflectionTentacles_Render;

        // Remove the distortion effect in Core (when heat is active) -- also see the Heat Wave backdrop,
        // which can be toggled independently
        IL.Celeste.DisplacementRenderer.BeforeRender += patch_DisplacementRenderer_BeforeRender;

        // Remove cloud movement and snow in the PICO-8 version of Celeste
        IL.Celeste.Pico8.Classic.Draw += patch_Classic_Draw;

        // Modify amount of wind snow (0-100%) rendered; same for Stardust which is basically the same thing but colorful
        IL.Celeste.WindSnowFG.Render += patch_WindSnowFG_Render;
        IL.Celeste.StardustFG.Render += patch_StardustFG_Render;
    }


    private static int ReplaceWindSnowAmount(int num)
    {
        if (FewerVisualDistractionsModule.Settings.ModEnabled)
            return (int)(num * FewerVisualDistractionsModule.Settings.WindSnowAndStardustAmount / 100f);
        else
            return num;
    }

    private static void patch_WindSnowFG_Render(ILContext il)
    {
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<WindSnowFG>("positions"),
            instr => instr.MatchStloc(3)
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find WindSnowFG.Render CIL sequence to hook!");
            return;
        }

        cursor.Emit(OpCodes.Ldloc_1);
        cursor.EmitDelegate(ReplaceWindSnowAmount);
        cursor.Emit(OpCodes.Stloc_1);
    }

    private static void patch_StardustFG_Render(ILContext il)
    {
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<StardustFG>("particles"),
            instr => instr.MatchLdlen(),
            instr => instr.MatchConvI4()
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find StardustFG.Render CIL sequence to hook!");
            return;
        }

        cursor.EmitDelegate(ReplaceWindSnowAmount);
    }

    public static bool ShouldAnimatePico8Clouds() => !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.Pico8CloudMovement;
    public static bool ShouldRenderPico8Snow() => !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.ShowPico8Snow;

    private static void patch_Classic_Draw(ILContext il)
    {
        ILCursor cursor = new(il);

        // First patch: remove cloud movement

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdloc(4),
            instr => instr.MatchDup(),
            instr => instr.MatchLdfld<Pico8.Classic.Cloud>("x")
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find Pico8.Classic.Draw CIL sequence to hook for cloud movement removal!");
            return;
        }

        ILCursor afterLine = cursor.Clone();

        if (!afterLine.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<Pico8.Classic>("E")
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find Pico8.Classic.Draw jump target for cloud movement removal!");
            return;
        }

        cursor.EmitDelegate(ShouldAnimatePico8Clouds);
        cursor.Emit(OpCodes.Brfalse, afterLine.Next);

        // Second patch: remove snow particles

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<Pico8.Classic>("E"),
            instr => instr.MatchLdloc(10),
            instr => instr.MatchLdfld<Pico8.Classic.Particle>("x")
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find Pico8.Classic.Draw CIL sequence to hook for particle removal!");
            return;
        }

        afterLine = cursor.Clone();

        if (!afterLine.TryGotoNext(instr => instr.MatchCallvirt<Pico8.Emulator>("rectfill")))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find Pico8.Classic.Draw jump target for particle removal!");
            return;
        }

        afterLine.Index += 1;

        cursor.EmitDelegate(ShouldRenderPico8Snow);
        cursor.Emit(OpCodes.Brfalse, afterLine.Next);
    }

    private static bool ShouldRenderHeatWaveDisplacement() => !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.ShowHeatDistortion;
    private static void patch_DisplacementRenderer_BeforeRender(ILContext il)
    {
        ILCursor cursor = new(il);
        ILLabel afterIfStatement = default;

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdloc(2),
            instr => instr.MatchBrfalse(out afterIfStatement)
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find DisplacementRenderer.BeforeRender CIL sequence to hook!");
            return;
        }

        cursor.EmitDelegate(ShouldRenderHeatWaveDisplacement);
        cursor.Emit(OpCodes.Brfalse, afterIfStatement);
    }

    private static void WaterFall_Render(On.Celeste.WaterFall.orig_Render orig, WaterFall self)
    {
        if (ShouldDrawWaterfalls())
            orig(self);
    }
    private static void BigWaterfall_Render(On.Celeste.BigWaterfall.orig_Render orig, BigWaterfall self)
    {
        if (ShouldDrawWaterfalls())
            orig(self);
    }

    private static void WaterFall_RenderDisplacement(On.Celeste.WaterFall.orig_RenderDisplacement orig, WaterFall self)
    {
        if (ShouldDrawWaterfalls())
            orig(self);
    }

    private static void BigWaterfall_RenderDisplacement(On.Celeste.BigWaterfall.orig_RenderDisplacement orig, BigWaterfall self)
    {
        if (ShouldDrawWaterfalls())
            orig(self);
    }

    private static void ReflectionTentacles_Render(On.Celeste.ReflectionTentacles.orig_Render orig, ReflectionTentacles self)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.ShowTentacles)
            orig(self);
    }

    public static bool ShouldDrawWaterfalls() => !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.ShowWaterfalls;
    private static void patch_WaterFall_Update(ILContext il)
    {
        // Patch out most of Update if waterfalls are disabled; specifically, we jump over the two if statements,
        // leaving only the audio stuff and the base.Update() call
        
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<WaterFall>("water")
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find first Waterfall.Update CIL sequence to hook!");
            return;
        }

        ILCursor afterIfStatements = cursor.Clone();
        if (!afterIfStatements.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchCall<Entity>("Update")
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find second Waterfall.Update CIL sequence to hook!");
            return;
        }

        cursor.EmitDelegate(ShouldDrawWaterfalls);
        cursor.Emit(OpCodes.Brfalse, afterIfStatements.Next);
    }

    public static void Unload()
    {
        On.Celeste.WaterFall.Render -= WaterFall_Render;
        On.Celeste.BigWaterfall.Render -= BigWaterfall_Render;
        On.Celeste.WaterFall.RenderDisplacement -= WaterFall_RenderDisplacement;
        On.Celeste.BigWaterfall.RenderDisplacement -= BigWaterfall_RenderDisplacement;
        IL.Celeste.WaterFall.Update -= patch_WaterFall_Update;
        On.Celeste.ReflectionTentacles.Render -= ReflectionTentacles_Render;
        IL.Celeste.DisplacementRenderer.BeforeRender -= patch_DisplacementRenderer_BeforeRender;
        IL.Celeste.Pico8.Classic.Draw -= patch_Classic_Draw;
        IL.Celeste.WindSnowFG.Render -= patch_WindSnowFG_Render;
        IL.Celeste.StardustFG.Render -= patch_StardustFG_Render;
    }
}
