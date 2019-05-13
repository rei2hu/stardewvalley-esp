using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace sdv_helper.Detectors
{
    class Detector : IDetector
    {
        private Dictionary<string, IDetector> detectors = new Dictionary<string, IDetector>();
        public EntityList Entities { get; set; } = new EntityList();
        public GameLocation Location { get; set; }
        public Detector addDetector(string type)
        {
            IDetector d = null;
            switch (type)
            {
                case "NPC":
                    d = new NPCDetector();
                    break;
                case "Object":
                    d = new ObjectDetector();
                    break;
                case null:
                    return this;
            }
            detectors.Add(type, d);
            return this;
        }

        public EntityList detect()
        {
            Entities.Clear();
            foreach (var kvp in detectors)
            {

                Entities.AddRange(kvp.Value.detect());
            }
            return Entities;
        }

        public IDetector setLocation(GameLocation loc)
        {
            foreach (var kvp in detectors)
            {
                kvp.Value.setLocation(loc);
            }
            Location = loc;
            return this;
        }
    }
}
