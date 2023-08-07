using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace GemMatch {
    public class Controller {
        protected int MaxMemoryCount => 7;
        
        private UniTaskCompletionSource<GameResult> gameCompletionSource;
        public List<IControllerEvent> Listeners { get; } = new List<IControllerEvent>();

        public Level CurrentLevel { get; protected set; }
        public Tile[] Tiles { get; protected set; }
        public Mission[] Missions { get; protected set; }
        
        public List<Entity> Memory { get; protected set; }
        public List<Tile> ActiveTiles { get; protected set; } = new List<Tile>();

        public virtual PathFinder PathFinder { get; protected set; }
        public virtual ColorsDistributor ColorsDistributor { get; protected set; }

        public virtual void StartGame(Level level) {
            // 초기화
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => new Mission { entity = m.entity }).ToArray();
            Tiles = level.tiles.Select(tileModel => new Tile(tileModel.Clone())).ToArray();
            PathFinder = new PathFinder(Tiles);
            ColorsDistributor = new ColorsDistributor(CurrentLevel);
            
            // 랜덤 컬러인 노멀 피스들의 컬러들을 배치해준다.
            ColorsDistributor.DistributeClearableColors(Tiles);

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

        public void Input(int tileIndex) {
            if (Memory.Count >= MaxMemoryCount) return;

            // 타일 터치 처리
            var tile = Tiles[tileIndex];
            if (CanTouch(tile) == false) return;
            Touch(tile);
            
            // 게임 종료조건 검사
            if (IsCleared()) ClearGame();
            if (IsFailed()) FailGame();
        }

        public void InputAbility(Ability ability, Tile targetTile, object extraParam) {
            ability?.Run(this, targetTile, extraParam);
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

        public Tile GetTile(Entity entity) => Tiles.SingleOrDefault(t => t.Entities.Values.Any(e => ReferenceEquals(e, entity)));

        protected bool IsCleared() {
            return Tiles.Any(t => t.Entities.Any()) == false && Memory.Any() == false; // 임시로 미션과 관계 없이 모든 엔티티를 얻었는가를 목표로
            return !Missions.Where((mission, index) => CurrentLevel.missions[index].count >= mission.count).Any();
        }

        protected bool IsFailed() {
            return Memory.Count >= MaxMemoryCount;
        }

        public bool CanTouch(Tile tile) {
            if (tile.Piece == null || tile.Piece.CanAddMemory() == false) return false;
            if (tile.Entities.Values.Where(e => e.Layer > Layer.Piece).Any(e => e.PreventTouch())) return false;
            if (PathFinder.HasPathToTop(tile) == false) return false;

            return true;
        }

        protected void Touch(Tile tile) {
            var piece = tile.Piece;

            // 먼저 타일을 Hit 처리
            Hit(tile);
            
            // 메모리로 이동시킨다.
            MoveToMemory(tile);
            
            // 활성화된 타일들을 다시 계산한다.
            CalculateActiveTiles();
            
            // 메모리에서 같은 색깔 세 개가 있을 경우 파괴한다.
            TryRemoveFromMemory(piece);
        }

        protected void Hit(Tile tile) {
            tile.Hit();
            
            // 주변 타일에 SplashHit
            foreach (var adjacentTile in TileUtility.GetAdjacentTiles(tile, Tiles)) {
                adjacentTile.Hit();
            }
        }

        protected void MoveToMemory(Tile tile) {
            var piece = tile.Piece;
            Memory.Add(piece);
            tile.RemoveLayer(Layer.Piece);
            foreach (var listener in Listeners) listener.OnMoveToMemory(tile, piece);
        }

        protected bool TryRemoveFromMemory(Entity piece) {
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

        protected void CalculateActiveTiles() {
            var newActiveTiles = Tiles
                .Where(t => !ActiveTiles.Contains(t) && CanTouch(t))
                .ToArray();
            
            ActiveTiles.AddRange(newActiveTiles);
            foreach (var listener in Listeners) listener.OnAddActiveTiles(newActiveTiles);
        }
    }
}
