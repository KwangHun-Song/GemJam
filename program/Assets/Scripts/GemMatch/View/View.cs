using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OverlayStatusSystem;
using UnityEngine;
using Utility;

namespace GemMatch {
    public class View : MonoBehaviour, IControllerEvent {
        [SerializeField] private Transform tileViewRoot;
        [SerializeField] private Transform memoryViewRoot;
        [SerializeField] private Transform extraSlot;
        [SerializeField] private TileViewScaler viewScaler;
        
        [Header("MonoBehaviour를 상속한 AbilityView들은 여기에!")]
        [SerializeField] private ShuffleAbilityView shuffleAbilityView;
        [SerializeField] private MagneticAbilityView magneticAbilityView;

        private TileView[] tileViews;
        public TileView[] TileViews => tileViews ??= tileViewRoot.GetComponentsInChildren<TileView>();

        private MemoryView[] memoryViews;
        public MemoryView[] MemoryViews {
            get { return memoryViews ??= memoryViewRoot.GetComponentsInChildren<MemoryView>(); }
            private set => memoryViews = value;
        }

        private Controller Controller { get; set; }

        private Dictionary<AbilityIndex, IAbilityView> abilityViews;
        private Dictionary<AbilityIndex, IAbilityView> AbilityViews => abilityViews ??= new Dictionary<AbilityIndex, IAbilityView> {
            { AbilityIndex.MagneticAbility, magneticAbilityView },
            { AbilityIndex.ShuffleAbility, shuffleAbilityView },
            { AbilityIndex.RocketAbility, new RocketAbilityView() },
        };

        private SoundName[] characterVoices = new[] {
            SoundName.huhu,
            SoundName.yeah,
            SoundName.yeah_huh
        };

        #region 순서대로 실행되어야 하는 연출 관리

        private Queue<Func<UniTask>> AnimationQueue { get; } = new Queue<Func<UniTask>>();
        private bool OnAnimation { get; set; }

        private void EnqueueAnimation(Func<UniTask> anim) {
            AnimationQueue.Enqueue(anim);
            TryTriggerAnimationAsync().Forget();
        }

        private async UniTask TryTriggerAnimationAsync() {
            if (OnAnimation) return;
            OnAnimation = true;
            
            while (AnimationQueue.Any()) {
                await AnimationQueue.Dequeue().Invoke();
            }

            OnAnimation = false;
        }

        #endregion

        // (주의) 아직 Page GameObject가 만들어지기 전이므로 component 호출은 지양한다
        public void OnStartGame(Controller controller) {
            Controller = controller;
            var tiles = controller.Tiles;
            
            for (int i = 0; i < TileViews.Length; i++) {
                TileViews[i].Initialize(this, tiles[i]);
            }
            
            DrawEdges();
            viewScaler.SetPlayViewPosition(controller.Tiles);

            foreach (var tileView in TileViews) {
                foreach (var entityView in tileView.EntityViews.Values) {
                    entityView.OnCreate().Forget();
                }
            }

            foreach (var memoryView in MemoryViews) {
                memoryView.Initialize();
            }
        }

        public void OnClearGame(Mission[] missions) {
            // todo: 성공팝업
        }

        public void OnFailGame(Mission[] missions) {
            // todo: 실패팝업
        }

        public void OnReplayGame(Mission[] missions) { }

        public void OnChangeMission(Mission mission, int changeCount) {
            SimpleSound.Play(characterVoices.PickRandom());
            OverlayStatusHelper.UpdateMissionCount(mission, changeCount);
        }

        public void OnMoveToMemory(Tile tile, Entity entity) {
            EnqueueAnimation(AddMemoryAsync);
            
            async UniTask AddMemoryAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var entityView = tileView.EntityViews.Values.Single(ev => ReferenceEquals(ev.Entity, entity));
                var memoryView = MemoryViews.First(v => v.IsEmpty());

                await entityView.OnMoveMemory();

                // 타일뷰 소속에서 해당 엔티티뷰를 제거한다.
                tileView.RemoveEntityView(entityView);
                
                // 엔티티뷰의 부모를 타일에서 메모리로 바꾼다.
                entityView.transform.SetParent(memoryView.CellRoot);
                entityView.transform.localPosition = Vector3.zero;
                
                // 메모리뷰 소속에서는 엔티티뷰를 추가한다.
                await memoryView.AddEntityAsync(entityView);
                
                // 메모리를 정렬한다.
                await SortMemoryAsync();
            }
        }

        public void OnMoveFromMemory(Tile tile, Entity entity) {
            EnqueueAnimation(MoveToTileAsync);

            async UniTask MoveToTileAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var memoryView = MemoryViews.Single(mv => ReferenceEquals(mv.EntityView?.Entity, entity));
                var entityView = memoryView.EntityView;
                
                // 엔티티뷰의 부모를 메모리에서 타일로 바꾼다.
                entityView.transform.SetParent(tileView.entitiesRoot);
                
                // 메모리 소속에서 해당 엔티티뷰를 제거한다.
                await memoryView.RemoveEntityAsync(false);
                
                // 타일뷰 소속에 엔티티뷰를 추가한다.
                tileView.AddEntityView(entityView);
                
                // 메모리를 정렬한다.
                await SortMemoryAsync();
            }
        }

        public void OnCreateMemory(Entity entity) {
            EnqueueAnimation(CreateMemoryAsync);
            
            async UniTask CreateMemoryAsync() {
                var firstEmptyMemoryView = MemoryViews.First(v => v.EntityView == null);
                var entityView = CreateEntityView(entity);
                if (entityView is NormalPieceView normalPieceView) normalPieceView.SetOnMemoryUI(true);

                await firstEmptyMemoryView.AddEntityAsync(entityView);
                await SortMemoryAsync();
            }
        }

        public void OnDestroyMemory(Entity entity) {
            EnqueueAnimation(RemoveMemoryAsync);

            async UniTask RemoveMemoryAsync() {
                SimpleSound.Play(SoundName.get_ascending);
                await MemoryViews
                    .Single(v => v.EntityView != null && ReferenceEquals(v.EntityView.Entity, entity))
                    .RemoveEntityAsync(true);
                await SortMemoryAsync();
            }
        }

        public void OnRunAbility(IAbility ability) {
            if (AbilityViews.ContainsKey(ability.Index) == false) return;
            EnqueueAnimation(() => AbilityViews[ability.Index].RunAbilityAsync(this, ability, Controller));
        }

        public void OnRestoreAbility(IAbility ability) {
            if (AbilityViews.ContainsKey(ability.Index) == false) return;
            EnqueueAnimation(() => AbilityViews[ability.Index].RestoreAbilityAsync(this, ability, Controller));
        }

        public void OnCreateEntity(Tile tile, Entity entity) {
            EnqueueAnimation(CreateEntityAsync);

            UniTask CreateEntityAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var entityView = CreateEntityView(entity);
                tileView.AddEntityView(entityView);
                
                return UniTask.CompletedTask;
            }
        }

        public void OnDestroyEntity(Tile tile, Entity entity) {
            EnqueueAnimation(DestroyEntityAsync);

            UniTask DestroyEntityAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var entityView = tileView.EntityViews.Values.Single(ev => ReferenceEquals(ev.Entity, entity));

                tileView.RemoveEntityView(entityView);
                DestroyImmediate(entityView.gameObject);
                
                return UniTask.CompletedTask;
            }
        }

        public void OnAddActiveTiles(IEnumerable<Tile> activeTiles) {
            foreach (var tileView in TileViews) {
                var isActive = activeTiles.Contains(tileView.Tile);
                var receiveEVs = tileView.EntityViews.Values.OfType<IReceiveActivation>();
                
                foreach (var iReceive in receiveEVs) {
                    iReceive.OnActive(isActive);
                }
            }
        }

        public void OnAddExtraSlot() {
            AddExtraSlotAsync().Forget();

            async UniTask AddExtraSlotAsync() {
                await UniTask.Delay(1000);
                extraSlot.DOScale(Vector3.zero, 0.3F).SetEase(Ease.InBack, 3);
            }
        }

        public void OnRemoveExtraSlot() {
            extraSlot.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutBack, 3);
        }

        public async UniTask SortMemoryAsync() {
            var sorted = MemoryViews
                .OrderBy(mv => mv.EntityView == null ? 1 : 0)
                .ThenBy(mv => mv.EntityView?.Entity.Color ?? 0)
                .ToArray();
            MemoryViews = sorted.ToArray();

            for (int i = MemoryViews.Length - 1; i >= 0; i--) {
                MemoryViews[i].transform.SetAsFirstSibling();
            }
        }

        public TileView GetTileView(Tile tile) {
            return TileViews.FirstOrDefault(tv => tv.Tile == tile);
        }

        public bool IsRightTopTileOf4ClosedTiles(Tile tile) {
            if (tile.IsOpened) return false;
            if (tile.X == 0) return false;
            if (tile.Y == 0) return false;

            var left = Controller.GetTile(tile.X - 1, tile.Y);
            var bottomTile = Controller.GetTile(tile.X, tile.Y - 1);
            var leftBottomTile = Controller.GetTile(tile.X - 1, tile.Y - 1);

            if (left.IsOpened || bottomTile.IsOpened || leftBottomTile.IsOpened) return false;
            if (GetTileView(left).IsShowingDeco) return false;
            if (GetTileView(bottomTile).IsShowingDeco) return false;
            if (GetTileView(leftBottomTile).IsShowingDeco) return false;

            return true;
        }

        public void OnClickEntity(Entity entity) {
            Controller.Input(Controller.GetTile(entity).Index);
        }

        private void DrawEdges() {
            foreach(var tileView in TileViews) {
                tileView.DrawEdges(Controller.Tiles);
            }
        }

        internal virtual EntityView CreateEntityView(Entity entity) {
            var prefab = Resources.Load<EntityView>(entity.Index.ToString());
            var entityView = Instantiate(prefab, null, true);
            entityView.Initialize(null, entity);
            entityView.OnCreate().Forget();

            return entityView;
        }
    }
}