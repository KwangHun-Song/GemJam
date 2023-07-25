using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace GemMatch {
    public enum GameResult { Clear, Fail }
    public enum ColorIndex { None = 0, Red, Orange, Yellow, Green, Blue, Purple }
    
    public class Controller {
        private const int MaxMemoryCount = 7;
        
        private UniTaskCompletionSource<GameResult> gameCompletionSource;
        private List<IGemMatchListener> listeners = new List<IGemMatchListener>();

        public Level CurrentLevel { get; private set; }
        public Tile[] Tiles { get; private set; }
        public List<ColorIndex> Memory { get; private set; }
        public Mission[] Missions { get; private set; }

        public void StartGame(Level level) {
            // 초기화
            CurrentLevel = level;
            Memory = new List<ColorIndex>();
            Tiles = level.tiles;
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            
            // 게임 시작
            gameCompletionSource = new UniTaskCompletionSource<GameResult>();

            // 이벤트 전달
            foreach (var listener in listeners) listener.OnStartGame(Tiles, Missions);
        }

        public async UniTask<GameResult> WaitUntilGameEnd() {
            return await gameCompletionSource.Task;
        }

        public void Input(int tileIndex) {
            if (Memory.Count >= MaxMemoryCount) return;

            var tile = Tiles[tileIndex];
            
            if (IsValidTouch(tile) == false) return;
            
            // 메모리에 추가
            Memory.Add(tile.NormalBlock.Color);
            foreach (var listener in listeners) listener.OnAddMemory(tile.NormalBlock.Color, Memory.Count - 1);

            // 같은 색깔이 세 개면 제거
            foreach (var color in GetThreeColors(Memory)) {
                RemoveColor(color);
                foreach (var listener in listeners) listener.OnRemoveMemory(color);
            }
            
            // 클리어조건 검사
            if (IsCleared()) {
                ClearGame();
            }
            
            // 실패조건 검사
            if (Memory.Count >= MaxMemoryCount) {
                FailGame();
            }
        }

        public void ClearGame() {
            gameCompletionSource.TrySetResult(GameResult.Clear);
            
            // 이벤트 전달
            foreach (var listener in listeners) listener.OnClearGame(Missions);
        }

        public void FailGame() {
            gameCompletionSource.TrySetResult(GameResult.Fail);
            
            // 이벤트 전달
            foreach (var listener in listeners) listener.OnFailGame(Missions);
        }

        public void ReplayGame() {
            StartGame(CurrentLevel);
            
            // 이벤트 전달
            foreach (var listener in listeners) listener.OnReplayGame(Missions);
        }

        private bool IsCleared() {
            return !Missions.Where((mission, index) => CurrentLevel.missions[index].count > mission.count).Any();
        }

        private void RemoveColor(ColorIndex color) {
            Memory = Memory.Where(c => c != color).ToList();
        }

        private static bool IsValidTouch(Tile tile) {
            if (tile?.NormalBlock == null) return false;
            if (tile.AdjacentTiles.All(t => t != null && t.entities.Any())) return false;
            if (tile.entities.Any(e => e.CanTouch == false)) return false;

            return true;
        }

        private static IEnumerable<ColorIndex> GetThreeColors(List<ColorIndex> colorIndices) {
            return colorIndices.GroupBy(c => c)
                .Where(g => g.Count() >= 3)
                .Select(g => g.Key)
                .ToArray();
        }
    }
}
