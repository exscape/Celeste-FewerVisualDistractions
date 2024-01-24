using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.FewerVisualDistractions;
using static FewerVisualDistractionsModuleSettings;

public static class DeathEffectTweaker
{
    private static ILHook deathRoutineHook;
    public static void Load()
    {
        // Tweak or remove the death effect (ring around the player)
        On.Celeste.DeathEffect.Draw += DeathEffect_Draw;
        IL.Celeste.DeathEffect.Draw += patch_DeathEffect_Draw;

        // Remove the displacement "burst" effect around the player
        deathRoutineHook = new(typeof(PlayerDeadBody).GetMethod("DeathRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(),
            patch_PlayerDeadBody_DeathRoutine);

        // Remove screen wipes
        IL.Celeste.Level.Render += patch_Level_Render;
    }

    //  Borrowed from CelesteTAS
    private static ScreenWipe ReplaceScreenWipe(ScreenWipe wipe) => FewerVisualDistractionsModule.Settings.ScreenWipes ? wipe : null;
    private static void patch_Level_Render(ILContext il)
    {
        ILCursor ilCursor = new(il);
        if (ilCursor.TryGotoNext(i => i.MatchLdarg(0), i => i.MatchLdfld<Level>("Wipe"), i => i.OpCode == OpCodes.Brfalse_S))
        {
            ilCursor.Index += 2;
            ilCursor.EmitDelegate(ReplaceScreenWipe);
        }
    }

    public static bool ShouldShowDeathWarpEffect() => FewerVisualDistractionsModule.Settings.WarpingDeathEffect;
    private static void patch_PlayerDeadBody_DeathRoutine(ILContext il)
    {
        ILCursor cursor = new(il);
        if (!cursor.TryGotoNext(
            instr => instr.MatchLdcR4(0),
            instr => instr.MatchLdcR4(80),
            instr => instr.MatchLdcR4(1),
            instr => instr.MatchLdnull(),
            instr => instr.MatchLdnull()
           ))
        {
            Logger.Log(LogLevel.Error, "DeathEffectTweaker", "Couldn't find CIL sequence to hook for PlayerDeadBody.DeathRoutine!");
            return;
        }

        // Jump back to the start; the first LdcR4 instruction is the sixth in the C# line we want to skip
        cursor.Index -= 6;
        ILCursor afterOriginalCode = cursor.Clone();
        afterOriginalCode.Index += 13;

        // Jump over the call to level.Displacement.AddBurst() that creates the warp effect
        cursor.EmitDelegate(ShouldShowDeathWarpEffect);
        cursor.Emit(OpCodes.Brfalse, afterOriginalCode.Next);
    }

    private static void DeathEffect_Draw(On.Celeste.DeathEffect.orig_Draw orig, Vector2 position, Color color, float ease)
    {
        if (FewerVisualDistractionsModule.Settings.RotatingDeathEffect != DeathEffectSettingValue.Hidden)
            orig(position, color, ease);
    }

    public static bool ShouldUseSingleColorDeathEffect() => FewerVisualDistractionsModule.Settings.RotatingDeathEffect == DeathEffectSettingValue.NoFlashes;
    private static void patch_DeathEffect_Draw(ILContext il)
    {
        ILCursor cursor = new(il);
        if (!cursor.TryGotoNext(
            instr => instr.MatchLdarg(2),
            instr => instr.MatchLdcR4(10),
            instr => instr.MatchMul(),
            instr => instr.MatchConvR8()
            ))
        {
            Logger.Log(LogLevel.Error, "DeathEffectTweaker", "Couldn't find CIL sequence to hook for DeathEffect.Draw!");
            return;
        }

        /*
        // Patch to something along the lines of
        Color color2;
        if (FewerVisualDistractionsModule.Settings.DeathEffect == DeathEffectSettingValue.SingleColor)
            color2 = color;
        else // This line is the original code, 13 instructions long
            color2 = (Math.Floor((double)(ease * 10f)) % 2.0 == 0.0) ? color : Color.White;
         */

        ILCursor originalCode = cursor.Clone();
        ILCursor afterOriginalCode = cursor.Clone();
        afterOriginalCode.Index += 13;

        // Check the setting, and skip to the original code if the setting is disabled
        cursor.EmitDelegate(ShouldUseSingleColorDeathEffect);
        cursor.Emit(OpCodes.Brfalse, originalCode.Next);

        // Add in our replacement code, that simply uses the color from the method argument all the time (and thus removing the white flashes)
        cursor.Emit(OpCodes.Ldarg_1);
        cursor.Emit(OpCodes.Stloc_0);
        cursor.Emit(OpCodes.Br, afterOriginalCode.Next);
    }

    public static void Unload()
    {
        IL.Celeste.DeathEffect.Draw -= patch_DeathEffect_Draw;
        On.Celeste.DeathEffect.Draw -= DeathEffect_Draw;
        deathRoutineHook?.Undo();
        deathRoutineHook?.Dispose();
        deathRoutineHook = null;
    }
}
