using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BackdropHider;
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
        if (!BackdropHiderModule.Settings.ShowWindIndicator || level?.windController?.pattern == WindController.Patterns.None)
            return;

        string magnitude = level.Wind.Length() switch
        {
            < 1 => "none",
            < 750 => "weak",
            < 1100 => "strong",
            _ => "crazy",
        };
        string direction = (level.Wind.X, level.Wind.Y) switch
        {
            (<0, 0) => "Left",
            (>0, 0) => "Right",
            (0, <0) => "Up",
            (0, >0) => "Down",
            _ => "None"
        }; 

        ActiveFont.DrawOutline($"Wind: {direction}, {magnitude}", new Vector2(50, 50), new Vector2(0f, 0f), Vector2.One, Color.White, 2f, Color.Black);
    }
}