using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Assertions;

namespace GemMatch {
    public enum GameResult { Clear, Fail }
    public enum ColorIndex { None = 0, Red, Orange, Yellow, Green, Blue, Purple, Brown, Pink, Cyan, Random }
    
    public class Controller {
        private const int MaxMemoryCount = 7;
        
        private UniTaskCompletionSource<GameResult> gameCompletionSource;
        public List<IControllerEvent> Listeners { get; } = new List<IControllerEvent>();

        public Level CurrentLevel { get; protected set; }
        public Tile[] Tiles { get; protected set; }
        public Mission[] Missions { get; protected set; }
        
        public List<Entity> Memory { get; protected set; }
        public HashSet<Tile> ActiveTiles { get; protected set; } = new HashSet<Tile>();

        public void StartGame(Level level) {
            // 초기화
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            Tiles = level.tiles.Select(tileModel => new Tile(tileModel.Clone())).ToArray();
            CalculateRandomColors();

            // 게임 시작
            gameCompletionSource = new UniTaskCompletionSource<GameResult>();

            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnStartGame(this);
            
            // Active 타일들을 한 번 계산해준다.
            CalculateActiveTiles();
        }

        public async UniTask<GameResult> WaitUntilGameEnd() {
            return await gameCompletionSource.Task;
        }

        public virtual void Input(int tileIndex) {
            if (Memory.Count >= MaxMemoryCount) return;

            var tile = Tiles[tileIndex];

            if (CanTouch(tile) == false) return;
            
            // 타일 터치 처리
            Touch(tile);
            
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
            if (y > GetTopY()) return null;

            return Tiles[y * Constants.Width + x];
        }

        public int GetTopY() => Tiles.Max(t => t.Y); // 0 based. 최적화가 필요하면 캐싱하자.

        public Tile GetTile(Entity entity) => Tiles.SingleOrDefault(t => t.Entities.Any(e => ReferenceEquals(e, entity)));

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

        private void CalculateRandomColors() {
            var randomEntityCount = CurrentLevel.tiles
                .SelectMany(tm => tm.entityModels)
                .Count(em => em.color == ColorIndex.Random);
            var colorCount = CurrentLevel.colorCount;
            var availableColors = System.Enum.GetValues(typeof(ColorIndex)).Cast<ColorIndex>().Take(colorCount).ToList();
            var colorsQueue = GenerateColorQueue(randomEntityCount, availableColors);
            
            // TODO : 컬러 배치 작업중...
        }

        public static Queue<ColorIndex> GenerateColorQueue(int queueCount, List<ColorIndex> colors) {
            // 결과는 3의 배수여야 한다.
            Assert.AreEqual(0, queueCount % 3);
            
            var setsCount = queueCount / 3;
            var colorSets = new List<ColorIndex>();
            
            // 적어도 한 번씩은 선택되어야 하므로 기존 컬러들 하나씩 선택
            colorSets.AddRange(colors.Take(setsCount));
            
            // 더 필요한 만큼 colors에서 랜덤으로 뽑아서 추가
            for (int i = 0; i < setsCount - colors.Count; i++) {
                colorSets.Add(colors.PickRandom());
            }

            // 컬러를 세 개씩 하면, 이제 queue에 들어갈 컬러들이 된다.
            var colorItems = colorSets.SelectMany(color => Enumerable.Repeat(color, 3)).ToList();
            // 개수는 같아야 한다.
            Assert.AreEqual(queueCount, colorItems.Count);
            
            // 결과물은 여기에 저장할 것이다.
            var stack = new Stack<ColorIndex>();

            int safeStop = 0;
            int slots = 7; // 여기에 가능한 슬롯인지 체크한다.
            while (stack.Count < queueCount && safeStop < 5000) {
                safeStop++;
                // 랜덤으로 하나를 선택한다.
                var candidate = colorItems.PickRandom();

                // 이번 색깔을 넣어서, 그 색깔이 3의 배수가 된다면 추가한다. 
                if ((stack.Count(c => c == candidate) + 1) % 3 == 0) {
                    stack.Push(candidate);
                    colorItems.Remove(candidate);
                    slots += 3;
                }

                // 슬롯에 2칸 이상 남았다면 색깔을 추가한다.
                if (slots - stack.Count > 1) {
                    stack.Push(candidate);
                    colorItems.Remove(candidate);
                } else {
                    // 실패했으면 스택에서 세 개를 뺀다.
                    colorItems.Add(stack.Pop());
                    colorItems.Add(stack.Pop());
                    colorItems.Add(stack.Pop());
                }
            }

            return new Queue<ColorIndex>(stack.Reverse());
        }

        private void CalculateActiveTiles() {
            var newActiveTiles = Tiles
                .Where(t => !ActiveTiles.Contains(t) && CanTouch(t))
                .ToArray();
            
            ActiveTiles.AddRange(newActiveTiles);
            foreach (var listener in Listeners) listener.OnAddActiveTiles(newActiveTiles);
        }

        private bool CanTouch(Tile tile) {
            if (tile.Piece == null || tile.Piece.CanAddMemory() == false) return false;
            if (tile.Entities.Where(e => e.Layer > Layer.Piece).Any(e => e.PreventTouch())) return false;
            if (HasPathToTop(tile) == false) return false;

            return true;
        }

        public bool HasPathToTop(Tile startTile) {
            // 시작 타일이 가장 상단이면 바로 true 반환
            if (startTile.Y == GetTopY()) return true;
            
            // 방문한 타일들을 추적하기 위한 집합, 시작 타일을 방문한 것으로 표시
            var visitedTiles = new HashSet<Tile> { startTile };

            // 인접한 타일들을 검사
            foreach (var adjacentTile in GetAdjacentTiles(startTile)) {
                // 이웃 타일로의 경로가 있는지 DFS
                if (FindPathToTopDfs(adjacentTile, visitedTiles)) {
                    return true;
                }
            }
    
            // 모든 가능성을 다 찾아봤지만 경로가 없다면 false 반환
            return false;
        }

        private bool FindPathToTopDfs(Tile tile, HashSet<Tile> visitedTiles) {
            // 이미 방문한 타일이라면 바로 반환
            if (visitedTiles.Contains(tile)) return false;
            visitedTiles.Add(tile); // 이 타일을 방문한 것으로 표시

            // 타일이 위쪽 가장자리에 도달했다면 true 반환
            if (tile.Y == GetTopY()) return true;

            // 이 타일이 점유되어 있다면 경로가 없는 것으로 간주
            if (tile.CanPassThrough() == false) return false;

            // 인접한 타일들을 검사
            foreach (var adjacentTile in GetAdjacentTiles(tile)) {
                // 이웃 타일로의 경로가 있는지 DFS
                if (FindPathToTopDfs(adjacentTile, visitedTiles)) {
                    return true;
                }
            }

            // 모든 가능성을 다 찾아봤지만 경로가 없다면 false 반환
            return false;
        }

        private void Touch(Tile tile) {
            var piece = tile.Piece;

            // 먼저 타일을 Hit 처리
            Hit(tile);
            
            // 메모리로 이동시킨다.
            MoveToMemory(tile);
            
            // 메모리에서 같은 색깔 세 개가 있을 경우 파괴한다.
            TryRemoveFromMemory(piece);
        }

        private void Hit(Tile tile) {
            tile.Hit();
            
            // 주변 타일에 SplashHit
            foreach (var adjacentTile in GetAdjacentTiles(tile)) {
                adjacentTile.Hit();
            }
        }

        private void MoveToMemory(Tile tile) {
            var piece = tile.Piece;
            Memory.Add(piece);
            tile.RemoveLayer(Layer.Piece);
            foreach (var listener in Listeners) listener.OnMoveToMemory(tile, piece);
            
            CalculateActiveTiles();
        }

        private bool TryRemoveFromMemory(Entity piece) {
            var color = piece.Color;
            var targetEntitiesInMemory = Memory
                .Where(e => e is NormalPiece np && np.Color == color)
                .ToArray();
            
            // 같은 색깔이 세 개여야 제거할 수 있다.
            if (targetEntitiesInMemory.Count() < 3) {
                return false;
            }

            // 메모리에서 엔티티를 제거
            foreach (var e in targetEntitiesInMemory.Take(3)) {
                Memory.Remove(e);
                foreach (var listener in Listeners) listener.OnRemoveMemory(e);
            }
            
            // 미션 증가
            var mission = Missions.SingleOrDefault(m => m.entity.Equals(piece.Model));
            if (mission != null) mission.count += 3;

            return true;
        }

        private IEnumerable<Tile> GetAdjacentTiles(Tile tile) {
            if (tile.X > 0 && GetTile(tile.X - 1, tile.Y) != null) 
                yield return GetTile(tile.X - 1, tile.Y);
            if (tile.X < Constants.Width - 1 && GetTile(tile.X + 1, tile.Y) != null) 
                yield return GetTile(tile.X + 1, tile.Y);
            if (tile.Y > 0 && GetTile(tile.X, tile.Y - 1) != null) 
                yield return GetTile(tile.X, tile.Y - 1);
            if (tile.Y < GetTopY() && GetTile(tile.X, tile.Y + 1) != null) 
                yield return GetTile(tile.X, tile.Y + 1);
        }
    }
}
