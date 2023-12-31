using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.FewerVisualDistractions;
public static class ParallaxSpeedLimiter
{
    public static ILHook parallaxRenderHook;
    public static void Load()
    {
        On.Celeste.Parallax.Update += Parallax_Update;

        parallaxRenderHook = new(
            typeof(Parallax).GetMethod("orig_Render", BindingFlags.Public | BindingFlags.Instance), patch_Parallax_orig_Render);
    }

    public static bool ShouldLockParallax() => FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == FewerVisualDistractionsModuleSettings.ParallaxSettingValue.Locked;
    private static void patch_Parallax_orig_Render(ILContext il)
    {
        // This patch removes the parallax effect when the player moves, but does not remove the parallax effect due to wind (especially in chapter 4)
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(
            instr => instr.MatchLdloc(0),
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<Backdrop>("Scroll")
        ))
        {
            Logger.Log(LogLevel.Error, "FewerVisualDistractions", "Couldn't find CIL sequence to hook in Parallax.orig_Render!");
            return;
        }

        // Lock the background entirely, keeping every pixel in the same place even as you move through the level
        ILCursor target = cursor.Clone();
        target.Index += 5;
        cursor.EmitDelegate(ShouldLockParallax);
        cursor.Emit(OpCodes.Brtrue, target.Next);
    }

    private static void Parallax_Update(On.Celeste.Parallax.orig_Update orig, Parallax self, Scene scene)
    {
        // This patch removes background movement due to wind (especially in chapter 4), but does not affect the parallax effect as the player moves

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

    public static void Unload()
    {
        On.Celeste.Parallax.Update -= Parallax_Update;
    }
}
