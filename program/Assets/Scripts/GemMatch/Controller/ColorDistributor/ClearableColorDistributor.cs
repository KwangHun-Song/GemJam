using System.Linq;

namespace GemMatch {
    public class ClearableColorDistributor : IColorDistributor {

        public IColorCalculator GetColorCalculator() {
            return new ClearableColorCalculator();
        }
        
        public bool DistributeColors(Level level, Tile[] tiles) {
            var colorCalculator = GetColorCalculator();
            var randomColorPieces = tiles
                .SelectMany(t => t.Entities.Values)
                .Where(e => e is NormalPiece && e.Color == ColorIndex.Random)
                .ToArray();
            if (randomColorPieces.Any() == false) return true;

            // 클리어 가능한 컬러들 큐 만들기
            var availableColors = level.colorCandidates.ToList();
            var colorsQueue = colorCalculator.GenerateColorQueue(randomColorPieces.Count(), availableColors);

            UnityEngine.Debug.Log($"colors : {string.Join(", ", colorsQueue.Select(ci => ci.ToString().Substring(0, 1)))}");

            // 랜덤 컬러를 단일 컬러로 교체해서 아무 노멀피스나 클릭할 수 있게 솔버를 돌려서 클릭 결과 얻기
            var dummyLevel = GetSoleColorLevel(level);
            var solver = new Solver(new PickRandomAvailableNormalPieceAI(), new ColorSelectingController());
            var solverResult = solver.Solve(dummyLevel);
            if (solverResult.gameResult != GameResult.Clear) return false;

            UnityEngine.Debug.Log($"tileIndices: {string.Join(", ", solverResult.tileIndices)}");

            // 클릭한 순서에 맞게 색깔들 배치하기
            foreach (var tileIndex in solverResult.tileIndices) {
                if (tiles[tileIndex].Piece is not NormalPiece normalPiece) continue;
                normalPiece.Color = colorsQueue.Dequeue();
            }

            return true;
        }

        private Level GetSoleColorLevel(Level level) {
            return new Level {
                tiles = level.tiles.Select(tm => {
                    var tileModel = tm.Clone();
                    var entityModel = tileModel.entityModels.SingleOrDefault(em => em.index == EntityIndex.NormalPiece);
                    if (entityModel != null) entityModel.color = ColorIndex.Sole;
                    return tileModel;
                }).ToArray(),
                missions = level.missions,
                colorCount = level.colorCount,
            };
        }
    }
}