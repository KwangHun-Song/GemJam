using System;

namespace GemMatch {
    [Serializable]
    public class Level : ICloneable<Level> {
        public TileModel[] tiles;
        public Mission[] missions;
        public int colorCount;
        public ColorIndex[] colorCandidates;
        public Level Clone() {
            return (Level)MemberwiseClone();
        }
    }
}