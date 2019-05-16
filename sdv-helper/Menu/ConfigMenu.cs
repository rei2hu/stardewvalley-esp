using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sdv_helper.Config;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sdv_helper.Menu
{
    class ConfigMenu : IClickableMenu
    {
        private static readonly string menuText = "Stardew Valley ESP";
        private static readonly int paddingX = 30;
        private static readonly int paddingY = 20;
        private static readonly Rectangle titleRect = new Rectangle(0, 256, 60, 60);
        private static readonly int textLength = 300;

        private readonly Dictionary<string, ColorComponent> colorPickers = new Dictionary<string, ColorComponent>();
        private readonly Scrollbar scrollbar;
        private readonly Settings settings;

        private readonly int bWidth = 1200;
        private readonly int bStartX = paddingX * 2;
        private readonly int bStartY = paddingY;
        private int currentEntry = 0;
        private bool scrolling = false;

        private readonly int bHeight;
        private int pages;
        private readonly int entriesPerPage;

        public ConfigMenu(Settings settings)
        {
            bHeight = Game1.viewport.Height - paddingY * 2;
            entriesPerPage = (int)Math.Floor(bHeight / (Game1.dialogueFont.MeasureString("A").Y + 20));

            this.settings = settings;
            pages = settings.DSettings.Count - entriesPerPage;
            scrollbar = new Scrollbar(bStartX + bWidth, bStartY, bHeight - borderWidth, pages);
            ResetColorPickers();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var c in colorPickers)
            {
                c.Value.receiveLeftClick(x, y, playSound);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            scrolling = false;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (x >= scrollbar.Left && x <= scrollbar.Right && y >= scrollbar.Top && y <= scrollbar.Bottom)
            {
                scrolling = true;
            }
            if (!scrolling) return;

            int sbHeight = scrollbar.Bottom - scrollbar.Top;
            int position = (int)(1f * (Game1.getMouseY() - scrollbar.Top) / sbHeight * pages);
            ScrollTo(position);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (direction < 0)
                ScrollDown();
            else
                ScrollUp();
        }

        public override void draw(SpriteBatch b)
        {
            // if there are new entries, reset the stuff
            if (settings.DSettings.Count != colorPickers.Count)
            {
                ResetColorPickers();
                pages = settings.DSettings.Count - entriesPerPage;
                scrollbar.Pages = pages;
            }

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            // main menu thing
            drawTextureBox(b, Game1.menuTexture, titleRect, bStartX, bStartY, bWidth, bHeight, Color.White);

            // title box
            Vector2 size = Game1.dialogueFont.MeasureString(menuText);
            drawTextureBox(b, Game1.menuTexture, titleRect, bStartX - paddingX / 2, bStartY - paddingY / 2, (int)size.X + paddingX, (int)size.Y + paddingY, Color.White);
            Utility.drawTextWithShadow(b, menuText, Game1.dialogueFont, new Vector2(bStartX, bStartY), Game1.textColor);
            scrollbar.draw(b);

            StringBuilder sb = new StringBuilder();
            for (int i = currentEntry; i < currentEntry + entriesPerPage; i++)
            {
                sb.Clear();
                int yCoord = bStartY + borderWidth * 2 + (i - currentEntry) * (28 + 36 + 5);
                foreach (char c in colorPickers.ElementAt(i).Key)
                {
                    sb.Append(c);
                    if (Game1.dialogueFont.MeasureString(sb).X > textLength)
                    {
                        sb.Remove(sb.Length - 2, 2);
                        break;
                    }
                }
                Utility.drawTextWithShadow(b, sb.ToString(), Game1.dialogueFont, new Vector2(bStartX + borderWidth, yCoord), Game1.textColor);
                colorPickers.ElementAt(i).Value.DrawAt(b, textLength + 150, yCoord);
            }

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero,
                    Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        private void ResetColorPickers()
        {
            // for every entry in a category do this
            List<string> keys = settings.DSettings.Keys.ToList();
            keys.Sort();

            colorPickers.Clear();
            foreach (string key in keys)
            {
                ColorComponent c = new ColorComponent(key, settings.DSettings[key], settings);
                colorPickers.Add(key, c);
            }
        }

        private void ScrollUp()
        {
            ScrollTo(currentEntry - 1);
        }

        private void ScrollDown()
        {
            ScrollTo(currentEntry + 1);
        }

        private void ScrollTo(int position)
        {
            if (position > pages || position < 0) return;
            currentEntry = position;
            scrollbar.SetBarAt(position);
            ResetColorPickers(); // kind of bad, because of unnecessary resort
        }
    }
}
