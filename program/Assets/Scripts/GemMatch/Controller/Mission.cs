using System;

namespace GemMatch {
    [Serializable]
    public class Mission : ICloneable<Mission> {
        public EntityModel entity;
        public int count;

        public Mission Clone() {
            return new Mission() {
                entity = this.entity.Clone(),
                count = this.count,
            };
        }
    }
}