using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GemMatch {
    public class View : MonoBehaviour, IControllerEvent, ICollidableListener, ITouchableListener{
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
                AddListener(memoryView);
            }

            gameStatusText.text = "";
        }

        public void OnClearGame(Mission[] missions) {
            gameStatusText.text = "Completed!";
            ClearListeners();
        }

        public void OnFailGame(Mission[] missions) {
            gameStatusText.text = "failed!";
            ClearListeners();
        }

        public void OnReplayGame(Mission[] missions) { }

        public void OnAddMemory(Controller controller, Entity entity) {
            AddMemoryAsync().Forget();

            async UniTask AddMemoryAsync() {
                var memoryView = MemoryViews.First(v => v.IsEmpty());
                await memoryView.AddEntityAsync(CreateEntityView(entity, memoryView.CellRoot));
                await SortMemoryAsync();
            }
        }

        public void OnRemoveMemory(Controller controller, Entity entity) {
            RemoveMemoryAsync().Forget();

            async UniTask RemoveMemoryAsync() {
                await MemoryViews
                    .Single(v => v.EntityView.Entity == entity)
                    .RemoveEntityAsync();
                await SortMemoryAsync();
            }
        }

        public async UniTask SortMemoryAsync() {
            var sorted = MemoryViews
                .OrderBy(v => v.EntityView != null)
                .ThenBy(v => v.EntityView.Entity.Color);
            MemoryViews = sorted.ToArray();

            for (int i = MemoryViews.Length - 1; i >= 0; i--) {
                MemoryViews[i].transform.SetAsLastSibling();
            }
        }

        public void OnClickEntity(Entity entity) {
            var tile = Controller.Tiles.Single(t => t.Entities.Any(e => ReferenceEquals(e, entity)));
            Controller.Input(tile.Index);

            if (tile.Entities.Contains(entity) == false) {
                var tileView = TileViews.Single(tv => tv.Tile == tile);
                var entityView = tileView.RemoveEntityView(entity.Layer);
                entityView.DestroyAsync().Forget();
            }
        }

        internal EntityView CreateEntityView(Entity entity, Transform parent) {
            var prefab = Resources.Load<EntityView>(entity.Index.ToString());
            var view = Instantiate(prefab, parent, true);
            view.transform.localPosition = Vector3.zero;
            view.transform.localScale = Vector3.one;

            return view;
        }

        private void AddListener(object subject) {
            if (subject is IColliderable l1) {
                l1.Register(this);
            }

            if (subject is ITouchable l2) {
                l2.Register(this);
            }
        }

        private readonly List<IColliderable> collidedSubjects = new List<IColliderable>();
        private readonly List<ITouchable> touchedSubjects = new List<ITouchable>();

        public void NotifyOnCollide(IColliderable subject) {
            collidedSubjects.Add(subject);
        }

        public void NotifyOnTouch(ITouchable subject) {
            touchedSubjects.Add(subject);
        }
    }
}