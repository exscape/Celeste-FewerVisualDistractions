using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.FewerVisualDistractions.Patches;
using static FewerVisualDistractionsModuleSettings;

public static class DeathEffectTweaker
{
    private static ILHook deathRoutineHook;

    // Stored when screen wipes are disabled, yet a screen wipe has an onComplete action.
    // Executed after the current update/render cycle is completed.
    private static Action deferredScreenWipeAction = null;

    public static void Load()
    {
        // Tweak or remove the death effect (ring around the player)
        On.Celeste.DeathEffect.Draw += DeathEffect_Draw;
        IL.Celeste.DeathEffect.Draw += patch_DeathEffect_Draw;

        // Remove the displacement "burst" effect around the player
        deathRoutineHook = new(typeof(PlayerDeadBody).GetMethod("DeathRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(),
            patch_PlayerDeadBody_DeathRoutine);

        // Remove screen wipes
        On.Celeste.AreaData.DoScreenWipe += AreaData_DoScreenWipe;
        On.Monocle.Engine.Update += Engine_Update;
    }

    private static void AreaData_DoScreenWipe(On.Celeste.AreaData.orig_DoScreenWipe orig, AreaData self, Scene scene, bool wipeIn, Action onComplete)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.DeathEffects.ScreenWipes)
        {
            orig(self, scene, wipeIn, onComplete);
            return;
        }

        // Ignore the wipe, but run the onComplete action if it exists; see Engine_Update below.
        // Adds compatibility with Speedrun Tool save states (and perhaps other mods) that use level.DoScreenWipe to execute actions
        deferredScreenWipeAction = onComplete;
    }

    private static void Engine_Update(On.Monocle.Engine.orig_Update orig, Engine self, GameTime gameTime)
    {
        // Handle screen wipe onComplete actions that weren't executed because screen wipes are disabled
        deferredScreenWipeAction?.Invoke();
        deferredScreenWipeAction = null;
        orig(self, gameTime);
    }

    public static bool ShouldShowDeathWarpEffect() => !FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.DeathEffects.WarpingDeathEffect;
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
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.DeathEffects.RotatingDeathEffect != DeathEffectSettingValue.Hidden)
            orig(position, color, ease);
    }

    public static bool ShouldUseSingleColorDeathEffect() => FewerVisualDistractionsModule.Settings.ModEnabled && FewerVisualDistractionsModule.Settings.DeathEffects.RotatingDeathEffect == DeathEffectSettingValue.NoFlashes;
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
        On.Celeste.AreaData.DoScreenWipe -= AreaData_DoScreenWipe;
        On.Monocle.Engine.Update -= Engine_Update;
        IL.Celeste.DeathEffect.Draw -= patch_DeathEffect_Draw;
        On.Celeste.DeathEffect.Draw -= DeathEffect_Draw;
        deathRoutineHook?.Undo();
        deathRoutineHook?.Dispose();
        deathRoutineHook = null;
    }
}
