using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace GemMatch {
    public enum GameResult { Clear, Fail }
    public enum ColorIndex { None = 0, Red, Orange, Yellow, Green, Blue, Purple, Random }
    
    public class Controller {
        private const int MaxMemoryCount = 7;
        
        private UniTaskCompletionSource<GameResult> gameCompletionSource;
        public List<IControllerEvent> Listeners { get; } = new List<IControllerEvent>();

        public Level CurrentLevel { get; private set; }
        public Tile[] Tiles { get; private set; }
        public List<Entity> Memory { get; private set; }
        public Mission[] Missions { get; private set; }

        public void StartGame(Level level) {
            // 초기화
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            var randomColorCount = level.tiles
                .Where(m => m.entityModels[0].color == ColorIndex.Random)
                .Count();
            var randomQue = MathUtility.Create3MatchColorQueue(randomColorCount);
            // random color에 부를때마다 색 부여
            Tiles = level.tiles.Select(tileModel => {
                var m = tileModel.Clone();
                var c = m.entityModels[0].color;
                if (c == ColorIndex.Random) c = randomQue.Dequeue();
                m.entityModels[0].color = c;
                return new Tile(m);
            }).ToArray();
            foreach (var tile in Tiles) tile.Initialize(this);

            // 게임 시작
            gameCompletionSource = new UniTaskCompletionSource<GameResult>();

            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnStartGame(this);
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
            if (IsFailed()) {
                FailGame();
            }
        }

        public void ClearGame() {
            gameCompletionSource.TrySetResult(GameResult.Clear);
            
            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnClearGame(Missions);
        }

        public void FailGame() {
            gameCompletionSource.TrySetResult(GameResult.Fail);
            
            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnFailGame(Missions);
        }

        public void ReplayGame() {
            StartGame(CurrentLevel);
            
            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnReplayGame(Missions);
        }

        public Tile GetTile(int x, int y) {
            if (x < 0) return null;
            if (x > Constants.Width - 1) return null;
            if (y < 0) return null;
            if (y * Constants.Width + x > Tiles.Length - 1) return null;

            return Tiles[y * Constants.Width + x];
        }

        public static Entity GetEntity(EntityModel entityModel) {
            return entityModel.index switch {
                EntityIndex.NormalPiece => new NormalPiece(entityModel),
            };
        }

        private bool IsCleared() {
            return !Missions.Where((mission, index) => CurrentLevel.missions[index].count > mission.count).Any();
        }

        private bool IsFailed() {
            return Memory.Count >= MaxMemoryCount;
        }

        private static bool CanTouch(Tile tile) {
            if (tile.Piece == null || tile.Piece.CanAddMemory() == false) return false;
            if (tile.Entities.Where(e => e.Layer > Layer.Piece).Any(e => e.PreventTouch())) return false;

            return HasPathToTop(tile);
        }

        private static bool HasPathToTop(Tile startTile) {
            // 방문한 타일들을 추적하기 위한 집합
            var visitedTiles = new HashSet<Tile>();
    
            // DFS를 시작
            return FindPathToTopDfs(startTile, visitedTiles);
        }

        private static bool FindPathToTopDfs(Tile tile, HashSet<Tile> visitedTiles) {
            // 이미 방문한 타일이라면 바로 반환, 이 타일을 방문한 것으로 표시
            if (visitedTiles.Contains(tile)) return false;
            visitedTiles.Add(tile);

            // 타일이 위쪽 가장자리에 도달했다면 true 반환
            if (tile.Up == null) return true;

            // 이 타일이 점유되어 있다면 경로가 없는 것으로 간주
            if (tile.CanPassThrough() == false) return false;

            // 인접한 타일들을 검사
            foreach (var adjacentTile in tile.AdjacentTiles) {
                if (adjacentTile == null) continue;
                    
                // 이웃 타일로의 경로가 있는지 DFS
                if (FindPathToTopDfs(adjacentTile, visitedTiles)) {
                    return true;
                }
            }

            // 모든 가능성을 다 찾아봤지만 경로가 없다면 false 반환
            return false;
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
                    foreach (var listener in Listeners) listener.OnRemoveMemory(this, piece);
                }
            }
        }

        private void AddToMemory(Entity piece) {
            Memory.Add(piece);
            foreach (var listener in Listeners) listener.OnAddMemory(this, piece);
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
