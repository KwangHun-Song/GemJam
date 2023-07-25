using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public class Tile {
        public int index;
        public bool isVisible;
        public List<Entity> entities;

        public int Index => index;
        public int X => index % Constants.Width;
        public int Y => index / Constants.Width;
        
        public Tile Left { get; private set; }
        public Tile Right { get; private set; }
        public Tile Up { get; private set; }
        public Tile Down { get; private set; }
        public IEnumerable<Tile> AdjacentTiles => new[] { Left, Right, Up, Down };

        public NormalBlock NormalBlock => entities.SingleOrDefault(e => e.Index == EntityIndex.Normal) as NormalBlock;
    }
}