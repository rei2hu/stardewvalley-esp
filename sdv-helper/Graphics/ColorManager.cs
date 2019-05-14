using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sdv_helper.Detectors;
using StardewValley;
using System.Collections.Generic;

namespace sdv_helper.Graphics
{
    class ColorManager
    {
        private static readonly Dictionary<Color, Texture2D> textures = new Dictionary<Color, Texture2D>();
        public ColorManager()
        {
            foreach (Color c in new Color[] { Color.White, Color.LightGreen, Color.Red, Color.DarkGray, Color.LightGray })
            {
                textures.Add(c, new Texture2D(Game1.graphics.GraphicsDevice, 1, 1));
                textures[c].SetData(new Color[] { c });
            }
        }

        public Texture2D GetTextureWithColor(Color c)
        {
            if (!textures.ContainsKey(c))
                textures.Add(c, new Texture2D(Game1.graphics.GraphicsDevice, 1, 1));
            textures[c].SetData(new Color[] { c });
            return textures[c];
        }

        public Color GetDefaultColorFor(object entity, Detector detector)
        {
            Color c = Color.DarkGray;
            switch (entity)
            {
                case NPC n:
                    if (n.IsMonster)
                    {
                        c = Color.Red;
                    }
                    else
                    {
                        c = Color.LightGray;
                    }
                    break;
                case StardewValley.Object o:
                    if (o.Name.StartsWith("Artifact"))
                    {
                        c = Color.White;
                    }
                    else if (o.isForage(detector.Location))
                    {
                        c = Color.LightGreen;
                    }
                    break;
                case FarmAnimal _:
                    c = Color.White;
                    break;
                case null:
                    throw new System.Exception($"Unknown entity type {entity.GetType()}");
            }
            return c;
        }
    }
}
