using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using StardewValley.TerrainFeatures;
using StardewValley.Network;
using Netcode;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using sdv_helper.Detectors;

namespace sdv_helper
{
    public class ModEntry : Mod
    {
        private static Detector detector = new Detector();
        private static readonly Dictionary<Color, Texture2D> textures = new Dictionary<Color, Texture2D>();
        static ModEntry()
        {
            foreach (Color c in new Color[] { Color.White, Color.Black, Color.LightGreen, Color.Red, Color.DarkGray, Color.LightGray })
            {
                textures.Add(c, new Texture2D(Game1.graphics.GraphicsDevice, 1, 1));
                textures[c].SetData(new Color[] { c });
            }
            detector.addDetector("NPC")
                .addDetector("Object");
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderingHud += Display_RenderingHud;
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            detector.detect();
        }

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
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
                string name = "";
                Color c = Color.DarkGray;
                switch (target)
                {
                    case NPC n:
                        name = n.Name;
                        c = Color.LightGray;
                        break;
                    case StardewValley.Object o:
                        name = o.Name;
                        if (o.isForage(detector.Location))
                        {
                            c = Color.LightGreen;
                        }
                        else if (o.Name.StartsWith("Artifact"))
                        {
                            c = Color.White;
                        }
                        break;
                    case null:
                        Monitor.Log($"Unknown Type {target.GetType()}");
                        continue;
                }

                string text = $"{name}: {distance.ToString("D2")}";
                Vector2 textSize = Game1.smallFont.MeasureString(text) / Game1.tileSize;
                float slope = (targetPos.Y - centerY) / (targetPos.X - centerX);

                // where it should be drawn
                Vector2 currentDrawPos = new Vector2(entry.Key.X - vx - textSize.X / 2, entry.Key.Y - vy - textSize.Y / 2);

                // if it's offscreen to the left or right
                if (entry.Key.X < vx)
                {
                    // far left
                    currentDrawPos.X = 0;
                    currentDrawPos.Y = entry.Key.Y + (vx - entry.Key.X) * slope - vy;
                }
                else if (entry.Key.X > vx + vw)
                {
                    // far right
                    currentDrawPos.X = vw - textSize.X;
                    currentDrawPos.Y = entry.Key.Y + (entry.Key.X - vx - vw) * slope - vy;
                }
                currentDrawPos.Y = Math.Max(0, Math.Min(currentDrawPos.Y, vh - textSize.Y));

                // scale to screen
                currentDrawPos *= Game1.tileSize;
                textSize *= Game1.tileSize;

                // draw rectangle around text
                Game1.spriteBatch.Draw(
                    textures[c],
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

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                detector.setLocation(e.NewLocation);
                    
        }
    }
}