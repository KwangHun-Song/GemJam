using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public enum Direction { LeftDown, Left, LeftUp, Up, RightUp, Right, RightDown, Down }

    public class DirectionTiles {
        public Tile LeftDown;
        public Tile Left;
        public Tile LeftUp;
        public Tile Up;
        public Tile RightUp;
        public Tile Right;
        public Tile RightDown;
        public Tile Down;
    }
    
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
            if (tile.Y < GetTopY(tiles) && GetTile(tile.X, tile.Y + 1, tiles) != null) 
                yield return GetTile(tile.X, tile.Y + 1, tiles);
            if (tile.X > 0 && GetTile(tile.X - 1, tile.Y, tiles) != null) 
                yield return GetTile(tile.X - 1, tile.Y, tiles);
            if (tile.X < Constants.Width - 1 && GetTile(tile.X + 1, tile.Y, tiles) != null) 
                yield return GetTile(tile.X + 1, tile.Y, tiles);
            if (tile.Y > 0 && GetTile(tile.X, tile.Y - 1, tiles) != null) 
                yield return GetTile(tile.X, tile.Y - 1, tiles);
        }

        public static DirectionTiles GetAdjacentWithDiagonalTiles(Tile tile, Tile[] tiles) {
            var surroundingTiles = new DirectionTiles();

            // 오프셋을 사용
            var directionOffsets = new Dictionary<Direction, (int x, int y)> {
                { Direction.LeftDown, (-1, -1) },
                { Direction.Left, (-1, 0) },
                { Direction.LeftUp, (-1, 1) },
                { Direction.Up, (0, 1) },
                { Direction.RightUp, (1, 1) },
                { Direction.Right, (1, 0) },
                { Direction.RightDown, (1, -1) },
                { Direction.Down, (0, -1) }
            };

            foreach (var direction in directionOffsets) {
                var x = tile.X + direction.Value.x;
                var y = tile.Y + direction.Value.y;

                // 타일이 유효 범위에 있으면 추가한다.
                if (x >= 0 && x <= Constants.Width - 1 && y >= 0 && y <= GetTopY(tiles)) {
                    switch (direction.Key) {
                        case Direction.LeftDown: surroundingTiles.LeftDown = GetTile(x, y, tiles); break;
                        case Direction.Left: surroundingTiles.Left = GetTile(x, y, tiles); break;
                        case Direction.LeftUp: surroundingTiles.LeftUp = GetTile(x, y, tiles); break;
                        case Direction.Up: surroundingTiles.Up = GetTile(x, y, tiles); break;
                        case Direction.RightUp: surroundingTiles.RightUp = GetTile(x, y, tiles); break;
                        case Direction.Right: surroundingTiles.Right = GetTile(x, y, tiles); break;
                        case Direction.RightDown: surroundingTiles.RightDown = GetTile(x, y, tiles); break;
                        case Direction.Down: surroundingTiles.Down = GetTile(x, y, tiles); break;
                    }
                }
            }

            return surroundingTiles;
        }
    }
}