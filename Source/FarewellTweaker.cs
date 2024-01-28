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
        On.Celeste.MoonCreature.Render += MoonCreature_Render;

        // Freeze hue changes of spikes
        On.Celeste.CrystalStaticSpinner.UpdateHue += CrystalStaticSpinner_UpdateHue;

        // Freeze noise/static on billboard screens, and also disable the audio noise if the visual noise is not rendered
        On.Celeste.PlaybackBillboard.Update += PlaybackBillboard_Update;
        On.Celeste.SoundSource.Play += SoundSource_Play;
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
        if (FewerVisualDistractionsModule.Settings.ShowFloatingCreatures || !(self is Decal decal && decal.Name.Contains("farewell/creature_")))
            orig(self);
    }

    private static void MoonCreature_Render(On.Celeste.MoonCreature.orig_Render orig, MoonCreature self)
    {
        if (FewerVisualDistractionsModule.Settings.ShowFloatingCreatures)
            orig(self);
    }

    private static void CrystalStaticSpinner_UpdateHue(On.Celeste.CrystalStaticSpinner.orig_UpdateHue orig, CrystalStaticSpinner self)
    {
        if (FewerVisualDistractionsModule.Settings.AnimateCrystalColors)
            orig(self);
    }

    private static void PlaybackBillboard_Update(On.Celeste.PlaybackBillboard.orig_Update orig, PlaybackBillboard self)
    {
        if (FewerVisualDistractionsModule.Settings.AnimateBillboardNoise)
            orig(self);
    }

    private static SoundSource SoundSource_Play(On.Celeste.SoundSource.orig_Play orig, SoundSource self, string path, string param, float value)
    {
        if (FewerVisualDistractionsModule.Settings.AnimateBillboardNoise || !path.Contains("tutorial_static"))
            return orig(self, path, param, value);
        else
            return self;

    }

    public static void Unload()
    {
        On.Celeste.LightningRenderer.Bolt.Render -= Bolt_Render;
        On.Celeste.LightningRenderer.DrawBezierLightning -= LightningRenderer_DrawBezierLightning;
        On.Celeste.LightningRenderer.Update -= LightningRenderer_Update;
        On.Monocle.Entity.Render -= Entity_Render;
        On.Celeste.MoonCreature.Render -= MoonCreature_Render;
        On.Celeste.CrystalStaticSpinner.UpdateHue -= CrystalStaticSpinner_UpdateHue;
        On.Celeste.PlaybackBillboard.Update -= PlaybackBillboard_Update;
        On.Celeste.SoundSource.Play -= SoundSource_Play;
    }
}
