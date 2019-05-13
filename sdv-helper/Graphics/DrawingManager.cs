using Microsoft.Xna.Framework;
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
            colorManager = new ColorManager(Game1.graphics.GraphicsDevice);
            this.settings = settings;
        }

        public void SendHudMessage(string message, int type)
        {
            Game1.addHUDMessage(new HUDMessage(message, type));
        }

        public void LabelEntities(Detector detector)
        {
            Vector2 playerPosition = new Vector2(Game1.player.getTileX(), Game1.player.getTileY());

            // viewport height/width
            float vw = Game1.viewport.Width / Game1.tileSize;
            float vh = Game1.viewport.Height / Game1.tileSize;
            float vx = Game1.viewport.X / Game1.tileSize;
            float vy = Game1.viewport.Y / Game1.tileSize;
            float centerX = vx + vw / 2;
            float centerY = vy + vh / 2;

            foreach (var entry in detector.Entities)
            {
                Vector2 targetPos = entry.Key;
                object target = entry.Value;
                int distance = (int)Vector2.Distance(playerPosition, targetPos);

                // not safe but should be guaranteed as every object should have a Name property
                string name = (string)target.GetType().GetProperty("Name").GetValue(target);
                Color c = colorManager.GetDefaultColorFor(target, detector);

                // kind of weird you have to get the color before checking if it's enabled
                if (!settings.IsEnabled(name, c)) continue;
                c = settings.GetColorFor(name, c);
                Texture2D texture = colorManager.GetTextureWithColor(c);

                string text = $"{name}: {distance.ToString("D2")}";
                Vector2 textSize = Game1.smallFont.MeasureString(text) / Game1.tileSize;
                float slope = (targetPos.Y - centerY) / (targetPos.X - centerX);

                // where it should be drawn
                Vector2 currentDrawPos = new Vector2(entry.Key.X - vx - textSize.X / 2, entry.Key.Y - vy - textSize.Y / 2);

                // if it's offscreen to the left or right
                if (entry.Key.X < vx)
                {
                    currentDrawPos.X = 0;
                    currentDrawPos.Y = entry.Key.Y + (vx - entry.Key.X) * slope - vy;
                }
                else if (entry.Key.X > vx + vw)
                {
                    currentDrawPos.X = vw - textSize.X;
                    currentDrawPos.Y = entry.Key.Y + (entry.Key.X - vx - vw) * slope - vy;
                }
                currentDrawPos.Y = Math.Max(0, Math.Min(currentDrawPos.Y, vh - textSize.Y));

                // scale to screen
                currentDrawPos *= Game1.tileSize;
                textSize *= Game1.tileSize;

                // draw rectangle around text
                Game1.spriteBatch.Draw(
                    texture,
                    new Rectangle((int)currentDrawPos.X, (int)currentDrawPos.Y, (int)textSize.X, (int)textSize.Y),
                    c * 0.5f);

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
