using Celeste.Pico8;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.FewerVisualDistractions.Patches;

public static class AdditionalEffectTweaker
{
    private static List<float> origCloudSpeeds;
    private static List<Pico8.Classic.Particle> origParticles;

    private static ILHook blinkingHairHook;

    public static void Load()
    {
        // Remove waterfalls
        On.Celeste.WaterFall.Render += WaterFall_Render;
        On.Celeste.BigWaterfall.Render += BigWaterfall_Render;

        // Remove the warping effect shown behind the waterfalls
        On.Celeste.WaterFall.RenderDisplacement += WaterFall_RenderDisplacement;
        On.Celeste.BigWaterfall.RenderDisplacement += BigWaterfall_RenderDisplacement;

        // Remove the ripples on the water where the waterfall ends -- the game doesn't do this for the BigWaterfall class
        On.Celeste.WaterFall.Update += WaterFall_Update;

        // Remove chapter 6 tentacles (the veil that hides about half the screen until you get close)
        On.Celeste.ReflectionTentacles.Render += ReflectionTentacles_Render;

        // Remove the distortion effect in Core (when heat is active) -- also see the Heat Wave backdrop,
        // which can be toggled independently
        On.Celeste.HeatWave.RenderDisplacement += HeatWave_RenderDisplacement;

        // Remove cloud movement and snow in the PICO-8 version of Celeste
        On.Celeste.Pico8.Classic.Init += Classic_Init;
        On.Celeste.Pico8.Classic.Update += Classic_Update;

        // Remove hair blanking (when double dash is available) in the PICO-8 version of Celeste
        blinkingHairHook = new(typeof(Classic.player_hair).GetMethod("draw_hair", BindingFlags.Public | BindingFlags.Instance), patch_draw_hair);
        IL.Celeste.Pico8.Classic.draw_player += patch_draw_player;

        // Modify amount of wind snow (0-100%) rendered; same for Stardust which is basically the same thing but colorful
        IL.Celeste.WindSnowFG.Render += patch_WindSnowFG_Render;
        IL.Celeste.StardustFG.Render += patch_StardustFG_Render;
    }

    private static int ReplaceWindSnowAmount(int num)
    {
        if (FewerVisualDistractionsModule.Settings.ModEnabled)
            return (int)(num * FewerVisualDistractionsModule.Settings.Wind.WindSnowAndStardustAmount / 100f);
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

    private static void Classic_Init(On.Celeste.Pico8.Classic.orig_Init orig, Pico8.Classic self, Pico8.Emulator emulator)
    {
        orig(self, emulator);
        origCloudSpeeds = self.clouds.Select(c => c.spd).ToList();
        origParticles = new(self.particles);
    }

    private static void Classic_Update(On.Celeste.Pico8.Classic.orig_Update orig, Pico8.Classic self)
    {
        bool shouldAnimateClouds = !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.VariousEffects.Pico8CloudMovement;
        bool shouldRenderSnow = !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.VariousEffects.ShowPico8Snow;

        if (!shouldRenderSnow && self.particles.Count > 0)
            self.particles.Clear();
        else if (shouldRenderSnow && self.particles.Count == 0)
            self.particles = new(origParticles);

        if (!shouldAnimateClouds && self.clouds.Count > 0 && self.clouds[0].spd != 0)
            self.clouds.ForEach(c => c.spd = 0);
        else if (shouldAnimateClouds && self.clouds.Count > 0 && self.clouds[0].spd == 0)
        {
            for (int i = 0; i < self.clouds.Count; i++)
                self.clouds[i].spd = origCloudSpeeds[i];
        }

        orig(self);
    }

    public static bool ShouldUseSingleColorHair() => FewerVisualDistractionsModule.Settings.ModEnabled && !FewerVisualDistractionsModule.Settings.VariousEffects.Pico8BlinkingHair;

    private static int GetHairColor(int djump)
    {
        if (djump == 0) return 12; // Blue
        else if (djump == 1) return 8; // Red
        else return 11; // Green
    }

    private static void patch_draw_hair(ILContext il)
    {
        ILCursor cursor = new(il);
        ILCursor beforeOriginalCode = cursor.Clone();
        ILCursor afterOriginalCode = cursor.Clone();
        if (!afterOriginalCode.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(8),
            instr => instr.MatchStloc(0)))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find CIL sequence to hook for draw_hair (PICO-8)!");
            return;
        }

        cursor.EmitDelegate(ShouldUseSingleColorHair);
        cursor.Emit(OpCodes.Brfalse, beforeOriginalCode.Next);

        // Load the proper hair color, depending on the djump argument
        cursor.Emit(OpCodes.Ldarg_3);
        cursor.EmitDelegate(GetHairColor);
        cursor.Emit(OpCodes.Stloc_0);
        cursor.Emit(OpCodes.Br, afterOriginalCode.Next);
    }

    private static void patch_draw_player(ILContext il)
    {
        ILCursor cursor = new(il);
        if (!cursor.TryGotoNext(
            instr => instr.MatchLdcI4(160) // Load player with white hair
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find CIL sequence to hook for draw_player (PICO-8)!");
            return;
        }

        // This patch works pretty much the same way as the one in DeathEffectTweaker.cs (patch_DeathEffect_Draw)
        // If djump == 2 i.e. we can dash twice, possibly replace the color calculation (blinking) with our own (static)

        ILCursor originalCode = cursor.Clone();
        ILCursor afterOriginalCode = cursor.Clone();
        afterOriginalCode.Index += 2;

        // Check the setting, and skip to the original code if the setting is disabled
        cursor.EmitDelegate(ShouldUseSingleColorHair);
        cursor.Emit(OpCodes.Brfalse, originalCode.Next);

        // Add in our replacement code
        cursor.Emit(OpCodes.Ldc_I4, 144); // Player with green hair
        cursor.Emit(OpCodes.Stloc_0);
        cursor.Emit(OpCodes.Br, afterOriginalCode.Next);
    }

    private static void HeatWave_RenderDisplacement(On.Celeste.HeatWave.orig_RenderDisplacement orig, HeatWave self, Level level)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.VariousEffects.ShowHeatDistortion)
            orig(self, level);
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
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.VariousEffects.ShowTentacles)
            orig(self);
    }

    public static bool ShouldDrawWaterfalls() => !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.VariousEffects.ShowWaterfalls;

    private static void WaterFall_Update(On.Celeste.WaterFall.orig_Update orig, WaterFall self)
    {
        var oldWater = self.water;

        // Skip both if statements in the original method; both require this.water != null, so this is easy!
        if (!ShouldDrawWaterfalls())
            self.water = null;

        orig(self);

        if (!ShouldDrawWaterfalls())
            self.water = oldWater;
    }

    public static void Unload()
    {
        On.Celeste.WaterFall.Render -= WaterFall_Render;
        On.Celeste.BigWaterfall.Render -= BigWaterfall_Render;
        On.Celeste.WaterFall.RenderDisplacement -= WaterFall_RenderDisplacement;
        On.Celeste.BigWaterfall.RenderDisplacement -= BigWaterfall_RenderDisplacement;
        On.Celeste.WaterFall.Update -= WaterFall_Update;
        On.Celeste.ReflectionTentacles.Render -= ReflectionTentacles_Render;
        On.Celeste.HeatWave.RenderDisplacement -= HeatWave_RenderDisplacement;
        On.Celeste.Pico8.Classic.Init -= Classic_Init;
        On.Celeste.Pico8.Classic.Update -= Classic_Update;
        IL.Celeste.WindSnowFG.Render -= patch_WindSnowFG_Render;
        IL.Celeste.StardustFG.Render -= patch_StardustFG_Render;
        blinkingHairHook?.Undo();
        blinkingHairHook?.Dispose();
        blinkingHairHook = null;
        IL.Celeste.Pico8.Classic.draw_player -= patch_draw_player;
    }
}
