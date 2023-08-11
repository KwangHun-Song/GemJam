using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GemMatch {
    public class MemoryView : MonoBehaviour {
        [SerializeField] private Transform cellRoot;

        public Transform CellRoot => cellRoot;
        
        public EntityView EntityView { get; private set; }
        
        public void Initialize() {
            if (IsEmpty() == false) RemoveEntityAsync(true, true).Forget();
        }

        public bool IsEmpty() {
            return EntityView == null;
        }

        public async UniTask AddEntityAsync(EntityView entityView, bool immediately = false) {
            if (EntityView != null) {
                DestroyImmediate(EntityView);
            }
            
            EntityView = entityView;
            var tfm = EntityView.transform;
            tfm.SetParent(CellRoot);
            tfm.localPosition = Vector3.zero;
            tfm.localScale = Vector3.zero;
            tfm.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutBack);
        }

        public async UniTask RemoveEntityAsync(bool destroy, bool immediately = false) {
            if (EntityView == null) return;
            if (destroy) {
                await EntityView.DestroyAsync(immediately);
            }

            EntityView = null;
        }
    }
}