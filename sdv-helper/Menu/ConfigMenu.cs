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
        public static readonly Rectangle TitleRect = new Rectangle(0, 256, 60, 60);
        public static readonly string MenuText = "Stardew Valley ESP";
        public static readonly string[] TabNames = new string[] { "Colors", "Hotkeys" };
        public static readonly int PaddingX = 30;
        public static readonly int PaddingY = 20;
        public static readonly int TextLength = 300;
        public static readonly int TabHeight = 50;

        private readonly Dictionary<string, ColorComponent> colorPickers = new Dictionary<string, ColorComponent>();
        private readonly List<Tab> tabs = new List<Tab>();
        private readonly Scrollbar scrollbar;
        private readonly Settings settings;

        private readonly int bWidth = (int)(TextLength * 1.5);
        private readonly int bStartX = PaddingX * 2;
        private readonly int bStartY = PaddingY;
        private int tabIndex = 0;
        private int currentEntry = 0;
        private bool scrolling = false;

        private readonly int bHeight;
        private readonly int entriesPerPage;
        private int pages;

        public ConfigMenu(Settings settings)
        {
            this.settings = settings;
            bHeight = Game1.viewport.Height - PaddingY * 2 - TabHeight;
            entriesPerPage = (int)Math.Floor((bHeight - borderWidth * 2) / (Game1.dialogueFont.MeasureString("A").Y + 20));
            pages = settings.DSettings.Count - entriesPerPage;
            scrollbar = new Scrollbar(bStartX + bWidth, bStartY, bHeight - borderWidth, pages);

            ResetColorPickers();

            int off = 0;
            for (int i = 0; i < TabNames.Length; i++)
            {
                Vector2 size = Game1.smallFont.MeasureString(TabNames[i]);
                int width = (int)size.X + PaddingX;
                tabs.Add(new Tab(bStartX + off, bStartY + bHeight, width, (int)size.Y + PaddingY, TabNames[i], i));
                off += width + PaddingX / 2;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (KeyValuePair<string, ColorComponent> c in colorPickers)
                c.Value.receiveLeftClick(x, y, playSound);
            foreach (Tab t in tabs)
                if (t.WasClicked(x, y))
                {
                    SwitchTabTo(t.TabIndex);
                    break;
                }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (IsColorPickerOpen())
                return;

            scrolling = false;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (IsColorPickerOpen())
                return;

            if (x >= scrollbar.Left && x <= scrollbar.Right && y >= scrollbar.Top && y <= scrollbar.Bottom)
                scrolling = true;
            if (!scrolling)
                return;

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
            drawTextureBox(b, Game1.menuTexture, TitleRect, bStartX, bStartY, bWidth, bHeight, Color.White);

            // title box
            Vector2 size = Game1.dialogueFont.MeasureString(MenuText);
            drawTextureBox(b, Game1.menuTexture, TitleRect, bStartX - PaddingX / 2, bStartY - PaddingY / 2, (int)size.X + PaddingX, (int)size.Y + PaddingY, Color.White);
            Utility.drawTextWithShadow(b, MenuText, Game1.dialogueFont, new Vector2(bStartX, bStartY), Game1.textColor);

            // scrollbar
            scrollbar.draw(b);

            // menu content
            int yCoord;
            switch (tabIndex)
            {
                case 1:
                    yCoord = bStartY + borderWidth * 2;
                    Utility.drawTextWithShadow(b, "Menu Key:", Game1.dialogueFont, new Vector2(bStartX + borderWidth, yCoord), Game1.textColor);
                    Utility.drawTextWithShadow(b, settings.MenuKey.ToString(), Game1.dialogueFont, new Vector2(bStartX + bWidth - borderWidth * 2, yCoord), Game1.textColor);
                    yCoord += 28 + 36 + 5;
                    Utility.drawTextWithShadow(b, "Load Key:", Game1.dialogueFont, new Vector2(bStartX + borderWidth, yCoord), Game1.textColor);
                    Utility.drawTextWithShadow(b, settings.LoadKey.ToString(), Game1.dialogueFont, new Vector2(bStartX + bWidth - borderWidth * 2, yCoord), Game1.textColor);
                    break;
                case 0:
                default:
                    StringBuilder sb = new StringBuilder();
                    for (int i = currentEntry; i < currentEntry + entriesPerPage; i++)
                    {
                        sb.Clear();
                        yCoord = bStartY + borderWidth * 2 + (i - currentEntry) * (28 + 36 + 5);
                        foreach (char c in colorPickers.ElementAt(i).Key)
                        {
                            sb.Append(c);
                            if (Game1.dialogueFont.MeasureString(sb).X > TextLength)
                            {
                                sb.Remove(sb.Length - 2, 2);
                                break;
                            }
                        }
                        Utility.drawTextWithShadow(b, sb.ToString(), Game1.dialogueFont, new Vector2(bStartX + borderWidth, yCoord), Game1.textColor);
                        colorPickers.ElementAt(i).Value.DrawAt(b, TextLength + 150, yCoord);
                    }
                    break;
            }

            // tabs
            foreach (Tab t in tabs)
                t.draw(b);

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

        private bool IsColorPickerOpen()
        {
            foreach (KeyValuePair<string, ColorComponent> c in colorPickers)
                if (c.Value.ColorPicker.visible) return true;
            return false;
        }

        private void SwitchTabTo(int index)
        {
            if (tabIndex == index)
                return;
            tabIndex = index;
        }
    }
}
