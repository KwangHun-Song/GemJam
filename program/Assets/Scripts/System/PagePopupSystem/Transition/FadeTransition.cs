using Cysharp.Threading.Tasks;
using DG.Tweening;
using GemMatch;
using UnityEngine;
using UnityEngine.UI;

namespace PagePopupSystem {
    public class FadeTransition : MonoBehaviour {
        [SerializeField] private Image curtain;
        
        public bool OnAnimation { get; private set; }
        public bool IsFadeOut { get; private set; }
        
        public async UniTask FadeOut() {
            OnAnimation = true;
            await curtain.DOFade(1, 0.66F).SetEase(Ease.OutSine).ToUniTask();
            OnAnimation = false;
            IsFadeOut = true;
        }
        
        public async UniTask FadeIn() {
            OnAnimation = true;
            await curtain.DOFade(0, 1F).SetEase(Ease.OutSine).ToUniTask();
            OnAnimation = false;
            IsFadeOut = false;
        }
    }
}