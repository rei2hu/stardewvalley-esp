﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sdv_helper.Detectors;
using StardewValley;
using System;

namespace sdv_helper.Graphics
{
    class DrawingManager
    {
        private readonly ColorManager colorManager;
        private readonly Settings.Settings settings;

        public DrawingManager(Settings.Settings settings)
        {
            colorManager = new ColorManager();
            this.settings = settings;
        }

        public void SendHudMessage(string message, int type)
        {
            Game1.addHUDMessage(new HUDMessage(message, type));
        }

        public void LabelEntities(Detector detector)
        {
            Vector2 playerPosition = Game1.player.Position;

            float sceenWidth = Game1.viewport.Width;
            float screenHeight = Game1.viewport.Height;
            float screenLeft = Game1.viewport.X;
            float screenTop = Game1.viewport.Y;
            float centerX = screenLeft + sceenWidth / 2;
            float centerY = screenTop + screenHeight / 2;

            foreach (var entry in detector.Entities)
            {
                Vector2 targetPos = entry.Key * Game1.tileSize;
                object target = entry.Value;
                int distance = (int)Vector2.Distance(playerPosition, targetPos);

                // not safe but should be guaranteed as every object should have a Name property
                string name = (string)target.GetType().GetProperty("Name").GetValue(target);
                Color c = colorManager.GetDefaultColorFor(target, detector);

                // kind of weird you have to get the color before checking if it's enabled
                if (!settings.IsEnabled(name, c)) continue;
                c = settings.GetColorFor(name, c);
                Texture2D texture = colorManager.GetTextureWithColor(c);

                string text = $"{name}: {(distance / Game1.tileSize).ToString("D2")}";
                Vector2 textSize = Game1.smallFont.MeasureString(text);
                float slope = (targetPos.Y - centerY) / (targetPos.X - centerX);

                // where it should be drawn
                Vector2 currentDrawPos = new Vector2(targetPos.X - screenLeft - textSize.X / 2, targetPos.Y - screenTop - textSize.Y / 2);

                // if it's offscreen to the left or right
                if (targetPos.X < screenLeft)
                {
                    currentDrawPos.X = 0;
                    currentDrawPos.Y = targetPos.Y + (screenLeft - targetPos.X) * slope - screenTop;
                }
                else if (targetPos.X > screenLeft + sceenWidth)
                {
                    currentDrawPos.X = sceenWidth - textSize.X;
                    currentDrawPos.Y = targetPos.Y + (targetPos.X - screenLeft - sceenWidth) * slope - screenTop;
                }
                currentDrawPos.Y = Math.Max(0, Math.Min(currentDrawPos.Y, screenHeight - textSize.Y));

                // draw rectangle around text
                Game1.spriteBatch.Draw(
                    texture,
                    new Rectangle((int)currentDrawPos.X, (int)currentDrawPos.Y, (int)textSize.X, (int)textSize.Y),
                    c * 0.75f);

                // draw text
                Game1.spriteBatch.DrawString(
                    Game1.smallFont,
                    text,
                    currentDrawPos,
                    Game1.textColor);
            }
        }
    }
}
