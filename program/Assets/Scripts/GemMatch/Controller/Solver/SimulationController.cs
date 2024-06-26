using System.Collections.Generic;
using System.Linq;
using GemMatch.UndoSystem;

namespace GemMatch {
    public class SimulationController : Controller {
        /// <summary>
        /// 시뮬레이션은 completionSource를 쓰지 않는다.
        /// </summary>
        public override void StartGame(Level level, bool isReplay = false) {
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            Tiles = level.tiles.Select(tileModel => new Tile(tileModel.Clone())).ToArray();
            PathFinder = new PathFinder(Tiles);
            ColorDistributor = new ClearableColorDistributor();
            UndoHandler = new UndoHandler();
            
            ColorDistributor.DistributeColors(CurrentLevel, Tiles);
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
    }
}