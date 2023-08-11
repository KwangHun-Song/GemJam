using System;
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
        
        private Toggle toggle;
        private Toggle Toggle => toggle ??= GetComponent<Toggle>();

        private void Awake() {
            Toggle.onValueChanged.AddListener(ShowEffect);
        }

        private void ShowEffect(bool isOn) {
            if (isOn) {
                canvasGroup.DOFade(1, 0.2F);
            } else {
                canvasGroup.DOFade(0, 0.1F);
            }
        }
    }
}