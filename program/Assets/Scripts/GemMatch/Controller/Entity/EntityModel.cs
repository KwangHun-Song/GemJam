using System;

namespace GemMatch {
    [Serializable]
    public class EntityModel : IEquatable<EntityModel> {
        public EntityIndex index;
        public Layer layer;
        public int displayType;
        public int durability;
        public ColorIndex color;

        public EntityModel Clone() {
            return (EntityModel)MemberwiseClone();
        }

        public bool Equals(EntityModel other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return index == other.index && layer == other.layer && displayType == other.displayType && color == other.color;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityModel)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine((int)index, (int)layer, displayType, (int)color);
        }
    }
}