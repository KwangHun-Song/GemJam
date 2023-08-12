using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OverlayStatusSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace GemMatch {
    public class MemoryView : MonoBehaviour {
        [SerializeField] private Transform cellRoot;

        public Transform CellRoot => cellRoot;
        
        public EntityView EntityView { get; private set; }

        public EntityModel EntityModel { get; private set; }
        
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
            EntityModel = EntityView.Entity.Model;
            var tfm = EntityView.transform;
            tfm.SetParent(CellRoot);
            tfm.localPosition = Vector3.zero;
            tfm.localScale = Vector3.zero;
            tfm.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutBack);
        }

        public float threshold = 1F;
        public float collectionDuration = 0.8F;
        public async UniTask RemoveEntityAsync(bool destroy, bool immediately = false) {
            if (EntityView == null) return;

            if (destroy) {
                var statusPosition = MissionStatusViewHolder.GetMissionStatusViewPosition(EntityModel);
                var cacheView = Instantiate(EntityView, statusPosition);
                MissionStatusViewHolder.Add(cacheView.gameObject);
                cacheView.gameObject.SetActive(false);

                await EntityView.DestroyAsync(immediately);
                var memoryPosition = this.transform.position;
                cacheView.transform.position = memoryPosition;

                cacheView.gameObject.SetActive(true);
                if (statusPosition != null) {
                    var wayPoints = new Vector3[] {
                        statusPosition.position,
                        memoryPosition + Vector3.down * threshold + Vector3.left * threshold,
                        memoryPosition,
                    };
                    cacheView.transform.DOPath(wayPoints, collectionDuration, PathType.CubicBezier).SetEase(Ease.InOutSine);
                    await  UniTask.Delay(TimeSpan.FromSeconds(collectionDuration));
                    DestroyImmediate(cacheView.gameObject);
                }
            }

            EntityView = null;
        }
    }
}