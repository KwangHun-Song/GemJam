using UnityEngine;

namespace GemMatch {
    public class View : MonoBehaviour, IGemMatchListener {
        [SerializeField] private Transform tileViewRoot;
        [SerializeField] private Transform memoryViewRoot;

        private TileView[] tileViews;
        private TileView[] TileViews => tileViews ??= tileViewRoot.GetComponents<TileView>();

        private MemoryView[] memoryViews;
        private MemoryView[] MemoryViews => memoryViews ??= memoryViewRoot.GetComponents<MemoryView>();
        
        public void OnStartGame(Tile[] tiles, Mission[] missions) {
            throw new System.NotImplementedException();
        }

        public void OnClearGame(Mission[] missions) {
            throw new System.NotImplementedException();
        }

        public void OnFailGame(Mission[] missions) {
            throw new System.NotImplementedException();
        }

        public void OnReplayGame(Mission[] missions) {
            throw new System.NotImplementedException();
        }

        public void OnAddMemory(ColorIndex color, int memoryIndex) {
            throw new System.NotImplementedException();
        }

        public void OnRemoveMemory(ColorIndex color) {
            throw new System.NotImplementedException();
        }
    }
}