using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    /// <summary>
    /// 컬러 결정용 컨트롤러이므로 노멀피스만 픽하는 기능을 가졌다.
    /// </summary>
    public class SelectColorController : Controller {
        /// <summary>
        /// 시뮬레이션은 completionSource를 쓰지 않는다.
        /// </summary>
        public override void StartGame(Level level) {
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            Tiles = level.tiles.Select(tileModel => new Tile(tileModel.Clone())).ToArray();
            PathFinder = new PathFinder(Tiles);
            ColorsDistributor = new ColorsDistributor(CurrentLevel);
            
            ColorsDistributor.DistributeClearableColors(Tiles);
            CalculateActiveTiles();
        }

        /// <summary>
        /// 시뮬레이션은 completionSource를 쓰지 않으므로 즉시 결과를 반환한다.
        /// </summary>
        public SimulationResult SimulationInput(int tileIndex) {
            if (Memory.Count >= MaxMemoryCount) return SimulationResult.Error;

            // 타일 터치 처리
            var tile = Tiles[tileIndex];
            if (CanTouch(tile) == false) return SimulationResult.Error;
            Touch(tile);

            if (IsCleared()) return SimulationResult.Clear;
            if (IsFailed()) return SimulationResult.Fail;
            return SimulationResult.OnProgress;
        }

        /// <summary>
        /// 골피스 등을 신경쓰지 않고 노멀피스만 모두 픽했는지 검사한다.
        /// </summary>
        protected override bool IsCleared() {
            if (Tiles.SelectMany(t => t.Entities.Values).Any(e => e is NormalPiece) == false && Memory.Any() == false) return true;
            return false;
        }

        public override bool CanTouch(Tile tile) {
            if (tile.Piece is not NormalPiece) return false; // 기존 컨트롤러와 다르게 노멀피스만 터치한다.
            if (tile.Entities.Values.Where(e => e.Layer > Layer.Piece).Any(e => e.PreventTouch())) return false;
            if (PathFinder.HasPathToTop(tile) == false) return false;

            return true;
        }
    }
}