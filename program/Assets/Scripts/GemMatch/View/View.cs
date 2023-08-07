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

        private TileView[] tileViews;
        public TileView[] TileViews => tileViews ??= tileViewRoot.GetComponentsInChildren<TileView>();

        private MemoryView[] memoryViews;
        public MemoryView[] MemoryViews {
            get { return memoryViews ??= memoryViewRoot.GetComponentsInChildren<MemoryView>(); }
            private set => memoryViews = value;
        }

        private Controller Controller { get; set; }
        
        public void OnStartGame(Controller controller) {
            Controller = controller;
            var tiles = controller.Tiles;
            
            for (int i = 0; i < TileViews.Length; i++) {
                TileViews[i].Initialize(this, tiles[i]);
            }

            foreach (var tileView in TileViews) {
                foreach (var entityView in tileView.EntityViews) {
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

        public void OnMoveToMemory(Tile tile, Entity entity) {
            AddMemoryAsync().Forget();

            async UniTask AddMemoryAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var entityView = tileView.EntityViews.Single(ev => ReferenceEquals(ev.Entity, entity));
                var memoryView = MemoryViews.First(v => v.IsEmpty());
                
                // 엔티티뷰의 부모를 타일에서 메모리로 바꾼다.
                entityView.transform.SetParent(memoryView.CellRoot);
                entityView.transform.localPosition = Vector3.zero;

                // 타일뷰 소속에서 해당 엔티티뷰를 제거한다.
                tileView.EntityViews.Remove(entityView);
                
                // 메모리뷰 소속에서는 엔티티뷰를 추가한다.
                await memoryView.AddEntityAsync(entityView);
                
                // 메모리를 정렬한다.
                await SortMemoryAsync();
            }
        }

        public void OnRemoveMemory(Entity entity) {
            RemoveMemoryAsync().Forget();

            async UniTask RemoveMemoryAsync() {
                await MemoryViews
                    .Single(v => v.EntityView != null && ReferenceEquals(v.EntityView.Entity, entity))
                    .RemoveEntityAsync();
                await SortMemoryAsync();
            }
        }

        public void OnRunAbility(Ability ability) { }
        public void OnDestroyEntity(Tile tile, Entity entity) {
            var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
            var entityView = tileView.EntityViews.Single(ev => ReferenceEquals(ev.Entity, entity));

            tileView.EntityViews.Remove(entityView);
            DestroyImmediate(entityView.gameObject);
        }

        public void OnAddActiveTiles(IEnumerable<Tile> tiles) {
            foreach (var entityView in TileViews.Where(tv => tiles.Contains(tv.Tile))
                         .SelectMany(tv => tv.EntityViews)
                         .Where(ev => ev is IReceiveActivation)) {
                ((IReceiveActivation)entityView).OnActive();
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

        internal EntityView CreateEntityView(Entity entity, Transform parent) {
            var prefab = Resources.Load<EntityView>(entity.Index.ToString());
            var view = Instantiate(prefab, parent, true);
            view.transform.localPosition = Vector3.zero;
            view.transform.localScale = Vector3.one;
            view.Initialize(null, entity);

            return view;
        }
    }
}