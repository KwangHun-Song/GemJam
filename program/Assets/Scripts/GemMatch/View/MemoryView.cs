using System;
using Cysharp.Threading.Tasks;
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
            if (EntityView != null) {
                DestroyImmediate(EntityView);
            }
            
            EntityView = entityView;
        }

        public async UniTask RemoveEntityAsync(bool immediately = false) {
            if (EntityView == null) return;
            await EntityView.DestroyAsync();

            EntityView = null;
        }
    }
}