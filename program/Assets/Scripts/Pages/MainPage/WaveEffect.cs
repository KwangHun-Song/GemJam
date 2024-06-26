using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Pages {
    public class WaveEffect : MonoBehaviour {
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private bool showOnce = false;

        private void Start() {
            if (showOnce) {
                ShowOnce();
            } else {
                PutBack();
            }
        }

        public void StartEffect(float duration, float delay, float wait) {
            StartEffectAsync().Forget();

            async UniTask StartEffectAsync() {
                await UniTask.Delay((int)(delay * 1000));

                DOTween.Sequence()
                    .SetId(GetInstanceID())
                    .AppendCallback(PutBack)
                    .Append(transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.InSine))
                    .AppendInterval(wait)
                    .SetLoops(-1);
            }
        }

        private void ShowOnce() {
            transform.SetParent(endPoint);
            transform.DOLocalMove(Vector3.zero, 3.5F).SetEase(Ease.InSine);
        }

        private void PutBack() {
            if (!transform) return;
            transform.SetParent(startPoint);
            transform.localPosition = Vector3.zero;
            transform.SetParent(endPoint);
            transform.SetAsLastSibling();
        }

        public void StopEffect() {
            DOTween.Kill(GetInstanceID());
        }
    }
}