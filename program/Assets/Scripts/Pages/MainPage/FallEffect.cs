using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pages {
    public class FallEffect : MonoBehaviour {
        [SerializeField] private RectTransform startPoint;
        [SerializeField] private RectTransform referRect;
        [Header("랜덤 떨어지는 인터벌")]
        [SerializeField] private float minInterval = 3F;
        [SerializeField] private float maxInterval = 6F;
        [Header("떨어지는 시간")] 
        [SerializeField] private float minDur = 8F;
        [SerializeField] private float maxDur = 16F;
        [Header("랜덤 스케일")] 
        [SerializeField] private float minScale = 0.5F;
        [SerializeField] private float maxScale = 3F;
        [Header("랜덤 도는 횟수")] 
        [SerializeField] private int minRotateCount = 3;
        [SerializeField] private int maxRotateCount = 10;
        [Header("랜덤 도는 시간")] 
        [SerializeField] private float minRotateDur = 2F;
        [SerializeField] private float maxRotateDur = 6F;

        private GameObject fallGem;
        private GameObject FallGem => fallGem ??= Resources.Load<GameObject>(nameof(FallGem));

        private bool onEffect;

        private void OnEnable() {
            StartEffect();
        }

        public void StartEffect() {
            if (onEffect) return;
            StartEffectAsync().Forget();

            async UniTask StartEffectAsync() {
                onEffect = true;
                while (gameObject.activeSelf) {
                    ShowOne();
                    await UniTask.Delay((int)(Random.Range(minInterval, maxInterval) * 1000));
                }
                onEffect = false;
            }
        }

        public void ShowOne() {
            var referWidth = referRect.rect.width;
            var referHeight = referRect.rect.height;
            var gem = Instantiate(FallGem);
            var tfm = gem.transform as RectTransform;

            var randomDur = Random.Range(minDur, maxDur);
            var randomScale = Random.Range(minScale, maxScale);
            var randomStartX = Random.Range(0, referWidth) - referWidth / 2;
            
            tfm.SetParent(startPoint);
            tfm.anchoredPosition = Vector2.right * randomStartX;
            tfm.localScale = Vector3.one * randomScale;
            tfm.DOAnchorPosY(referHeight * -1.2F, randomDur)
                .OnStart(RotateRandomly)
                .OnComplete(() => DestroyImmediate(gem.gameObject));
                
            
            void RotateRandomly() {
                var randomAngle = Random.Range(0f, 360f) * Random.Range(minRotateCount, maxRotateCount) * RandomDirection();
                var randomDuration = Random.Range(minRotateDur, maxRotateDur);
                tfm.DORotate(new Vector3(0f, 0f, randomAngle), randomDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(RotateRandomly); // 끝나면 다시 실행하도록

                int RandomDirection() {
                    return Random.Range(0, 2) == 0 ? 1 : -1;
                }
            }
        }
    }
}