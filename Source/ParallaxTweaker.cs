using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.FewerVisualDistractions;
using static FewerVisualDistractionsModuleSettings;

public static class ParallaxTweaker
{
    public static void Load()
    {
        // Remove background movement due to wind (especially in chapter 4); this does not affect the parallax effect as the player moves
        On.Celeste.Parallax.Update += Parallax_Update;

        // Tweak/lock background movement when the player moves (Everest method)
        On.Celeste.Parallax.Render += Parallax_Render;

        // Lock parallax in dream blocks
        IL.Celeste.DreamBlock.Render += patch_DreamBlock_Render;

        // Respect parallax settings for the Planets backdrop
        On.Celeste.Planets.Render += Planets_Render;
    }

    private static void Parallax_Update(On.Celeste.Parallax.orig_Update orig, Parallax self, Scene scene)
    {
        if (FewerVisualDistractionsModule.Settings.ModEnabled)
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
        }

        orig(self, scene);
    }

    public static Vector2 ReplaceParallaxScrollVector(Vector2 scroll)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == ParallaxSettingValue.Standard)
            return scroll;
        else if (FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == ParallaxSettingValue.Locked)
            return Vector2.Zero;

        // We want a vector with 1 for each nonzero component.
        // Just in case a negative value is used elsewhere, copy the sign too.
        // To have this truly follow the camera we'd use Vector2.One, but the backgrounds aren't designed for that, and so we'll quickly run out of visible pixels (usually in the Y direction).
        return new Vector2(
            (float)Math.CopySign(scroll.X == 0f ? 0f : 1f, scroll.X),
            (float)Math.CopySign(scroll.Y == 0f ? 0f : 1f, scroll.Y));
    }

    private static void Parallax_Render(On.Celeste.Parallax.orig_Render orig, Parallax self, Scene scene)
    {
        var oldScroll = self.Scroll;
        self.Scroll = ReplaceParallaxScrollVector(self.Scroll); // The method checks if we should actually replace or not
        orig(self, scene);

        if (FewerVisualDistractionsModule.Settings.ModEnabled && FewerVisualDistractionsModule.Settings.ParallaxDuringMovement != ParallaxSettingValue.Standard)
            self.Scroll = oldScroll;
    }

    private static bool DreamBlockParallaxLocked() => FewerVisualDistractionsModule.Settings.ModEnabled && FewerVisualDistractionsModule.Settings.DreamBlockStarsFollowCamera;
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

    private static void Planets_Render(On.Celeste.Planets.orig_Render orig, Planets self, Scene scene)
    {
        var oldScroll = self.Scroll;
        self.Scroll = ReplaceParallaxScrollVector(self.Scroll); // The method checks if we should actually replace or not
        orig(self, scene);

        if (FewerVisualDistractionsModule.Settings.ModEnabled && FewerVisualDistractionsModule.Settings.ParallaxDuringMovement != ParallaxSettingValue.Standard)
            self.Scroll = oldScroll;
    }

    public static void Unload()
    {
        On.Celeste.Parallax.Update -= Parallax_Update;
        On.Celeste.Parallax.Render -= Parallax_Render;
        IL.Celeste.DreamBlock.Render -= patch_DreamBlock_Render;
        On.Celeste.Planets.Render -= Planets_Render;
    }
}
