using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GemMatch.UndoSystem;
using OverlayStatusSystem;

namespace GemMatch {
    public class Controller {
        protected int MaxMemoryCount => 7 + (ExtraMemorySlot ? 1 : 0);
        
        private UniTaskCompletionSource<GameResult> gameCompletionSource;
        public List<IControllerEvent> Listeners { get; } = new List<IControllerEvent>();

        public Level CurrentLevel { get; protected set; }
        public Tile[] Tiles { get; protected set; }
        public Mission[] Missions { get; protected set; }
        public Mission GetMission(EntityIndex entityIndex, ColorIndex colorIndex)
            => Missions.SingleOrDefault(m => m.entity.index == entityIndex && m.entity.color == colorIndex);
        public List<Entity> Memory { get; protected set; }
        public List<Tile> ActiveTiles { get; protected set; } = new List<Tile>();

        public virtual PathFinder PathFinder { get; protected set; }
        public virtual IColorDistributor ColorDistributor { get; protected set; }
        public UndoHandler UndoHandler { get; protected set; }
        
        public bool ExtraMemorySlot { get; private set; }

        public virtual void StartGame(Level level, bool isReplay = false) {
            // 초기화
            CurrentLevel = level;
            Memory = new List<Entity>();
            Missions = level.missions.Select(m => m.Clone()).ToArray();
            OverlayStatusHelper.InitializeMissionsAsync(Missions).Forget();
            OverlayStatusHelper.UpdateLevelStatus(CurrentLevel);
            Tiles = level.tiles.Select(tileModel => new Tile(tileModel.Clone())).ToArray();
            PathFinder = new PathFinder(Tiles);
            
            if (isReplay) {
                UndoHandler.Reset();
                RemoveExtraMemorySlot();
            } else {
                ColorDistributor = new ClearableColorDistributor();
                UndoHandler = new UndoHandler();
            }

            // 랜덤 컬러인 노멀 피스들의 컬러들을 배치해준다.
            if (ColorDistributor.DistributeColors(CurrentLevel, Tiles) == false) {
                // 실패시 색깔을 랜덤으로 배치한다.
                new RandomColorDistributor().DistributeColors(CurrentLevel, Tiles);
            }

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

            // 타일 터치 처리
            var tile = Tiles[tileIndex];
            if (CanTouch(tile) == false) return;
            Touch(tile);
            
            // 게임 종료조건 검사
            if (IsCleared()) ClearGame();
            if (IsFailed()) FailGame();
        }

        public void InputAbility(IAbility ability, bool triggeredByPrev = false, bool isViewFirst = false) {
            if (ability == null) return;
            
            UnityEngine.Debug.Log(ability.Index);
            if (ability is DiscountMissionAbility discountMission)
                InputChangeMission(discountMission.TargetMission.entity, discountMission.TargetMission.count);
            else
                UndoHandler.Do(new AbilityCommand(this, ability, triggeredByPrev));
            
            foreach (var subAbility in ability.GetCascadedAbility()) {
                InputAbility(subAbility, true);
            }
            
            // 게임 종료조건 검사
            if (IsCleared()) ClearGame();
            if (IsFailed()) FailGame();
        }

        public void AddExtraMemorySlot() {
            ExtraMemorySlot = true;
            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnAddExtraSlot();
        }

        public void RemoveExtraMemorySlot() {
            ExtraMemorySlot = false;
            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnRemoveExtraSlot();
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
            StartGame(CurrentLevel, true);
            // 이벤트 전달
            foreach (var listener in Listeners) listener.OnReplayGame(Missions);
        }

        public Tile GetTile(Entity entity) => Tiles.SingleOrDefault(t => t.Entities.Values.Any(e => ReferenceEquals(e, entity)));
        public Tile GetTile(int x, int y) => Tiles[y * Constants.Width + x];


        private bool IsMissionChanged() {
            return !Missions.Where((mission, index) => CurrentLevel.missions[index].count >= mission.count).Any();
        }

        protected virtual bool IsCleared() {
            if (Tiles.SelectMany(t => t.Entities.Values).Any(e => e is NormalPiece) == false && Memory.Any() == false) return true;
            if (Missions.All(mission => mission.count <= 0)) return true;

            return false;
        }

        protected bool IsFailed() {
            return Memory.Count >= MaxMemoryCount;
        }

        public virtual bool CanTouch(Tile tile) {
            if (tile.Piece == null || tile.Piece.CanAddMemory() == false) return false;
            if (tile.Entities.Values.Where(e => e.Layer > Layer.Piece).Any(e => e.PreventTouch())) return false;
            if (PathFinder.HasPathToTop(tile) == false) return false;

            return true;
        }

        protected void Touch(Tile tile) {
            var piece = tile.Piece;
            // 앵커 역할을 할 커맨드를 하나 추가한다. 이 함수 내에서 사용될 Command는 자동 트리거된다(함께 언두시키기 위해).
            UndoHandler.Do(new Command(null, null));

            // 먼저 타일을 SplashHit 처리
            SplashHit(tile, true);
            
            // 메모리로 이동시킨다.
            MoveToMemory(tile);
            
            // 활성화된 타일들을 다시 계산한다.
            CalculateActiveTiles();

            CheckAndAchieveGoalPiece();
            
            // 메모리에서 같은 색깔 세 개가 있을 경우 파괴한다.
            TryRemoveFromMemory(piece);
        }

        public void SplashHit(Tile targetTile, bool triggeredByPrev = false) {
            foreach (var adjacentTile in TileUtility.GetAdjacentTiles(targetTile, Tiles)) {
                Hit(adjacentTile, triggeredByPrev);
            }
        }

        private void Hit(Tile tile, bool triggeredByPrev = false) {
            foreach (var entity in tile.Entities.Values) {
                if (!entity.CanBeHit()) continue;
                
                var hitInfo = entity.Hit();
                if (hitInfo.hitResult == HitResult.Destroyed) {
                    UndoHandler.Do(new HitCommand(this, tile, entity, triggeredByPrev));
                }
                    
                if (hitInfo.prevent) break;
            }
        }

        protected void MoveToMemory(Tile tile) {
            var piece = tile.Piece;
            UndoHandler.Do(new Command<Entity>(
                @do: () => {
                    Memory.Add(piece);
                    tile.RemoveLayer(Layer.Piece);
                    foreach (var listener in Listeners) listener.OnMoveToMemory(tile, piece);
                }, 
                undo: movedEntity => {
                    tile.AddEntity(movedEntity);
                    Memory.Remove(movedEntity);
                    foreach (var listener in Listeners) listener.OnMoveFromMemory(tile, piece);
                    CalculateActiveTiles();
                }, 
                param: piece,
                triggeredByPrev: true
            ));
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
            foreach (var entityToRemove in targetEntitiesInMemory.Take(3)) {
                UndoHandler.Do(new Command<Entity>(
                    @do: () => {
                        Memory.Remove(entityToRemove);
                        foreach (var listener in Listeners) listener.OnDestroyMemory(entityToRemove);
                    }, 
                    undo: removedEntity => {
                        Memory.Add(removedEntity);
                        foreach (var listener in Listeners) listener.OnCreateMemory(removedEntity);
                    }, 
                    param: entityToRemove,
                    triggeredByPrev:true
                    ));
            }
            
            // 미션 증가
            InputChangeMission(piece.Model, 3);

            return true;
        }

        private void InputChangeMission(EntityModel entityModel, int discount) {
            var mission = Missions.SingleOrDefault(m => m.entity.Equals(entityModel))
                        ?? Missions.SingleOrDefault(m => m.entity.index == EntityIndex.NormalPiece && m.entity.color == entityModel.color)
                        ?? Missions.SingleOrDefault(m => m.entity.index == EntityIndex.NormalPiece && m.entity.color == ColorIndex.All);
            if (mission != null) {
                UndoHandler.Do(new MissionCommand(this, mission, discount, true));
            }
        }

        public void CalculateActiveTiles() {
            ActiveTiles = Tiles.Where(CanTouch).ToList();
            foreach (var listener in Listeners) listener.OnAddActiveTiles(ActiveTiles);
        }

        private void CheckAndAchieveGoalPiece() {
            if (ActiveTiles.Any(t => t.Piece is GoalPiece) == false) return;
            
            var mission = Missions.SingleOrDefault(m => m.entity.index == EntityIndex.GoalPiece);
            if (mission == null) return;

            var originMissionCount = CurrentLevel.missions.Single(m => m.entity.index == EntityIndex.GoalPiece).count;
            var activeGoalCount = ActiveTiles.SelectMany(t => t.Entities.Values).Count(e => e is GoalPiece);
            var remainMissionCount = originMissionCount - activeGoalCount;

            if (mission.count > remainMissionCount) {
                var earnCount = mission.count - remainMissionCount;
                UndoHandler.Do(new MissionCommand(this, mission, earnCount, true));
            }
        }
    }
}
