using GemMatch;

namespace Utility {
    public class GlobalUtility {
        public static Tile[] FlipVertically(Tile[] tiles, int width, int height) {
            var result = new Tile[width * height];

            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    result[(height - 1 - y) * width + x] = tiles[y * width + x];
                }
            }

            return result;
        }
    }
}