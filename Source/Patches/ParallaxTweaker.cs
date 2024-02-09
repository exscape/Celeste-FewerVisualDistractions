using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.FewerVisualDistractions.Patches;
using static FewerVisualDistractionsModuleSettings;

public static class ParallaxTweaker
{
    static On.Celeste.DreamBlock.orig_PutInside PutInside_orig = default;

    public static void Load()
    {
        // Remove background movement due to wind (especially in chapter 4); this does not affect the parallax effect as the player moves
        On.Celeste.Parallax.Update += Parallax_Update;

        // Tweak/lock background movement when the player moves (Everest method)
        On.Celeste.Parallax.Render += Parallax_Render;

        // Lock parallax in dream blocks
        On.Celeste.DreamBlock.Render += DreamBlock_Render;
        On.Celeste.DreamBlock.PutInside += DreamBlock_PutInside;

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

    public static Vector2 ReplaceParallaxScrollVector(Vector2 scroll, bool loopX, bool loopY)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == ParallaxSettingValue.Standard)
            return scroll;
        else if (FewerVisualDistractionsModule.Settings.ParallaxDuringMovement == ParallaxSettingValue.Locked)
            return Vector2.Zero;

        // This used to be the "Follow Camera" case, but that code didn't work very well, and indeed had outright bugs:
        // A scroll value of -0.02 (used in the Strawberry Jam lobby, for one) was replaced with -1, so the sky moved at breakneck speeds
        // when you moved.
        // The name is still used for the setting, but the UI shows it as "Mixed".
        //
        // Ideally these should use 0f in the else clauses, but that causes issues on some levels, e.g. The Summit (at least near the top), as
        // *part* of the background turns black.
        return new Vector2(
            (loopX && scroll.X > 0.2f) ? 1f : scroll.X,
            (loopY && scroll.Y > 0.2f) ? 1f : scroll.Y);
    }

    private static void Parallax_Render(On.Celeste.Parallax.orig_Render orig, Parallax self, Scene scene)
    {
        var oldScroll = self.Scroll;
        self.Scroll = ReplaceParallaxScrollVector(self.Scroll, self.LoopX, self.LoopY); // The method checks if we should actually replace or not
        orig(self, scene);

        if (FewerVisualDistractionsModule.Settings.ModEnabled && FewerVisualDistractionsModule.Settings.ParallaxDuringMovement != ParallaxSettingValue.Standard)
            self.Scroll = oldScroll;
    }

    private static void DreamBlock_Render(On.Celeste.DreamBlock.orig_Render orig, DreamBlock self)
    {
        if (!FewerVisualDistractionsModule.Settings.ModEnabled || !FewerVisualDistractionsModule.Settings.DreamBlockStarsFollowCamera)
        {
            orig(self);
            return;
        }

        // Initialize the delegate/pointer if needed
        if (PutInside_orig == null)
            self.PutInside(Vector2.One);

        var cameraPosition = self.SceneAs<Level>().Camera.Position;
        var positions = self.particles.Select(p => p.Position).ToArray();

        // Null out the movement added for each particle in the foreach loop
        for (int i = 0; i < self.particles.Length; i++)
        {
            self.particles[i].Position = PutInside_orig(self, self.particles[i].Position);
            self.particles[i].Position -= cameraPosition * (0.3f + 0.25f * self.particles[i].Layer);
        }

        orig(self);

        for (int i = 0; i < positions.Length; i++)
        {
            self.particles[i].Position = positions[i];
        }
    }

    private static Vector2 DreamBlock_PutInside(On.Celeste.DreamBlock.orig_PutInside orig, DreamBlock self, Vector2 pos)
    {
        PutInside_orig = orig;

        if (!FewerVisualDistractionsModule.Settings.ModEnabled || !FewerVisualDistractionsModule.Settings.DreamBlockStarsFollowCamera)
            return orig(self, pos);
        else
            return pos;
    }

    private static void Planets_Render(On.Celeste.Planets.orig_Render orig, Planets self, Scene scene)
    {
        var oldScroll = self.Scroll;
        self.Scroll = ReplaceParallaxScrollVector(self.Scroll, self.LoopX, self.LoopY); // The method checks if we should actually replace or not
        orig(self, scene);

        if (FewerVisualDistractionsModule.Settings.ModEnabled && FewerVisualDistractionsModule.Settings.ParallaxDuringMovement != ParallaxSettingValue.Standard)
            self.Scroll = oldScroll;
    }

    public static void Unload()
    {
        On.Celeste.Parallax.Update -= Parallax_Update;
        On.Celeste.Parallax.Render -= Parallax_Render;
        On.Celeste.DreamBlock.Render -= DreamBlock_Render;
        On.Celeste.DreamBlock.PutInside -= DreamBlock_PutInside;
        On.Celeste.Planets.Render -= Planets_Render;
    }
}
