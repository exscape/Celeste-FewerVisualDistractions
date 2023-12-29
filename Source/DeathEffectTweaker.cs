using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.FewerVisualDistractions;
public static class DeathEffectTweaker
{
    public static ILHook deathRoutineHook = null;
    public static void Load()
    {
        // Easier than adding a second condition via an IL patch. 
        On.Celeste.DeathEffect.Draw += DeathEffect_Draw;
        IL.Celeste.DeathEffect.Draw += patch_DeathEffect_Draw;

        // PlayerDeadBody.DeathRoutine is a coroutine, so we can't use the IL.Celeste... += method
        deathRoutineHook = new(typeof(PlayerDeadBody).GetMethod("DeathRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(),
            patch_PlayerDeadBody_DeathRoutine);
    }

    private static void patch_PlayerDeadBody_DeathRoutine(ILContext il)
    {
        // Add the option to hide the "burst" effect that warps the area around the player as the player dies

        ILCursor cursor = new(il);
        float f;
        if (!cursor.TryGotoNext(
            instr => instr.MatchLdcR4(out f),
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
        cursor.Index -= 5;
        ILCursor originalCode = cursor.Clone();
        ILCursor afterOriginalCode = cursor.Clone();
        afterOriginalCode.Index += 13;

        cursor.Emit(OpCodes.Call, typeof(FewerVisualDistractionsModule).GetMethod("get_Settings"));
        cursor.Emit(OpCodes.Callvirt, typeof(FewerVisualDistractionsModuleSettings).GetMethod("get_DeathWarpEffect"));

        // Jump past the original code if we should hide the effect
        cursor.Emit(OpCodes.Brfalse, afterOriginalCode.Next);
    }

    private static void DeathEffect_Draw(On.Celeste.DeathEffect.orig_Draw orig, Vector2 position, Color color, float ease)
    {
        if (FewerVisualDistractionsModule.Settings.DeathEffect == FewerVisualDistractionsModuleSettings.DeathEffectSettingValue.Hidden)
            return;

        orig(position, color, ease);
    }

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
        if (BackdropHiderModule.Settings.DeathEffect == BackdropHiderModuleSettings.DeathEffectSettingValue.SingleColor)
            color2 = color;
        else // This line is the original code, 13 instructions long
            color2 = (Math.Floor((double)(ease * 10f)) % 2.0 == 0.0) ? color : Color.White;
         */

        ILCursor originalCode = cursor.Clone();
        ILCursor afterOriginalCode = cursor.Clone();
        afterOriginalCode.Index += 13;

        // First, load the condition onto the stack, so that we can check whether we should change the effect or not
        cursor.Emit(OpCodes.Call, typeof(FewerVisualDistractionsModule).GetMethod("get_Settings"));
        cursor.Emit(OpCodes.Callvirt, typeof(FewerVisualDistractionsModuleSettings).GetMethod("get_SingleColorDeathEffect"));

        // Next, jump to the original code if the setting is disabled
        cursor.Emit(OpCodes.Brfalse, originalCode.Next);

        // Add in our replacement code, that simply uses the color from the method argument all the time
        cursor.Emit(OpCodes.Ldarg_1);
        cursor.Emit(OpCodes.Stloc_0);
        cursor.Emit(OpCodes.Br, afterOriginalCode.Next);
    }

    public static void Unload()
    {
        IL.Celeste.DeathEffect.Draw -= patch_DeathEffect_Draw;
        On.Celeste.DeathEffect.Draw -= DeathEffect_Draw;
        deathRoutineHook?.Dispose();
        deathRoutineHook = null;
    }
}
