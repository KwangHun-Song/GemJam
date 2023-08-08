using System.Linq;

namespace GemMatch {
    /// <summary>
    /// 컬러를 그냥 섞어서 보여주어야 할 때 사용한다.(장치를 추가했는데 솔버가 작동하지 않을 때)
    /// 클리어 가능한 경우만 보여주는 디스트리뷰터보다 빠르다.
    /// </summary>
    public class RandomColorDistributor : IColorDistributor {
        public IColorCalculator GetColorCalculator() {
            return new RandomColorCalculator();
        }

        public bool DistributeColors(Level level, Tile[] tiles) {
            var colorCalculator = GetColorCalculator();
            var randomColorPieces = tiles
                .SelectMany(t => t.Entities.Values)
                .Where(e => e is NormalPiece && e.Color == ColorIndex.Random)
                .ToArray();
            if (randomColorPieces.Any() == false) return true;

            // 클리어 가능한 컬러들 큐 만들기
            var availableColors = Constants.UsableColors.Take(level.colorCount).ToList();
            var colorsQueue = colorCalculator.GenerateColorQueue(randomColorPieces.Count(), availableColors);

            var randomColorTilesIndices = tiles.Where(t => {
                var color = t.Piece?.Color ?? ColorIndex.None;
                return color == ColorIndex.Random;
            }).Select(t => t.Index).ToArray();

            UnityEngine.Debug.Log($"randomTileIndices: {string.Join(", ", randomColorTilesIndices)}");
            foreach (var tileIndex in randomColorTilesIndices) {
                if (randomColorTilesIndices.Contains(tileIndex)) {
                    tiles[tileIndex].Piece.Color = colorsQueue.Dequeue();
                }
            }

            return true;
        }
    }
}