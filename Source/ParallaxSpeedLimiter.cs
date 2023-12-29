using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Celeste.Mod.FewerVisualDistractions;
public static class ParallaxSpeedLimiter
{
    public static void Load()
    {
        On.Celeste.Parallax.Update += Parallax_Update;
    }

    private static void Parallax_Update(On.Celeste.Parallax.orig_Update orig, Parallax self, Scene scene)
    {
        // Null out the movement caused by the original Parallax.Update() (which hasn't called yet, but that doesn't matter)
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
