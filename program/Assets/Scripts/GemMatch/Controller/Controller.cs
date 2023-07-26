using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace GemMatch {
    public enum GameResult { Clear, Fail }
    public enum ColorIndex { None = 0, Red, Orange, Yellow, Green, Blue, Purple }
    
    public class Controller {
        private const int MaxMemoryCount = 7;
        
        private UniTaskCompletionSource<GameResult> gameCompletionSource;
        private List<IControllerEvent> listeners = new List<IControllerEvent>();

        public Level CurrentLevel { get; private set; }
        public Tile[] Tiles { get; private set; }
        public List<Entity> Memory { get; private set; }
        public Mission[] Missions { get; private set; }

        public void StartGame(Level level) {
            // 초기화
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            Tiles = level.tiles.Select(t => new Tile()).ToArray();
            foreach (var tile in Tiles) tile.Initialize(this);

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

            if (CanTouch(tile) == false) return;
            
            // 타일 터치 처리
            Touch(tile);

            // 메모리에 추가
            AddToMemory(tile.Piece);

            // 같은 색깔이 세 개면 제거
            foreach (var color in GetThreeColors(Memory)) {
                RemoveFromMemory(color);
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

        public Tile GetTile(int x, int y) {
            if (x < 0) return null;
            if (x > Constants.Width - 1) return null;
            if (y < 0) return null;
            if (y * Constants.Width + x > Tiles.Length - 1) return null;

            return Tiles[y * Constants.Width + x];
        }

        private bool IsCleared() {
            return !Missions.Where((mission, index) => CurrentLevel.missions[index].count > mission.count).Any();
        }

        private void Touch(Tile tile) {
            tile.RemoveLayer(Layer.Piece);
            foreach (var adjacentTile in tile.AdjacentTiles.Where(t => t != null)) {
                adjacentTile.SplashHit();
            }
        }

        private void RemoveFromMemory(ColorIndex color) {
            for (int i = Memory.Count - 1; i >= 0; i--) {
                if (Memory[i] is NormalPiece piece && piece.Color == color) {
                    Memory.Remove(piece);
                    foreach (var listener in listeners) listener.OnRemoveMemory(piece);
                }
            }
        }

        private void AddToMemory(Entity piece) {
            Memory.Add(piece);
            foreach (var listener in listeners) listener.OnAddMemory(piece);
        }

        private static bool CanTouch(Tile tile) {
            if (tile.Piece == null || tile.Piece.CanAddMemory == false) return false;
            if (tile.AdjacentTiles.All(t => t != null && t.CanPassThrough() == false)) return false;
            if (tile.Entities.Where(e => e.Layer > Layer.Piece).Any(e => e.PreventTouch)) return false;

            return true;
        }

        private static IEnumerable<ColorIndex> GetThreeColors(List<Entity> colorIndices) {
            return colorIndices.Where(e => e is NormalPiece)
                .GroupBy(e => e.Color)
                .Where(g => g.Count() >= 3)
                .Select(g => g.Key)
                .ToArray();
        }
    }
}
