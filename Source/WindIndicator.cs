using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.FewerVisualDistractions;
public class WindIndicator : Entity
{
    private readonly Level level;
    public WindIndicator(Level level)
    {
        Tag = Tags.HUD;
        this.level = level;
    }

    public override void Render()
    {
        // Don't simply display if wind != 0, because that could make the text disappear/flicker during wind transitions in screens with varying wind
        if (!FewerVisualDistractionsModule.Settings.ShowWindIndicator || level?.windController?.pattern == WindController.Patterns.None)
            return;

        string magnitude = level.Wind.Length() switch
        {
            < 1 => "none",
            < 750 => "weak",
            < 1100 => "strong",
            _ => "crazy",
        };
        string windString = (level.Wind.X, level.Wind.Y) switch
        {
            (<0, 0) => $"Wind: Left, {magnitude}",
            (>0, 0) => $"Wind: Right, {magnitude}",
            (0, <0) => $"Wind: Up, {magnitude}",
            (0, >0) => $"Wind: Down, {magnitude}",
            _ => "Wind: None"
        };

        ActiveFont.DrawOutline(windString, new Vector2(50, 50), new Vector2(0f, 0f), Vector2.One, Color.White, 2f, Color.Black);
    }
}
