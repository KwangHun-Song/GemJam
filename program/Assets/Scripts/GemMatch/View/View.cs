using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GemMatch {
    public class View : MonoBehaviour, IControllerEvent {
        [SerializeField] private Transform tileViewRoot;
        [SerializeField] private Transform memoryViewRoot;
        [SerializeField] private TMP_Text gameStatusText;
        
        [Header("MonoBehaviour를 상속한 AbilityView들은 여기에!")]
        [SerializeField] private ShuffleAbilityView shuffleAbilityView;

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
            { AbilityIndex.ShuffleAbility, shuffleAbilityView }
        };

        public void OnStartGame(Controller controller) {
            Controller = controller;
            var tiles = controller.Tiles;
            
            for (int i = 0; i < TileViews.Length; i++) {
                TileViews[i].Initialize(this, tiles[i]);
            }

            foreach (var tileView in TileViews) {
                foreach (var entityView in tileView.EntityViews.Values) {
                    entityView.OnCreate().Forget();
                }
            }

            foreach (var memoryView in MemoryViews) {
                memoryView.Initialize();
            }

            gameStatusText.text = "";
        }

        public void OnClearGame(Mission[] missions) {
            gameStatusText.text = "Completed!";
        }

        public void OnFailGame(Mission[] missions) {
            gameStatusText.text = "failed!";
        }

        public void OnReplayGame(Mission[] missions) { }

        public void OnChangeMission(Mission mission, int changeCount) {
            // TODO : 구현님 투두리스트에서 이곳에 구현 부탁드려요
        }

        public void OnMoveToMemory(Tile tile, Entity entity) {
            AddMemoryAsync().Forget();

            async UniTask AddMemoryAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var entityView = tileView.EntityViews.Values.Single(ev => ReferenceEquals(ev.Entity, entity));
                var memoryView = MemoryViews.First(v => v.IsEmpty());
                
                // 엔티티뷰의 부모를 타일에서 메모리로 바꾼다.
                entityView.transform.SetParent(memoryView.CellRoot);
                entityView.transform.localPosition = Vector3.zero;

                // 타일뷰 소속에서 해당 엔티티뷰를 제거한다.
                tileView.EntityViews.Remove(entity.Layer);
                
                // 메모리뷰 소속에서는 엔티티뷰를 추가한다.
                await memoryView.AddEntityAsync(entityView);
                
                // 메모리를 정렬한다.
                await SortMemoryAsync();
            }
        }

        public void OnMoveFromMemory(Tile tile, Entity entity) {
            MoveToTileAsync().Forget();

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
            CreateMemoryAsync().Forget();
            
            async UniTask CreateMemoryAsync() {
                var firstEmptyMemoryView = MemoryViews.First(v => v.EntityView == null);
                var entityView = CreateEntityView(entity);
                entityView.transform.SetParent(firstEmptyMemoryView.CellRoot);
                entityView.transform.localPosition = Vector3.zero;
                if (entityView is NormalPieceView normalPieceView) normalPieceView.SetForSlotUI(true);

                await firstEmptyMemoryView.AddEntityAsync(entityView);
                await SortMemoryAsync();
            }
        }

        public void OnDestroyMemory(Entity entity) {
            RemoveMemoryAsync().Forget();

            async UniTask RemoveMemoryAsync() {
                await MemoryViews
                    .Single(v => v.EntityView != null && ReferenceEquals(v.EntityView.Entity, entity))
                    .RemoveEntityAsync(true);
                await SortMemoryAsync();
            }
        }

        public void OnRunAbility(Ability ability) {
            if (AbilityViews.ContainsKey(ability.Index) == false) return;
            AbilityViews[ability.Index].RunAbilityAsync(this, ability, Controller).Forget();
        }

        public void OnCreateEntity(Tile tile, Entity entity) {
            var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
            var entityView = CreateEntityView(entity, tileView);
            tileView.EntityViews.Add(entity.Layer, entityView);
        }

        public void OnDestroyEntity(Tile tile, Entity entity) {
            var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
            var entityView = tileView.EntityViews.Values.Single(ev => ReferenceEquals(ev.Entity, entity));

            tileView.EntityViews.Remove(entity.Layer);
            DestroyImmediate(entityView.gameObject);
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

        public void OnClickEntity(Entity entity) {
            Controller.Input(Controller.GetTile(entity).Index);
        }

        internal virtual EntityView CreateEntityView(Entity entity, TileView tileView = null) {
            var prefab = Resources.Load<EntityView>(entity.Index.ToString());
            var parent = tileView == null ? transform : tileView.entitiesRoot;
            var entityView = Instantiate(prefab, parent, true);

            entityView.transform.localPosition = Vector3.zero;
            entityView.transform.localScale = Vector3.one;
            entityView.Initialize(tileView, entity);
            entityView.OnCreate().Forget();

            return entityView;
        }
    }
}