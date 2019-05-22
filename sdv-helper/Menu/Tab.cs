using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace sdv_helper.Menu
{
    class Tab : IClickableMenu
    {
        private Rectangle dimensions;

        public int TabIndex { get; set; }
        public string Label { get; set; }

        public Tab(int x, int y, int w, int h, string label, int index)
        {
            TabIndex = index;
            Label = label;

            dimensions = new Rectangle(x, y, w, h);
        }

        public override void draw(SpriteBatch b)
        {
            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height, Color.White);
            Utility.drawTextWithShadow(b, Label, Game1.smallFont, new Vector2(dimensions.X + ConfigMenu.PaddingX / 2, dimensions.Y + ConfigMenu.PaddingY / 2), Game1.textColor);
        }

        public bool WasClicked(int x, int y)
        {
            return dimensions.Contains(x, y);
        }
    }
}
