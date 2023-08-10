using System;
using System.Linq;

namespace GemMatch {
    [Serializable]
    public class Level : ICloneable<Level> {
        public TileModel[] tiles;
        public Mission[] missions;
        public int colorCount;
        public ColorIndex[] colorCandidates;
        public Level Clone() {
            return new Level() {
                tiles = this.tiles.Select(t => t.Clone()).ToArray(),
                missions = this.missions.Select(m => m.Clone()).ToArray(),
                colorCount = this.colorCount,
                colorCandidates = (ColorIndex[])this.colorCandidates.Clone(),
            };
        }
    }
}