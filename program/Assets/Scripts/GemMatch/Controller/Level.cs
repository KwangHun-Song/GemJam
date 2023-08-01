using System;

namespace GemMatch {
    [Serializable]
    public class Level {
        public TileModel[] tiles;
        public Mission[] missions;
        public int colorCount;
    }
}