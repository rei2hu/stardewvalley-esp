using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace sdv_helper.Drawing
{
    class ColorManager
    {
        private static readonly Dictionary<Color, Texture2D> textures = new Dictionary<Color, Texture2D>();
        public ColorManager(GraphicsDevice d)
        {
            foreach (Color c in new Color[] { Color.White, Color.Black, Color.LightGreen, Color.Red, Color.DarkGray })
            {
                textures.Add(c, new Texture2D(d, 1, 1));
                textures[c].SetData(new Color[] { c });
            }
        }
    }
}
