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

namespace sdv_helper
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Dictionary: location, list of items and locations
        private Dictionary<Vector2, StardewValley.Object> locationInformation = new Dictionary<Vector2, StardewValley.Object>();
        private GameLocation location;
        private static readonly Dictionary<Color, Texture2D> textures = new Dictionary<Color, Texture2D>();
        static ModEntry()
        {
            foreach (Color c in new Color[] { Color.White, Color.Black, Color.LightGreen, Color.Red, Color.DarkGray})
            {
                textures.Add(c, new Texture2D(Game1.graphics.GraphicsDevice, 1, 1));
                textures[c].SetData(new Color[] { c });
            }
        }

        public override void Entry(IModHelper helper)
        {

            // helper.Events.World.TerrainFeatureListChanged += World_TerrainFeatureListChanged;
            // helper.Events.World.LocationListChanged += World_LocationListChanged;
            helper.Events.Player.Warped += Player_Warped;
            // helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.Display.RenderingHud += Display_RenderingHud;
        }

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();

            // playerposition relative to game grid
            Vector2 playerPosition = new Vector2(Game1.player.Position.X / Game1.tileSize, Game1.player.Position.Y / Game1.tileSize);

            // viewport height/width
            float vw = Game1.viewport.Width / Game1.tileSize;
            float vh = Game1.viewport.Height / Game1.tileSize;
            float vx = Game1.viewport.X / Game1.tileSize;
            float vy = Game1.viewport.Y / Game1.tileSize;
            float centerX = vx + vw / 2;
            float centerY = vy + vh / 2;

            foreach (var entry in locationInformation)
            {

                int distance = (int)Vector2.Distance(playerPosition, entry.Key);
                string name = entry.Value.Name;
                string text = $"{name.Substring(0, Math.Min(name.Length, 4))}: {distance.ToString("D2")}";
                Vector2 size = Game1.smallFont.MeasureString(text);
                float slope = (entry.Key.Y - centerY) / (entry.Key.X - centerX);

                if (!counts.ContainsKey(name))
                {
                    counts.Add(name, 0);
                }
                counts[name]++;

                // assume it's on screen
                Vector2 currentDrawPos = new Vector2(entry.Key.X - vx, entry.Key.Y - vy);

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
                    currentDrawPos.X = vw - size.X / Game1.tileSize;
                    currentDrawPos.Y = entry.Key.Y + (entry.Key.X - vx - vw) * slope - vy;
                }
                // cap y values
                currentDrawPos.Y = Math.Max(size.Y / 2 / Game1.tileSize, Math.Min(currentDrawPos.Y, vh - size.Y / 2 / Game1.tileSize));
                // scale to screen
                currentDrawPos *= Game1.tileSize;

                // pick color
                Color c = Color.DarkGray;
                if (entry.Value.isForage(location))
                {
                    c = Color.LightGreen;
                }
                else if (entry.Value.Name.StartsWith("Artifact"))
                {
                    c = Color.White;
                }

                // draw rectangle around text
                Game1.spriteBatch.Draw(
                    textures[c],
                    new Rectangle((int) currentDrawPos.X, (int) currentDrawPos.Y, (int) size.X, (int) size.Y),
                    c);

                // draw text
                Game1.spriteBatch.DrawString(
                    Game1.smallFont,
                    text,
                    currentDrawPos,
                    Game1.textColor);
            }

            Vector2 drawPos = new Vector2(0, 0);
            foreach (var entry in counts)
            {
                Game1.spriteBatch.DrawString(
                    Game1.smallFont,
                    $"{entry.Key}: {entry.Value}",
                    drawPos,
                    Color.White);
                drawPos.Y += 25;
            }
        }
        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            location = e.NewLocation;
            PrintLocationObjects(e.NewLocation);
        }
        private void World_LocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            foreach (var loc in e.Added.Except(e.Removed))
            {
                PrintLocationObjects(loc);
            }
        }
        private void PrintLocationObjects(GameLocation loc)
        {
            PrintLocationObjects(loc, (l) => true);
        }
        private void PrintLocationObjects(GameLocation loc, Func<StardewValley.Object, bool> filter)
        {
            locationInformation.Clear();
            foreach (var obj in loc.Objects.Pairs)
            {
                if (filter(obj.Value))
                {
                    // Monitor.Log($"{obj.Key.X} {obj.Key.Y} {obj.Value.Name}");
                    locationInformation.Add(obj.Key, obj.Value);
                }
            }
        }
    }
}