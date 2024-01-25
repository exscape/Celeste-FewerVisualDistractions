namespace Celeste.Mod.FewerVisualDistractions;

public static class FarewellTweaker
{
    public static void Load()
    {
        // Remove lightning bolts inside Lightning areas
        On.Celeste.LightningRenderer.Bolt.Render += Bolt_Render;
        On.Celeste.LightningRenderer.DrawBezierLightning += LightningRenderer_DrawBezierLightning;

        // Freeze the edges of lightning areas
        On.Celeste.LightningRenderer.Update += LightningRenderer_Update;

        // Remove floating creatures
        On.Monocle.Entity.Render += Entity_Render;
    }

    private static void Bolt_Render(On.Celeste.LightningRenderer.Bolt.orig_Render orig, object self)
    {
        // This method handles the occasional lightning bolts inside the areas
        if (FewerVisualDistractionsModule.Settings.ShowLightningBolts)
            orig(self);
    }

    private static void LightningRenderer_DrawBezierLightning(On.Celeste.LightningRenderer.orig_DrawBezierLightning orig, ref int index, ref Microsoft.Xna.Framework.Graphics.VertexPositionColor[] verts, uint seed, Microsoft.Xna.Framework.Vector2 pos, Microsoft.Xna.Framework.Vector2 a, Microsoft.Xna.Framework.Vector2 b, float anchor, int steps, Microsoft.Xna.Framework.Color color)
    {
        // This method handles the smaller, curved lightning bolts that randomly shows up around the edges
        if (FewerVisualDistractionsModule.Settings.ShowLightningBolts)
            orig(ref index, ref verts, seed, pos, a, b, anchor, steps, color);
    }

    private static void LightningRenderer_Update(On.Celeste.LightningRenderer.orig_Update orig, LightningRenderer self)
    {
        // Freeze/animate the edges of the lightning area
        orig(self);
        if (!FewerVisualDistractionsModule.Settings.AnimateLightningAreas)
            self.edgeSeed = 1;
    }

    private static void Entity_Render(On.Monocle.Entity.orig_Render orig, Monocle.Entity self)
    {
        if (!FewerVisualDistractionsModule.Settings.ShowFloatingCreatures && self is Decal decal && decal.Name.Contains("farewell/creature_"))
            return;

        orig(self);
    }

    public static void Unload()
    {
        On.Celeste.LightningRenderer.Bolt.Render -= Bolt_Render;
        On.Celeste.LightningRenderer.DrawBezierLightning -= LightningRenderer_DrawBezierLightning;
        On.Celeste.LightningRenderer.Update -= LightningRenderer_Update;
        On.Monocle.Entity.Render -= Entity_Render;
    }
}
