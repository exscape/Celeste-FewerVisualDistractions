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
    }

    private static bool ShouldRenderHeatWaveDisplacement() => FewerVisualDistractionsModule.Settings.ShowHeatDistortion;
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
        if (FewerVisualDistractionsModule.Settings.ShowTentacles)
            orig(self);
    }

    public static bool ShouldDrawWaterfalls() => FewerVisualDistractionsModule.Settings.ShowWaterfalls;
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
    }
}
