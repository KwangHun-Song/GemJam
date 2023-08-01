using System;

namespace GemMatch {
    [Serializable]
    public class Level {
        public Tile[] tiles;
        public Mission[] missions;
        public int colorCount;
    }
}