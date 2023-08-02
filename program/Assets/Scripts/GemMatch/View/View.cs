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
                    entityView.OnCreate(Controller).Forget();
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

        public void OnMoveToMemory(Controller controller, Tile tile, Entity entity) {
            AddMemoryAsync().Forget();

            async UniTask AddMemoryAsync() {
                var tileView = TileViews.Single(tv => ReferenceEquals(tv.Tile, tile));
                var entityView = tileView.EntityViews.Single(ev => ReferenceEquals(ev.Entity, entity));
                var memoryView = MemoryViews.First(v => v.IsEmpty());
                entityView.transform.SetParent(memoryView.CellRoot);
                entityView.transform.localPosition = Vector3.zero;

                tileView.EntityViews.Remove(entityView);
                await memoryView.AddEntityAsync(entityView);
                
                foreach (var tv in TileViews) {
                    foreach (var ev in tv.EntityViews) {
                        ev.OnUpdate(Controller).Forget();
                    }
                }
                
                await SortMemoryAsync();
            }
        }

        public void OnRemoveMemory(Controller controller, Entity entity) {
            RemoveMemoryAsync().Forget();

            async UniTask RemoveMemoryAsync() {
                await MemoryViews
                    .Single(v => v.EntityView != null && ReferenceEquals(v.EntityView.Entity, entity))
                    .RemoveEntityAsync();
                await SortMemoryAsync();
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
            var tile = Controller.Tiles.Single(t => t.Entities.Any(e => ReferenceEquals(e, entity)));
            Controller.Input(tile.Index);
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