using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace GemMatch {
    public class MemoryView : MonoBehaviour {
        [SerializeField] private Transform cellRoot;

        public Transform CellRoot => cellRoot;
        
        public EntityView EntityView { get; private set; }
        
        public void Initialize() {
            if (IsEmpty() == false) RemoveEntityAsync(true).Forget();
        }

        public bool IsEmpty() {
            return EntityView == null;
        }

        public async UniTask AddEntityAsync(EntityView entityView, bool immediately = false) {
            EntityView = entityView;
        }

        public async UniTask RemoveEntityAsync(bool immediately = false) {
            if (EntityView == null) return;
            if (immediately == false) await EntityView.DestroyAsync();

            EntityView = null;
        }
    }
}