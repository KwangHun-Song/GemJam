using System;

namespace GemMatch {
    [Serializable]
    public class EntityModel {
        public EntityIndex index;
        public Layer layer;
        public int displayType;
        public int durability;
        public ColorIndex color;

        public EntityModel Clone() {
            return (EntityModel)MemberwiseClone();
        }
    }
}