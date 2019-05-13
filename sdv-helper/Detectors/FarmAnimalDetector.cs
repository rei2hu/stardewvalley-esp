using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace sdv_helper.Detectors
{
    class FarmAnimalDetector : IDetector
    {
        private GameLocation location;
        public EntityList Detect()
        {
            EntityList e = new EntityList();
            if (location != null && location is Farm)
                foreach (var c in ((Farm)location).getAllFarmAnimals())
                {
                    // bug: if an animal is located in a building, the position is
                    // incorrect
                    e.Add(new KeyValuePair<Vector2, object>(c.getTileLocation(), c));
                }
            return e;
        }

        public IDetector SetLocation(GameLocation loc)
        {
            location = loc;
            return this;
        }
    }
}
