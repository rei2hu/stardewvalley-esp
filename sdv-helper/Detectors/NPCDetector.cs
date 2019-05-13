using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

// this includes monsters
namespace sdv_helper.Detectors
{
    class NPCDetector : IDetector
    {
        private GameLocation location;
        public EntityList detect()
        {
            EntityList e = new EntityList();
            if (location != null)
                foreach (var c in location.getCharacters())
                {
                    if (!(c is NPC))
                        throw new Exception("Invalid object type provided to NPC detection list");
                    e.Add(new KeyValuePair<Vector2, object>(c.getTileLocation(), c));
                }
            return e;
        }

        public IDetector setLocation(GameLocation loc)
        {
            location = loc;
            return this;
        }
    }
}
