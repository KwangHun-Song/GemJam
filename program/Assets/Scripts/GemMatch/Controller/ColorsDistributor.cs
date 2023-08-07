using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public class ColorsDistributor {
        
        public virtual RandomColorCalculator ColorCalculator { get; } = new RandomColorCalculator();
        public Level Level { get;}
        
        public ColorsDistributor(Level currentLevel) {
            Level = currentLevel;
        }

        public void DistributeClearableColors(Tile[] tiles) {
            var randomColorPieces = tiles
                .SelectMany(t => t.Entities.Values)
                .Where(e => e.Color == ColorIndex.Random)
                .ToArray();
            if (randomColorPieces.Any() == false) return;

            // 클리어 가능한 컬러들 큐 만들기
            var availableColors = Constants.UsableColors.Take(Level.colorCount).ToList();
            var colorsQueue = ColorCalculator.GenerateColorQueue(randomColorPieces.Count(), availableColors);

            UnityEngine.Debug.Log(
                $"colors : {string.Join(", ", colorsQueue.Select(ci => ci.ToString().Substring(0, 1)))}");

            // 랜덤 컬러를 단일 색상으로 교체해서, 클리어 가능한 타일 클릭 순서 찾아내기
            var dummyLevel = new Level {
                tiles = Level.tiles.Select(tm => {
                    var tileModel = tm.Clone();
                    var entityModel = tileModel.entityModels.SingleOrDefault(em => em.index == EntityIndex.NormalPiece);
                    if (entityModel != null) entityModel.color = ColorIndex.Sole;
                    return tileModel;
                }).ToArray(),
                missions = Level.missions,
                colorCount = Level.colorCount,
            };

            // 가능한 것 아무거나 클릭하는 솔버를 만들어서 결과 얻기
            var solver = new Solver(new PickRandomAvailableAI());
            var solverResult = solver.Solve(dummyLevel);
            if (solverResult.gameResult != GameResult.Clear) {
                UnityEngine.Debug.Log("solver Failed!");
                SetRandomColors(colorsQueue);
                return;
            }

            UnityEngine.Debug.Log($"tileIndices: {string.Join(", ", solverResult.tileIndices)}");

            // 클릭한 순서에 맞게 색깔들 배치하기
            var randomColorTilesIndices = tiles.Where(t => {
                var color = t.Piece?.Color ?? ColorIndex.None;
                return color == ColorIndex.Random;
            }).Select(t => t.Index).ToArray();

            foreach (var tileIndex in solverResult.tileIndices) {
                if (randomColorTilesIndices.Contains(tileIndex)) {
                    tiles[tileIndex].Piece.Color = colorsQueue.Dequeue();
                }
            }

            // 색상을 랜덤으로 지정합니다. 클리어 가능한 색상들을 얻어오는데 실패한 경우 사용됩니다.
            void SetRandomColors(Queue<ColorIndex> colorsQueue) {
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
            }
        }
    }
}