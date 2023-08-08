using Cysharp.Threading.Tasks;
using DG.Tweening;
using GemMatch;
using UnityEngine;
using UnityEngine.UI;

namespace PagePopupSystem {
    public class FadeTransition : MonoBehaviour {
        [SerializeField] private Image curtain;
        
        public async UniTask FadeOut() {
            await curtain.DOFade(1, 0.66F).SetEase(Ease.OutSine).ToUniTask();
        }
        
        public async UniTask FadeIn() {
            await curtain.DOFade(0, 1F).SetEase(Ease.OutSine).ToUniTask();
        }
    }
}