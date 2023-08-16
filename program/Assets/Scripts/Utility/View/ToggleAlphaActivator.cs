using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Utility {
    /// <summary>
    /// 임시로 만들었으며, 퍼포먼스가 좋지 않으므로 리소스를 적용한 후에 제거하기
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleAlphaActivator : MonoBehaviour {
        [SerializeField] private CanvasGroup canvasGroup;
        [Header("isOn 상태가 alpha가 1인 경우 체크해주세요.")]
        [SerializeField] private bool reversed;
        
        private Toggle toggle;
        private Toggle Toggle => toggle ??= GetComponent<Toggle>();

        private void Awake() {
            Toggle.onValueChanged.AddListener(ShowEffect);
        }

        public void ShowEffectWithoutAnim(bool isOn) {
            if (isOn) {
                canvasGroup.alpha = reversed ? 1 : 0;
            } else {
                canvasGroup.alpha = reversed ? 0 : 1;
            }
        }

        private void ShowEffect(bool isOn) {
            if (isOn) {
                canvasGroup.DOFade(reversed ? 1 : 0, 0.2F);
            } else {
                canvasGroup.DOFade(reversed ? 0 : 1, 0.1F);
            }
        }
    }
}