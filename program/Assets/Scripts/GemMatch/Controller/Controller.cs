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
            if (isReplay == false) {
                PathFinder = new PathFinder(Tiles);
                ColorDistributor = new ClearableColorDistributor();
                UndoHandler = new UndoHandler();
            } else {
                UndoHandler.Reset();
                RemoveExtraMemorySlot();
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
            UndoHandler.Do(new AbilityCommand(this, ability, triggeredByPrev));
            
            foreach (var subAbility in ability.GetCascadedAbility()) {
                InputAbility(subAbility, true);
            }
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

        protected virtual bool IsCleared() {
            if (ActiveTiles.Any(t => t.Piece is GoalPiece)) return true;
            if (Tiles.SelectMany(t => t.Entities.Values).Any(e => e is NormalPiece) == false && Memory.Any() == false) return true;
            if (!Missions.Where((mission, index) => CurrentLevel.missions[index].count >= mission.count).Any()) return true;

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
            
            // 메모리에서 같은 색깔 세 개가 있을 경우 파괴한다.
            TryRemoveFromMemory(piece);

            // mission count를 깍는다
            // CheckAndReduceMissionCount(piece);
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
            var mission = Missions.SingleOrDefault(m => m.entity.Equals(piece.Model));
            if (mission != null) {
                UndoHandler.Do(new Command<Mission>(
                    @do: () => {
                        mission.count -= 3;
                        foreach (var listener in Listeners) listener.OnChangeMission(mission, mission.count);
                    }, 
                    undo: addedMission => {
                        addedMission.count += 3;
                        foreach (var listener in Listeners) listener.OnChangeMission(addedMission, addedMission.count);
                    }, 
                    param: mission,
                    triggeredByPrev:true
                ));
            }

            return true;
        }


        protected void CheckAndReduceMissionCount(Entity entity) {
            UndoHandler.Do(new Command<Entity>(
                @do: () => {
                    var mission = Missions.SingleOrDefault(m => m.entity.Equals(entity.Model));
                    if (mission != null) {
                        mission.count -= 1;
                        foreach (var listener in Listeners) listener.OnChangeMission(mission, -1); // 봐야뎀
                    }
                },
                undo: movedEntity => {
                    var mission = Missions.SingleOrDefault(m => m.entity.Equals(entity.Model));
                    if (mission != null) {
                        mission.count -= 1;
                        foreach (var listener in Listeners) listener.OnChangeMission(mission, -1);
                    }
                    CalculateActiveTiles();
                },
                param: entity,
                triggeredByPrev: true
            ));
        }

        public void CalculateActiveTiles() {
            ActiveTiles = Tiles.Where(CanTouch).ToList();
            foreach (var listener in Listeners) listener.OnAddActiveTiles(ActiveTiles);
        }
    }
}
