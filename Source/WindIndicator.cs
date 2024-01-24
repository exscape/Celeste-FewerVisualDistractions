using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

namespace Celeste.Mod.FewerVisualDistractions;

public class WindIndicator : Entity
{
    private Level level;
    private bool initialDraw = true;
    private float rotation = 0f;
    private float lastRotation = (float)Math.PI;
    private float positionX = 10f;
    private bool levelHasWindTriggers = false;

    public WindIndicator(Level level)
    {
        Tag = Tags.HUD | Tags.Global;
        SwitchLevel(level);
    }

    public void SwitchLevel(Level level)
    {
        this.level = level;
        levelHasWindTriggers = level.Entities.FindAll<WindTrigger>().Where(t => t.Pattern != WindController.Patterns.None).Any();
    }

    public override void Render()
    {
        if (!FewerVisualDistractionsModule.Settings.ShowWindIndicator || level?.Paused == true)
            return;

        bool shouldDisplay = levelHasWindTriggers || (level.windController != null && level.windController.pattern != WindController.Patterns.None);

        if (FewerVisualDistractionsModule.Settings.WindIndicatorType == FewerVisualDistractionsModuleSettings.WindIndicatorTypeValue.Graphical)
            RenderGraphicalIndicator(shouldDisplay); // Always called, as we need to animate when shouldDisplay has just changed to to false
        else if (shouldDisplay)
            RenderTextIndicator();
    }

    public void RenderGraphicalIndicator(bool shouldDisplay)
    {
        string strength = level.Wind.Length() switch
        {
            < 100 => "WindStrength_0",  // Used in rooms with alternating wind patterns
            < 600 => "WindStrength_1",  // Weak = 400
            < 1000 => "WindStrength_2", // Strong = 800
            _ => "WindStrength_3",      // Crazy = 1200
        };

        MTexture directionTexture = GFX.Gui["FewerVisualDistractions/WindDirection"];
        MTexture strengthTexture = GFX.Gui[$"FewerVisualDistractions/{strength}"];

        float targetX = shouldDisplay ? 10 : -directionTexture.Width;
        float targetRotation = (float)((level.Wind.X, level.Wind.Y) switch
        {
            (> 0, 0)  => 0.0,          // Right wind
            (< 0, 0) => Math.PI,       // Left wind
            (0, > 0)  => Math.PI / 2,  // Downwards wind
            (0, < 0)  => -Math.PI / 2, // Upwards wind
            _ => lastRotation
        });

        // Remember for rooms where the wind is variable on/off towards other than left, to avoid unnecessary rotations of the indicator
        if (level.Wind.Length() > 0)
            lastRotation = rotation;

        if (initialDraw)
        {
            rotation = targetRotation;
            positionX = targetX;
            initialDraw = false;
        }
        else
        {
            // Only animate rotation when not moving in/out
            if (positionX == targetX)
                rotation = Calc.Approach(rotation, targetRotation, (float)(Math.PI * (12.0 / 180.0))); // 12 degrees/frame = 15 frames per 180 degrees = 250 ms for the full animation
            else if (targetX > positionX)
                rotation = targetRotation; // Ensure rotation is correct immediately as the icon enters the screen

            positionX = Calc.Approach(positionX, targetX, 10); // 10 px/frame, about 14 frames for the animation, so just under 250 ms
        }

        // We FINALLY know whether we should draw or not
        if (positionX <= -directionTexture.Width)
            return;

        var distanceBetweenTextures = 5;
        var baseY = FewerVisualDistractionsModule.Settings.WindIndicatorPosition == FewerVisualDistractionsModuleSettings.PositionValue.Top ? 10 : (1080 - 10 - directionTexture.Height - strengthTexture.Height - distanceBetweenTextures);
        var yOffset = FewerVisualDistractionsModule.Settings.WindIndicatorOffset * 10;
        var positionY = FewerVisualDistractionsModule.Settings.WindIndicatorPosition == FewerVisualDistractionsModuleSettings.PositionValue.Top ? baseY + yOffset : baseY - yOffset;

        directionTexture.Draw(
            position: new Vector2(positionX + directionTexture.Width / 2f, positionY + directionTexture.Height / 2f),
            origin: new Vector2(directionTexture.Width / 2f, directionTexture.Height / 2f),
            color: Color.White,
            scale: Vector2.One,
            rotation: rotation);

        strengthTexture.Draw(new Vector2(positionX, positionY + directionTexture.Height + distanceBetweenTextures));
    }

    public void RenderTextIndicator()
    {
        float magnitude = level.Wind.Length();
        string strength = magnitude switch
        {
            < 10 => "none",
            < 250 => "very weak",
            < 600 => "weak",
            < 1000 => "strong",
            _ => "crazy",
        };

        string windString = (level.Wind.X, level.Wind.Y) switch
        {
            (< 0, 0) => $"Wind: Left, {strength}",
            (> 0, 0) => $"Wind: Right, {strength}",
            (0, < 0) => $"Wind: Up, {strength}",
            (0, > 0) => $"Wind: Down, {strength}",
            _ => "Wind: None"
        };

        var baseY = FewerVisualDistractionsModule.Settings.WindIndicatorPosition == FewerVisualDistractionsModuleSettings.PositionValue.Top ?
            0 : (1080 - ActiveFont.HeightOf("Wind: Right, strong"));
        var yOffset = FewerVisualDistractionsModule.Settings.WindIndicatorOffset * 10;
        var positionY = baseY == 0 ? baseY + yOffset : baseY - yOffset;

        ActiveFont.DrawOutline(windString, new Vector2(16, positionY), Vector2.Zero, Vector2.One, Color.White, 2f, Color.Black);
    }
}
