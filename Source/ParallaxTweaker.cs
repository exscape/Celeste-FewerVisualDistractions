using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.FewerVisualDistractions;
public static class ParallaxTweaker
{
    private static ILHook parallaxOrigRenderHook;
    public static void Load()
    {
        // Remove background movement due to wind (especially in chapter 4); this does not affect the parallax effect as the player moves
        On.Celeste.Parallax.Update += Parallax_Update;

        // Tweak/lock background movement when the player moves (Everest method)
        IL.Celeste.Parallax.Render += patch_Parallax_Render;

        // Tweak/lock background movement when the player moves (game original method)
        parallaxOrigRenderHook = new(
            typeof(Parallax).GetMethod("orig_Render", BindingFlags.Public | BindingFlags.Instance), patch_Parallax_orig_Render);

        // Lock parallax in dream blocks
        IL.Celeste.DreamBlock.Render += patch_DreamBlock_Render;
    }

    private static void Parallax_Update(On.Celeste.Parallax.orig_Update orig, Parallax self, Scene scene)
    {
        // Null out the movement caused by the original Parallax.Update() (which hasn't been called yet, but that doesn't matter)
        Vector2 parallaxMovement = self.Speed * Engine.DeltaTime;
        Vector2 windMovement = self.WindMultiplier * (scene as Level).Wind * Engine.DeltaTime;
        Vector2 totalMovement = parallaxMovement + windMovement;
        self.Position -= totalMovement;

        // Add back the clamped amounts
        var maxMovement = FewerVisualDistractionsModule.Settings.MaxParallaxSpeed * Engine.DeltaTime;
        self.Position.X += (float)Math.CopySign(Math.Min(Math.Abs(totalMovement.X), maxMovement), totalMovement.X);
        self.Position.Y += (float)Math.CopySign(Math.Min(Math.Abs(totalMovement.Y), maxMovement), totalMovement.Y);

        orig(self, scene);
    }

    public static Vector2 ReplaceParallaxScrollVector(Vector2 scroll)
    {
        if (FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == FewerVisualDistractionsModuleSettings.ParallaxSettingValue.Standard)
            return scroll;
        else if (FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == FewerVisualDistractionsModuleSettings.ParallaxSettingValue.Locked)
            return Vector2.Zero;

        // We want a vector with 1 for each nonzero component.
        // Just in case a negative value is used elsewhere, copy the sign too.
        // To have this truly follow the camera we'd use Vector2.One, but the backgrounds aren't designed for that, and so we'll quickly run out of visible pixels (usually in the Y direction).
        return new Vector2(
            (float)Math.CopySign(scroll.X == 0f ? 0f : 1f, scroll.X),
            (float)Math.CopySign(scroll.Y == 0f ? 0f : 1f, scroll.Y));
    }

    private static void patch_Parallax_orig_Render(ILContext il)
    {
        // This patch removes the parallax effect when the player moves, but does not remove the parallax effect due to wind (especially in chapter 4)
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<Backdrop>("Scroll")
        ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find CIL sequence to hook in Parallax.orig_Render!");
            return;
        }

        // Jump past the loading of this.Scroll and insert our delegate to transform it
        cursor.Index += 2;
        cursor.EmitDelegate(ReplaceParallaxScrollVector);
    }

    private static void patch_Parallax_Render(ILContext il)
    {
        // Apply the same patch to Everest's modded Parallax.Render, used for some modded maps
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<Backdrop>("Scroll")
        ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find CIL sequence to hook in Parallax.Render (Everest patched version)!");
            return;
        }

        // Jump past the loading of this.Scroll and insert our delegate to transform it
        cursor.Index += 2;
        cursor.EmitDelegate(ReplaceParallaxScrollVector);
    }

    private static bool DreamBlockParallaxLocked() => FewerVisualDistractionsModule.Settings.LockDreamBlockParallax;
    private static void patch_DreamBlock_Render(ILContext il)
    {
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdloc(4),
            instr => instr.MatchLdloc(1)
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find DreamBlock.Render CIL sequence to hook!");
            return;
        }

        ILCursor jumpTarget = cursor.Clone();

        if (!jumpTarget.TryGotoNext(
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdloc(4),
            instr => instr.MatchCallvirt<DreamBlock>("PutInside")
            ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find DreamBlock.Render CIL sequence for jump target!");
            return;
        }

        cursor.EmitDelegate(DreamBlockParallaxLocked);
        cursor.Emit(OpCodes.Brtrue, jumpTarget.Next);
    }

    public static void Unload()
    {
        On.Celeste.Parallax.Update -= Parallax_Update;
        IL.Celeste.Parallax.Render -= patch_Parallax_Render; parallaxOrigRenderHook.Undo();
        IL.Celeste.DreamBlock.Render -= patch_DreamBlock_Render;
        parallaxOrigRenderHook?.Undo();
        parallaxOrigRenderHook?.Dispose();
        parallaxOrigRenderHook = null;
    }
}
