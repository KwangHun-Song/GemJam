using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public static class TileUtility {
        public static int GetTopY(Tile[] tiles) => tiles.Max(t => t.Y); // 0 based

        public static Tile GetTile(int x, int y, Tile[] tiles) {
            if (x < 0) return null;
            if (x > Constants.Width - 1) return null;
            if (y < 0) return null;
            if (y > GetTopY(tiles)) return null;

            return tiles[y * Constants.Width + x];
        }

        public static IEnumerable<Tile> GetAdjacentTiles(Tile tile, Tile[] tiles) {
            if (tile.X > 0 && GetTile(tile.X - 1, tile.Y, tiles) != null) 
                yield return GetTile(tile.X - 1, tile.Y, tiles);
            if (tile.X < Constants.Width - 1 && GetTile(tile.X + 1, tile.Y, tiles) != null) 
                yield return GetTile(tile.X + 1, tile.Y, tiles);
            if (tile.Y > 0 && GetTile(tile.X, tile.Y - 1, tiles) != null) 
                yield return GetTile(tile.X, tile.Y - 1, tiles);
            if (tile.Y < GetTopY(tiles) && GetTile(tile.X, tile.Y + 1, tiles) != null) 
                yield return GetTile(tile.X, tile.Y + 1, tiles);
        }
    }
}