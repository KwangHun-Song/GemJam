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
        private TileView[] TileViews => tileViews ??= tileViewRoot.GetComponents<TileView>();

        private MemoryView[] memoryViews;
        private MemoryView[] MemoryViews => memoryViews ??= memoryViewRoot.GetComponents<MemoryView>();
        
        public void OnStartGame(Tile[] tiles, Mission[] missions) {
            for (int i = 0; i < tileViews.Length; i++) {
                TileViews[i].Initialize(tiles[i]);
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

        public void OnAddMemory(Entity entity) {
            MemoryViews
                .First(v => v.IsEmpty())
                .AddEntityAsync(CreateEntityView(entity)).Forget();
        }

        public void OnRemoveMemory(Entity entity) {
            MemoryViews
                .Single(v => v.EntityView.Entity == entity)
                .RemoveEntityAsync().Forget();
        }

        private EntityView CreateEntityView(Entity entity) {
            var prefab = Resources.Load<EntityView>(entity.Index.ToString());
            var entityView = Instantiate(prefab);
            entityView.Initialize(entity);

            return entityView;
        }
    }
}